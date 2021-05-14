using System;
using System.IO;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public class VerifyProvider
  {
    #region Properties, Constructors

    public event StateChange OnStateChange;

    private AppSettings _appSettings;
    private SQLProvider _sqlProvider;
    //private bool _existInitialCatalog = true;

    public VerifyProvider(AppSettings appSettings)
    {
      _appSettings = appSettings;

      _sqlProvider = new SQLProvider(appSettings);
    }

    #endregion

    public bool Verify(out bool isDbExist)
    {
      isDbExist = false;
      OnStateChange?.Invoke(MessageType.MESSAGE, "Checking the publish path is valid...");
      var validPath = (Directory.Exists(_appSettings.IISConfig.PhysicalPath) || Utils.CheckPathAnyAccess(_appSettings.IISConfig.PhysicalPath));
      if (!validPath)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "The publish path is NOT valid or IMPOSSIBLE to access" + Environment.NewLine + "Path: " + _appSettings.IISConfig.PhysicalPath);
        return false;
      }
      var isUsed = CheckPublicPathIsUsed(out SiteInfo site);
      if (isUsed)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "The publish path is used by other site: " + site.SiteName);
        return false;
      }
      else
      {
        OnStateChange?.Invoke(MessageType.MESSAGE, "The publish path is OKAY.");
      }

      OnStateChange?.Invoke(MessageType.MESSAGE, "SQL server - checking the connection to [master] with admin...");
      var isConnected = _sqlProvider.IsConnectedToSqlServer(_appSettings.SQLConfig.ConnectionString, "master", _appSettings.SQLConfig.AdminUsername, _appSettings.SQLConfig.AdminPassword);
      if (isConnected)
      {
        OnStateChange?.Invoke(MessageType.MESSAGE, "The connection is OKAY.");
      }
      else
      {
        OnStateChange?.Invoke(MessageType.ERROR, "SQL server - can not connect to admin.");
        return false;
      }

      OnStateChange?.Invoke(MessageType.MESSAGE, $"SQL server - checking if the user exist on server with admin...");
      var isUserExist = _sqlProvider.IsUsernameExist(_appSettings.SQLConfig.ConnectionString, "master", _appSettings.SQLConfig.AdminUsername, _appSettings.SQLConfig.AdminPassword, _appSettings.SQLConfig.Username);
      if (isUserExist)
      {
        OnStateChange?.Invoke(MessageType.MESSAGE, $"User [{_appSettings.SQLConfig.Username}] exists");
      }
      else
      {
        OnStateChange?.Invoke(MessageType.MESSAGE, $"User [{_appSettings.SQLConfig.Username}] does not exists");
        return false;
      }

      OnStateChange?.Invoke(MessageType.MESSAGE, $"SQL server - checking the connection to [{_appSettings.SQLConfig.Catalog}] with admin...");
      isConnected = _sqlProvider.IsConnectedToSqlServer(_appSettings.SQLConfig.ConnectionString, _appSettings.SQLConfig.Catalog, _appSettings.SQLConfig.AdminUsername, _appSettings.SQLConfig.AdminPassword);
      if (isConnected)
      {
        OnStateChange?.Invoke(MessageType.MESSAGE, $"DB [{_appSettings.SQLConfig.Catalog}] exists");
      }
      else
      {
        OnStateChange?.Invoke(MessageType.ERROR, $"DB [{_appSettings.SQLConfig.Catalog}] does not exists");

        OnStateChange?.Invoke(MessageType.MESSAGE, $"SQL server - granting [master] permission to user [{_appSettings.SQLConfig.Username}] with admin...");
        var isGranted = _sqlProvider.GrantPermissionToUser(_appSettings.SQLConfig.ConnectionString, "master", _appSettings.SQLConfig.AdminUsername, _appSettings.SQLConfig.AdminPassword, _appSettings.SQLConfig.Username);

        if (isGranted)
        {
          OnStateChange?.Invoke(MessageType.MESSAGE, $"Granted successfully");
        }
        else
        {
          OnStateChange?.Invoke(MessageType.ERROR, $"SQL server - granting fail");
          return false;
        }

        OnStateChange?.Invoke(MessageType.MESSAGE, $"SQL server - checking the connection to [master] with user [{_appSettings.SQLConfig.Username}]...");
        var isConnectedWithUser = _sqlProvider.IsConnectedToSqlServer(_appSettings.SQLConfig.ConnectionString, "master", _appSettings.SQLConfig.Username, _appSettings.SQLConfig.Password);

        if (isConnectedWithUser)
        {
          OnStateChange?.Invoke(MessageType.MESSAGE, $"The connection is OKAY. User [{_appSettings.SQLConfig.Username}] information is correct");
        }
        else
        {
          OnStateChange?.Invoke(MessageType.ERROR, $"Sql server - can not connect to [master] with user. User [{_appSettings.SQLConfig.Username}] information is incorrect");
        }

        OnStateChange?.Invoke(MessageType.MESSAGE, $"SQL server - revokinng [master] permission of user [{_appSettings.SQLConfig.Username}] with admin...");
        var isRevoked = _sqlProvider.RevokePermission(_appSettings.SQLConfig.ConnectionString, "master", _appSettings.SQLConfig.AdminUsername, _appSettings.SQLConfig.AdminPassword, _appSettings.SQLConfig.Username);

        if (isRevoked)
        {
          OnStateChange?.Invoke(MessageType.MESSAGE, $"Revoked successfully");
        }
        else
        {
          OnStateChange?.Invoke(MessageType.ERROR, $"Sql server - revoked failed");
        }

        if (isConnectedWithUser)
          return true;
        else
          return false;
      }

      isDbExist = true;
      OnStateChange?.Invoke(MessageType.MESSAGE, $"SQL server - checking the connection to [{_appSettings.SQLConfig.Catalog}] with user [{_appSettings.SQLConfig.Username}]...");
      isConnected = _sqlProvider.IsConnectedToSqlServer(_appSettings.SQLConfig.ConnectionString);
      if (isConnected)
      {
        OnStateChange?.Invoke(MessageType.MESSAGE, "The connection is OKAY.");
        return true;
      }

      // Catalog exist, user exist -> user don't have permission to the catalog
      OnStateChange?.Invoke(MessageType.ERROR, $"Sql server - can not connect with user. User [{_appSettings.SQLConfig.Username}] doesn't has permission to [{_appSettings.SQLConfig.Catalog}]");
      
      return false;
    }

    #region Helper methods

    private bool CheckPublicPathIsUsed(out SiteInfo site)
    {
      var trimChars = new char[] { ' ', '\\' };
      var appPath = _appSettings.IISConfig.PhysicalPath.ToLower().Trim(trimChars);
      site = Global.Sites.Find(t => 
        t.SiteName.Trim().ToLower() != _appSettings.IISConfig.WebSiteName.Trim().ToLower()
        && t.PhysicalPath.ToLower().Trim(trimChars) == appPath);
      var isUsed = site != null;
      return isUsed;
    }

    #endregion
  }
}
