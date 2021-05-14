using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.Web.Administration;
using Microsoft.Win32;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public static class Utils
  {
    #region Variables

    public static string _iisRegKey = @"Software\Microsoft\InetStp";
    public static string _sqlServerRegKey = @"Software\Microsoft\Microsoft SQL Server\Instance Names\SQL";
    public static string _installedFolderRegKey = @"";

    public static string CurrentFolder { get; set; }

    public static string _tempFolder => $@"{CurrentFolder}\data";

    public static string _fileZipName => "data.zip";
    public static string _fileAppSettingName => "appsettings.json";
    public static string _fileWebAppAppSettingName => "appsettings.json";
    public static string _fileDllAppName => "ACAG.Abacus.CalendarConnector.Server.dll";

    public static string _fileZipPath => $@"{CurrentFolder}\{_fileZipName}";
    public static string _fileAppSettingPath => $@"{CurrentFolder}\{_fileAppSettingName}";
    public static string _fileAppSettingWebAppPath => $@"{CurrentFolder}\data\IIS\server\{_fileWebAppAppSettingName}";
    public static string _fileDllAppPath => $@"data\IIS\server\{_fileDllAppName}";

    public static string _fileTextDbVersionEntry => "data/SQL/DBVersion.txt";

    #endregion

    #region Get methods
    public static string GetCurrentDbVersion(string connectionString, out string dbName, out bool? canConnected)
    {
      dbName = null;
      canConnected = null;
      if (connectionString == null)
        return null;

      if (!connectionString.ToLower().Contains("integrated"))
      {
        connectionString = connectionString + ";Integrated security=False;";
      }

      string currentVersion;
      string queryString = " SELECT [Value] FROM AppSetting WHERE Id = N'DATABASEVERSION' ";

      try
      {
        canConnected = false;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
          dbName = connection.Database;
          connection.Open();
          canConnected = true;

          using (SqlCommand command = new SqlCommand(queryString, connection))
          {
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = queryString;

            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            currentVersion = reader[0].ToString();
            reader.Close();
          }

          connection.Close();

          return currentVersion;
        }
      }
      catch
      {
        return null;
      }
    }

    public static string GetPackageDbVersion()
    {
      string nextVersion;

      try
      {
        using (FileStream zipToOpen = new FileStream(_fileZipPath, FileMode.Open))
        {
          using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
          {
            ZipArchiveEntry readmeEntry = archive.GetEntry(_fileTextDbVersionEntry);
            using (StreamReader reader = new StreamReader(readmeEntry.Open()))
            {
              nextVersion = reader.ReadToEnd();
            }
          }
        }
      }
      catch (Exception)
      {
        return null;
      }

      return nextVersion;
    }

    public static string ReadConnectionString(string appSettingPath)
    {
      try
      {
        string fileAppsetting = File.ReadAllText(appSettingPath);
        dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(fileAppsetting);

        var res = jsonObj["ConnectionStrings"]["CalendarConnectorContext"].ToString();
        return res;
      }
      catch
      {
        return null;
      }
    }

    public static MailSettings ReadEmailSetting(string appSettingPath)
    {
      try
      {
        string fileAppsetting = File.ReadAllText(appSettingPath);
        dynamic jObject = Newtonsoft.Json.JsonConvert.DeserializeObject(fileAppsetting);

        var res = new MailSettings
        {
          EmailSelector = jObject["MailSettings"]["MailSelector"],
          Smtp = new SmtpEmail
          {
            SenderName = jObject["MailSettings"]["Smtp"]["SenderName"],
            Mail = jObject["MailSettings"]["Smtp"]["Mail"],
            Password = jObject["MailSettings"]["Smtp"]["Password"],
            Host = jObject["MailSettings"]["Smtp"]["Host"],
            Port = jObject["MailSettings"]["Smtp"]["Port"]
          },
          SendGrid = new SendGridEmail
          {
            SenderName = jObject["MailSettings"]["SendGrid"]["SenderName"],
            Account = jObject["MailSettings"]["SendGrid"]["Account"],
            Key = jObject["MailSettings"]["SendGrid"]["Key"],
          }
        };

        return res;
      }
      catch
      {
        return null;
      }
    }

    public static bool WriteAppSettings(AppSettings settings)
    {
      try
      {
        string fileAppsetting = File.ReadAllText(_fileAppSettingWebAppPath);
        dynamic jObject = Newtonsoft.Json.JsonConvert.DeserializeObject(fileAppsetting);

        jObject["ConnectionStrings"]["CalendarConnectorContext"] = settings.SQLConfig.ConnectionString;

        jObject["MailSettings"]["MailSelector"] = settings.EmailSettings.EmailSelector;

        jObject["MailSettings"]["Smtp"]["SenderName"] = settings.EmailSettings.Smtp.SenderName;
        jObject["MailSettings"]["Smtp"]["Mail"] = settings.EmailSettings.Smtp.Mail;
        jObject["MailSettings"]["Smtp"]["Password"] = settings.EmailSettings.Smtp.Password;
        jObject["MailSettings"]["Smtp"]["Host"] = settings.EmailSettings.Smtp.Host;
        jObject["MailSettings"]["Smtp"]["Port"] = settings.EmailSettings.Smtp.Port;

        jObject["MailSettings"]["SendGrid"]["SenderName"] = settings.EmailSettings.SendGrid.SenderName;
        jObject["MailSettings"]["SendGrid"]["Account"] = settings.EmailSettings.SendGrid.Account;
        jObject["MailSettings"]["SendGrid"]["Key"] = settings.EmailSettings.SendGrid.Key;

        jObject["AbacusApiSettings"]["Url"] = string.IsNullOrEmpty(settings.IISConfig.HostName) ?
         string.Format("http://localhost:{0}/abacus",settings.IISConfig.Port) : settings.IISConfig.HostName + "/abacus";


        string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(_fileAppSettingWebAppPath, output);

        return true;
      }
      catch
      {
        return false;
      }
    }

    public static string GetPackageAppVersion()
    {
      var versionBytes = ReadPackageAppVersion();
      if (versionBytes == null)
        return null;

      var assembly = Assembly.Load(versionBytes);
      var fullName = assembly.FullName;
      var version = fullName.Split(',')
        .FirstOrDefault(t => t.Contains("Version"));
      if (version == null)
        return null;

      var res = version.Split('=').Last();

      return res;
    }

    private static byte[] ReadPackageAppVersion()
    {
      var fileZipPath = $@"{CurrentFolder}\data.zip";
      try
      {
        using (FileStream zipToOpen = new FileStream(fileZipPath, FileMode.Open))
        {
          using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
          {
            ZipArchiveEntry readmeEntry = archive.GetEntry($"data/IIS/server/{IISProvicer.APP_DLL_NAME}");

            using (var stream = readmeEntry.Open())
            {
              return ReadToEnd(stream);
            }

          }
        }
      }
      catch (Exception)
      {
        return null;
      }
    }

    public static byte[] ReadToEnd(System.IO.Stream stream)
    {
      long originalPosition = 0;

      if (stream.CanSeek)
      {
        originalPosition = stream.Position;
        stream.Position = 0;
      }

      try
      {
        byte[] readBuffer = new byte[4096];

        int totalBytesRead = 0;
        int bytesRead;

        while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
        {
          totalBytesRead += bytesRead;

          if (totalBytesRead == readBuffer.Length)
          {
            int nextByte = stream.ReadByte();
            if (nextByte != -1)
            {
              byte[] temp = new byte[readBuffer.Length * 2];
              Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
              Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
              readBuffer = temp;
              totalBytesRead++;
            }
          }
        }

        byte[] buffer = readBuffer;
        if (readBuffer.Length != totalBytesRead)
        {
          buffer = new byte[totalBytesRead];
          Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
        }
        return buffer;
      }
      finally
      {
        if (stream.CanSeek)
        {
          stream.Position = originalPosition;
        }
      }
    }

    public static string GetAppVersion(string dllPath)
    {
      if (dllPath == null)
        return null;

      string version;

      try
      {
        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(dllPath);
        version = myFileVersionInfo?.ProductVersion;
      }
      catch (Exception ex)
      {
        return null;
      }

      return version;
    }

    public static string GetSitePhysicPath(string siteName)
    {
      if (siteName == null)
        return null;

      try
      {
        ServerManager iisManager = new ServerManager();
        var site = iisManager.Sites.Where(s => s.Name == siteName).SingleOrDefault();

        if (site == null)
        {
          return _fileDllAppPath;
        }

        var applicationRoot = site.Applications.Where(a => a.Path == "/").Single();
        var virtualRoot = applicationRoot.VirtualDirectories.Where(v => v.Path == "/").Single();

        return virtualRoot.PhysicalPath;
      }
      catch (Exception ex)
      {
        return null;
      }
    }

    #endregion

    public static void ExtractToDirectory(string zipFilePath, string destinationDirectory)
    {
      using (var file = File.OpenRead(zipFilePath))
      {
        using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
        {
          ExtractToDirectory(zip, destinationDirectory);
        }
      }
    }

    public static bool TrySetPermission(string destinationDirectory)
    {
      try
      {
        var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
        FileSystemRights rights = FileSystemRights.FullControl;

        // *** Add Access Rule to the actual directory itself
        FileSystemAccessRule accessRule = new FileSystemAccessRule(sid, rights,
                                    InheritanceFlags.None,
                                    PropagationFlags.NoPropagateInherit,
                                    AccessControlType.Allow);

        DirectoryInfo info = new DirectoryInfo(destinationDirectory);
        DirectorySecurity Security = info.GetAccessControl(AccessControlSections.Access);

        bool result;
        Security.ModifyAccessRule(AccessControlModification.Set, accessRule, out result);

        if (!result)
          return false;

        // *** Always allow objects to inherit on a directory
        InheritanceFlags iFlags = InheritanceFlags.ObjectInherit;
        iFlags = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;

        // *** Add Access rule for the inheritance
        accessRule = new FileSystemAccessRule(sid, rights,
                                    iFlags,
                                    PropagationFlags.InheritOnly,
                                    AccessControlType.Allow);

        Security.ModifyAccessRule(AccessControlModification.Add, accessRule, out result);

        if (!result)
          return false;

        info.SetAccessControl(Security);

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private static void ExtractToDirectory(ZipArchive archive, string destinationDirectoryName)
    {
      foreach (ZipArchiveEntry file in archive.Entries)
      {
        string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
        if (file.Name == "")
        {
          Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
          continue;
        }
        file.ExtractToFile(completeFileName, true);
      }
    }

    public static bool IsPackageExtracted()
    {
      if (Directory.Exists(_tempFolder)) return true;

      string zipFilePath = $@"{CurrentFolder}\{_fileZipName}";
      string destinationDirectory = $@"{CurrentFolder}";

      try
      {
        Utils.ExtractToDirectory(zipFilePath, destinationDirectory);

        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static void DeleteTempFolder()
    {
      try
      {
        bool dirExists = Directory.Exists(_tempFolder);

        if (!dirExists)
        {
          return;
        }

        Directory.Delete(_tempFolder, true);
      }
      catch (Exception ex)
      {
        return;
      }
    }

    public static bool CheckPathAnyAccess(string path)
    {
      var levelDir = new LevelDirectory(path);
      return levelDir.CheckAnyAccess();
    }
  }


  public static class MessageExtensions
  {
    public static void AppendText(this RichTextBox rtxtBox, Color color, string message)
    {
      rtxtBox.InvokeControl(() =>
      {
        rtxtBox.SelectionStart = rtxtBox.TextLength;
        rtxtBox.SelectionLength = 0;

        rtxtBox.SelectionColor = color;
        rtxtBox.AppendText(message + Environment.NewLine);
        rtxtBox.SelectionColor = rtxtBox.ForeColor;

        rtxtBox.ScrollToCaret();
      });
    }

    public static Color GetColor(this MessageType type)
    {
      switch (type)
      {
        case MessageType.ERROR:
          return Color.Red;
        case MessageType.TITLE:
          return Color.Blue;

        case MessageType.MESSAGE:
        default:
          return Color.Black;
      }
    }

    public static void InvokeControl(this Control control, Action func)
    {
      if (control.InvokeRequired)
      {
        control.Invoke(new MethodInvoker(() => func()));
      }
      else
        func.Invoke();
    }
  }

  public enum MessageType
  {
    ERROR,
    MESSAGE,
    TITLE
  }

  public class LevelDirectory
  {
    private readonly string[] _pathArrs;
    private readonly string _path;
    private readonly string _rootPath;
    private int _curentLevel = 0;
    private readonly string _separator;
    private readonly bool _isValidPath;
    private const int DEEP_LEVEL_MAX = 100;

    public LevelDirectory(string path)
    {
      _path = path;
      _separator = Path.DirectorySeparatorChar.ToString();

      IsUncPath = _path.StartsWith(_separator + _separator);
      _curentLevel = IsUncPath ? 1 : 0;

      _rootPath = Path.GetPathRoot(_path);
      if (!string.IsNullOrWhiteSpace(_rootPath))
      {
        _pathArrs = _path.Substring(_rootPath.Length).Split(Path.DirectorySeparatorChar);
        LevelMax = _pathArrs.Length;

        var invalidChars = Path.GetInvalidFileNameChars();
        _isValidPath = _pathArrs.All(t => t.IndexOfAny(invalidChars) == -1);

      }
      else
      {
        _isValidPath = false;
      }
    }

    public bool IsUncPath { get; set; }

    public int LevelMax { get; set; }

    public bool CheckAnyAccess()
    {
      if (!_isValidPath)
        return false;

      var count = 0;
      string rootPath;
      while (true && count < DEEP_LEVEL_MAX)
      {
        rootPath = GetNexPath();
        if (rootPath == null)
          return false;

        var hasAccess = CheckAccess(rootPath);
        if (hasAccess)
          return true;

        count++;
      }
      return false;
    }

    private bool CheckAccess(string path)
    {
      try
      {
        var access = Directory.Exists(_rootPath);
        return access;
      }
      catch (Exception)
      {
        return false;
      }
    }

    private string GetPath(int level)
    {
      if (level > LevelMax)
        return null;

      _curentLevel = level + 1;

      var res = _rootPath + string.Join(_separator, _pathArrs.Take(level));
      return res;
    }

    private string GetNexPath()
    {
      return GetPath(_curentLevel);
    }
  }
}
