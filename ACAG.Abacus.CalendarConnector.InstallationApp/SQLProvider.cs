using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public enum ActionSQLEnum
  {
    UPDATE = 0,
    CREATE = 1,
    NOT_UPDATE = 2
  }

  public class SQLFile
  {
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FromVersion { get; set; }
    public string ToVersion { get; set; }
  }

  public class SQLProvider
  {
    public event StateChange OnStateChange;

    private const string _folderSQL = "data\\SQL";
    private const string _folderScript = "data\\SQL\\Script";

    private bool _resultInstall = false;
    private AppSettings _appSettings;

    const string _dbShemaFileName = "1DatabaseShemas";
    private string _dbGO = "GO";

    private string _databaseName, _initalCatalog, _sqlUserId, _sqlPassword,
                   _sqlConnectionString, _sqlConnectionStringMaster, _createDatabaseQuery,
                   _folderSQLPath, _folderScriptPath;
    
    public SQLProvider(AppSettings appSettings)
    {
      _appSettings = appSettings;
    }
    
    public bool Install()
    {
      OnStateChange?.Invoke(MessageType.TITLE, "BEGIN IMPLEMENT SQL SERVER APPLICATION...");
      try
      {
        InitConfiguration();

        var action = GetAction(_sqlConnectionString);
        switch (action)
        {
          case ActionSQLEnum.UPDATE:
            {
              OnStateChange?.Invoke(MessageType.TITLE, "Begin running SQL scripts to update...");
              _resultInstall = UpdateDatabase();
              break;
            }
          case ActionSQLEnum.CREATE:
            {
              OnStateChange?.Invoke(MessageType.TITLE, "Begin creating a new database...");
              _resultInstall = CreateDatabase();
              break;
            }
          case ActionSQLEnum.NOT_UPDATE:
            {
              OnStateChange?.Invoke(MessageType.MESSAGE, "Database is up to date.");
              _resultInstall = true;
              break;
            }
        }

        if (_resultInstall)
        {
          OnStateChange?.Invoke(MessageType.MESSAGE, "Done implementing Database.");
          return true;
        }
        else
        {
          OnStateChange?.Invoke(MessageType.ERROR, "Error when implement Database.");
          return false;
        }
      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "Error: " + Environment.NewLine + ex.Message);
        return false;
      }
    }

    public static bool IsSqlServerInstalled()
    {
      RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

      try
      {
        using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
        {
          RegistryKey instanceKey = hklm.OpenSubKey(@"Software\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);

          if (instanceKey != null)
          {
            var instances = instanceKey.GetValueNames().Length;

            if (instances > 0)
              return true;
          }

          return false;
        }
      }
      catch
      {
        return false;
      }
    }

    public static string GetSqlServerVersion()
    {
      RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
      try
      {
        using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
        {
          RegistryKey instanceKey = hklm.OpenSubKey(@"Software\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);

          if (instanceKey == null)
          {
            return null;
          }
          string[] instanceNames = instanceKey.GetValueNames();
          if (instanceNames.Length == 0)
          {
            return null;
          }

          foreach (var instance in instanceNames)
          {
            var instanceValue = instanceKey.GetValue(instance);
            RegistryKey key = hklm.OpenSubKey(@$"SOFTWARE\Microsoft\Microsoft SQL Server\{instanceValue}\MSSQLServer\CurrentVersion", false);
            if (key != null)
            {
              var res = key.GetValue("CurrentVersion").ToString();
              return res;
            }
          }
        }
      }
      catch
      {
      }
      return null;
    }

    public bool GrantPermissionToUser(string connectionString, string dbName, string userAdmin, string passwordAdmin, string targetUser)
    {
      SqlConnectionStringBuilder builderAdmin = null;
      try
      {
        builderAdmin = new SqlConnectionStringBuilder(connectionString);
        builderAdmin.InitialCatalog = dbName;
        builderAdmin.UserID = userAdmin;
        builderAdmin.Password = passwordAdmin;
        
        using (SqlConnection connection = new SqlConnection(builderAdmin.ConnectionString))
        {
          connection.Open();
            
          string query = $@"
            DECLARE @currentDbUser NVARCHAR(max)
            SET @currentDbUser = (
                SELECT  dp.name
                FROM sys.server_principals sp
                JOIN sys.database_principals dp on dp.sid = sp.sid
                WHERE sp.name = '{targetUser}'
            )

            IF (LEN(@currentDbUser) IS NULL OR LEN(@currentDbUser) = 0)
            BEGIN 
	            SET @currentDbUser = 'non___exist___user___'
            END

            DECLARE @query NVARCHAR(MAX)
            SET @query = CONCAT('IF EXISTS (SELECT * FROM sys.database_principals WHERE name =', '''', @currentDbUser, ''') BEGIN DROP USER ', @currentDbUser, ' END');

            EXEC sp_executesql @query

            IF EXISTS (SELECT * FROM sys.database_principals WHERE name = N'{targetUser}')
            BEGIN
	            DROP USER {targetUser}
            END

            IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'{targetUser}')
            BEGIN
                CREATE USER [{targetUser}] FOR LOGIN [{targetUser}]
                EXEC sp_addrolemember N'db_owner', N'{targetUser}'
            END;";

          using (var sqlCmd = new SqlCommand(query, connection))
          {
            sqlCmd.ExecuteNonQuery();
          }

          connection.Close();
          return true;
        }
      }
      catch
      {
        return false;
      }
    }
    
    public bool RevokePermission(string connectionString, string dbName, string userAdmin, string passwordAdmin, string targetUser)
    {
      SqlConnectionStringBuilder builderAdmin = null;

      try
      {
        builderAdmin = new SqlConnectionStringBuilder(connectionString);
        builderAdmin.InitialCatalog = dbName;
        builderAdmin.UserID = userAdmin;
        builderAdmin.Password = passwordAdmin;

        using (SqlConnection connection = new SqlConnection(builderAdmin.ConnectionString))
        {
          connection.Open();
          string query = $"DROP USER {targetUser}";

          using (var sqlCmd = new SqlCommand(query, connection))
          {
            sqlCmd.ExecuteNonQuery();
          }

          connection.Close();
          return true;
        }
      }
      catch
      {
        return false;
      }
    }

    private ActionSQLEnum GetAction(string connectionString)
    {
      if (IsConnectedToSqlServer(connectionString))
      {
        var currentVer = Utils.GetCurrentDbVersion(connectionString, out var dbName, out var canConnected);
        var nextVer = Global.Infor.Package.DatabaseVersion.Data;

        if (!IsVersionCabableOfUpdating(currentVer, nextVer))
        {
          return ActionSQLEnum.NOT_UPDATE;
        }
        return ActionSQLEnum.UPDATE;
      }
      else
      {
        return ActionSQLEnum.CREATE;
      }
    }

    private bool CreateDatabase()
    {
      try
      {
        CheckAndCreateDatabaseName();

        #region Grant Catalog permission for User

        OnStateChange?.Invoke(MessageType.MESSAGE, $"Grating [{_appSettings.SQLConfig.Catalog}] permission to user [{_appSettings.SQLConfig.Username}] with admin");

        var grantedPermission = GrantPermissionToUser(_sqlConnectionString, _appSettings.SQLConfig.Catalog, _appSettings.SQLConfig.AdminUsername, _appSettings.SQLConfig.AdminPassword, _appSettings.SQLConfig.Username);

        if (!grantedPermission)
        {
          OnStateChange?.Invoke(MessageType.ERROR, $"GRANTED FAIL...");
          return false;
        }

        #endregion

        #region Run schema script

        var sqlSchemaAndDefaultValuePaths = GetAllSqlFileByPath(_folderSQLPath);

        string schemaScriptPath = sqlSchemaAndDefaultValuePaths.Where(t => t.Contains(_dbShemaFileName)).FirstOrDefault();
        if (string.IsNullOrEmpty(schemaScriptPath))
        {
          OnStateChange?.Invoke(MessageType.ERROR, "Cannot find SQL Schema Script...");
          return false;
        }

        var text = File.ReadAllText(schemaScriptPath);
        if (!ExecuteSchemaQuery(_sqlConnectionString, text))
        {
          return false;
        }

        #endregion Run schema script 

        UpdateSQLToNextVersion();

        return true;
      }
      catch (Exception ex)
      {
        ex.Message.ToString();
        OnStateChange?.Invoke(MessageType.ERROR, "IMPLEMENT CREATE SQL SERVER APPLICATION FAIL...");
        return false;
      }
    }

    private bool UpdateDatabase()
    {
      try
      {
        UpdateSQLToNextVersion();

        return true;
      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "Error when run updating SQL scripts..." + Environment.NewLine + ex.Message);
        return false;
      }
    }

    private void InitConfiguration()
    {
      var builder = new SqlConnectionStringBuilder(_appSettings.SQLConfig.ConnectionString);
      _databaseName = builder.DataSource;
      _initalCatalog = builder.InitialCatalog;
      _sqlUserId = _appSettings.SQLConfig.AdminUsername;
      _sqlPassword = _appSettings.SQLConfig.AdminPassword;

      _sqlConnectionString = _appSettings.SQLConfig.ConnectionString;
      _sqlConnectionStringMaster = $"Data Source={_databaseName};Initial Catalog=master;User ID={_sqlUserId};Password={_sqlPassword}";
      _createDatabaseQuery = $"CREATE DATABASE[{_initalCatalog}]";

      _folderSQLPath = Path.Combine(Utils.CurrentFolder, _folderSQL);
      _folderScriptPath = Path.Combine(Utils.CurrentFolder, _folderScript);
    }

    private bool UpdateSQLToNextVersion()
    {
      try
      {
        var currentVer = Utils.GetCurrentDbVersion(_sqlConnectionString, out string dbName, out var canConnected);
        var nextVer = Utils.GetPackageDbVersion();

        if (currentVer == nextVer)
        {
          return true;
        }

        var allSqlScriptFiles = GetAllSqlFileVersionNameByPath(_folderScriptPath);

        allSqlScriptFiles = allSqlScriptFiles.OrderBy(t => new Version(t.FromVersion)).ThenBy(t => new Version(t.ToVersion)).ToList();

        var currentVersionIndex = allSqlScriptFiles.IndexOf(allSqlScriptFiles.Where(t => t.FromVersion == currentVer).FirstOrDefault());
        var nextVersionIndex = allSqlScriptFiles.IndexOf(allSqlScriptFiles.Where(t => t.ToVersion == nextVer).FirstOrDefault());

        if (currentVersionIndex == -1 || nextVersionIndex == -1 || currentVersionIndex > nextVersionIndex)
        {
          OnStateChange?.Invoke(MessageType.ERROR, "Cannot find sql version script..");
          return false;
        }

        List<SQLFile> lstSqlNeedToUpdate = allSqlScriptFiles.GetRange(currentVersionIndex, nextVersionIndex - currentVersionIndex + 1).ToList();

        foreach (var sqlFile in lstSqlNeedToUpdate)
        {
          OnStateChange?.Invoke(MessageType.MESSAGE, String.Format("Start excuting script from version: {0} - to version: {1}", sqlFile.FromVersion, sqlFile.ToVersion));
          ExecuteNonQuery(_sqlConnectionString, File.ReadAllText(sqlFile.FilePath));
        }

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }

    }


    #region HELPER METHOD

    public bool IsConnectedToSqlServer(string connectionString)
    {

      if (connectionString == null)
        return false;

      try
      {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          connection.Open();
          return true;
        }
      }
      catch
      {
        return false;
      }
    }

    public bool IsConnectedToSqlServer(string connectionString, string dbName, string username, string password)
    {
      SqlConnectionStringBuilder builder = null;
      try
      {
        builder = new SqlConnectionStringBuilder(connectionString);
        builder.InitialCatalog = dbName;
        builder.UserID = username;
        builder.Password = password;
      }
      catch
      {
        return false;
      }

      var isConnected = IsConnectedToSqlServer(builder.ConnectionString);
      return isConnected;

    }

    public bool IsUsernameExist(string connectionString, string dbName, string username, string password, string targetUsername)
    {
      SqlConnectionStringBuilder builder = null;
      try
      {
        builder = new SqlConnectionStringBuilder(connectionString);
        builder.InitialCatalog = dbName;
        builder.UserID = username;
        builder.Password = password;
      }
      catch
      {
        return false;
      }

      var isUsernameExist = IsUsernameExist(builder.ConnectionString, targetUsername);
      return isUsernameExist;
    }

    private bool IsUsernameExist(string connectionString, string username)
    {
      if (connectionString == null)
        return false;

      var sqlQuery = $"SELECT * FROM sys.server_principals WHERE name = '{username}'";

      return ExecuteReader(connectionString, sqlQuery);
    }

    private bool IsVersionCabableOfUpdating(string currentVersion, string nextVersion)
    {
      if (currentVersion == null || nextVersion == null)
        return false;

      int result;

      try
      {
        var currVer = new Version(currentVersion);
        var nextVer = new Version(nextVersion);
        result = currVer.CompareTo(nextVer);
      }
      catch
      {
        return false;
      }

      return result < 0;
    }

    private bool ExecuteSchemaQuery(string connectionString, string query)
    {
      try
      {
        if (!connectionString.ToLower().Contains("integrated"))
        {
          connectionString = connectionString + ";Integrated security=False;";
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          connection.Open();
          var dataQuery = query.Split(new string[] { _dbGO }, StringSplitOptions.None);
          foreach (var item in dataQuery)
          {
            using (var sqlCmd = new SqlCommand(item, connection))
            {
              if (!string.IsNullOrEmpty(item))
              {
                sqlCmd.CommandText = item;
                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.ExecuteNonQuery();
              }
            }
          }
          connection.Close();
        }
        return true;
      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "Error executing Schema script..." + Environment.NewLine + ex.Message);
        return false;
      }
    }

    private bool ExecuteNonQuery(string connectionString, string query)
    {
      try
      {
        if (!connectionString.ToLower().Contains("integrated"))
        {
          connectionString = connectionString + ";Integrated security=False;";
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          connection.Open();
          using (var sqlCmd = new SqlCommand(query, connection))
          {
            sqlCmd.ExecuteNonQuery();
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "Error when run Sql script..." + Environment.NewLine + ex.Message);
        return false;
      }
    }

    private bool ExecuteReader(string connectionString, string query)
    {
      try
      {
        if (!connectionString.ToLower().Contains("integrated"))
        {
          connectionString = connectionString + ";Integrated security=False;";
        }
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          connection.Open();
          using (var sqlCmd = new SqlCommand(query, connection))
          {
            SqlDataReader reader = sqlCmd.ExecuteReader();
            return reader.Read();
          }
        }
      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "Error when run Sql script..." + Environment.NewLine + ex.Message);
        return false;
      }
    }

    private bool CheckAndCreateDatabaseName()
    {
      using (var con = new SqlConnection(_sqlConnectionStringMaster))
      {
        var command = new SqlCommand($"SELECT * FROM master.dbo.sysdatabases where [name] like N'{_initalCatalog}'", con);
        if (con.State != System.Data.ConnectionState.Open) con.Open();
        var reader = command.ExecuteReader();
        command.Dispose();

        if (!reader.HasRows)
        {
          ExecuteNonQuery(_sqlConnectionStringMaster, _createDatabaseQuery);
        }

        reader.Close();
        return true;
      }
    }

    private List<string> GetAllSqlFileByPath(string path)
    {
      DirectoryInfo d = new DirectoryInfo(path);
      FileInfo[] files = d.GetFiles("*.sql");
      List<string> str = new List<string>();
      foreach (FileInfo file in files)
      {
        str.Add(file.FullName);
      }
      return str;
    }

    private List<SQLFile> GetAllSqlFileVersionNameByPath(string pathFolder)
    {
      DirectoryInfo d = new DirectoryInfo(pathFolder);
      FileInfo[] files = d.GetFiles("*.sql");
      List<SQLFile> result = new List<SQLFile>();
      foreach (FileInfo file in files)
      {
        SQLFile newModel = new SQLFile()
        {
          FileName = file.Name.Replace(".sql", ""),
          FilePath = file.FullName
        };
        newModel = BindFileSqlVersion(newModel);
        result.Add(newModel);
      }
      return result;
    }

    private SQLFile BindFileSqlVersion(SQLFile model)
    {
      string fileName = model.FileName;
      var startIndexOfFrom = fileName.IndexOf("_from_") + string.Format("_from_").Length;
      var endIndexOfFrom = fileName.IndexOf("_to_");

      var startIndexOfTo = fileName.IndexOf("_to_") + string.Format("_to_").Length;

      var fromVersion = fileName.Substring(startIndexOfFrom, endIndexOfFrom - startIndexOfFrom);
      var toVersion = fileName.Substring(startIndexOfTo, fileName.Length - startIndexOfTo);

      model.FromVersion = fromVersion;
      model.ToVersion = toVersion;
      return model;
    }

    #endregion
  }
}
