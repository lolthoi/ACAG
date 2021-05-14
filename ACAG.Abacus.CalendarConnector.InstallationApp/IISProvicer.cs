using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;
using System.Globalization;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public enum ActionIISEnum
  {
    UPDATE = 0,
    CREATE = 1
  }

  public class IISProvicer
  {
    public const string APP_DLL_NAME = "ACAG.Abacus.CalendarConnector.Server.dll";

    private static List<SiteInfo> _sites = null;

    public IISProvicer(AppSettings appSettings)
    {
      this._appSettings = appSettings;
    }

    #region Variables

    public event StateChange OnStateChange;

    private string _poolName, _webSiteName, _hostName, _phyPath, _sourceFileDataPath;
    private int _port;
    private const string folderApp = "data\\IIS";
    private ServerManager iisManager = new ServerManager();
    private AppSettings _appSettings;
    private bool resultInstall = false;

    #endregion Variables
    public bool Install()
    {
      InitConfiguration();

      var action = GetAction(_webSiteName);
      switch (action)
      {
        case ActionIISEnum.UPDATE:
          {
            OnStateChange?.Invoke(MessageType.MESSAGE, "Begin updating the web application...");
            resultInstall = HandleUpdateIIS();
            break;
          }
        case ActionIISEnum.CREATE:
          {
            OnStateChange?.Invoke(MessageType.MESSAGE, "Begin creating a new web application...");
            resultInstall = HandleCreateIIS();
            break;
          }
      }

      if (resultInstall)
      {
        OnStateChange?.Invoke(MessageType.MESSAGE, "Done.");
        return true;
      }
      else
      {
        return false;
      }
    }

    public static List<SiteInfo> GetAllSites()
    {
      if (_sites != null)
        return _sites;

      try
      {
        using (var iisManager = new ServerManager())
        {
          _sites = iisManager.Sites
            .Select(t =>
            {
              try
              {
                var res = new SiteInfo
                {
                  SiteName = t.Name,
                  Type = t.Bindings[0].Protocol,
                  HostName = t.Bindings[0].Host,
                  Port = t.Bindings[0].EndPoint.Port.ToString(),
                  PhysicalPath = t.Applications["/"].VirtualDirectories["/"].PhysicalPath,
                  State = t.State
                };
                return res;
              }
              catch (Exception) { return null; }
            })
            .Where(t => t != null)
            .OrderBy(t => t.SiteName)
            .ToList();
        }
        return _sites;
      }
      catch
      {
        return null;
      }
    }

    public static List<SiteInfo> GetAbacusSites()
    {
      var sites = Global.Sites;
      if (sites == null)
        return null;

      var res = sites.Where(site =>
        {
          var path = Path.Combine(site.PhysicalPath + "\\server\\", APP_DLL_NAME);
          return File.Exists(path);
        })
        .ToList();

      return res;
    }


    public static bool IsIISInstalled(out string version)
    {
      var IISRegKeyName = "Software\\Microsoft\\InetStp";
      var IISRegKeyValue = "MajorVersion";

      return GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out version);
    }

    public static bool IsNetCoreInstalled(out string version)
    {
      version = string.Empty;

      using (var regs = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
      {
        var obj = regs.OpenSubKey(@"SOFTWARE\Microsoft\ASP.NET Core\Shared Framework");
        if (obj == null)
        {
          return false;
        }
        var subNames = obj.GetSubKeyNames();
        if (subNames.Length == 0)
          return false;
        var lastVersion = subNames.OrderBy(t => t).Last();
        obj = obj.OpenSubKey(lastVersion);
        if (obj == null)
        {
          return false;
        }

        subNames = obj.GetSubKeyNames();
        if (subNames.Length == 0)
          return false;

        lastVersion = subNames.OrderBy(t => t).Last();
        obj = obj.OpenSubKey(lastVersion);
        if (obj == null)
        {
          return false;
        }

        var value = obj.GetValue("Version");
        if (value == null)
        {
          return false;
        }
        version = value.ToString();
        return true;
      }
    }


    #region HELPER METHOD

    private static bool GetRegistryValue<T>(RegistryHive hive, string key, string value, RegistryValueKind kind, out T data)
    {
      bool success = false;
      data = default(T);

      using (RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(hive, String.Empty))
      {
        if (baseKey != null)
        {
          using (RegistryKey registryKey = baseKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree))
          {
            if (registryKey != null)
            {
              try
              {
                RegistryValueKind kindFound = registryKey.GetValueKind(value);
                if (kindFound == kind)
                {
                  object regValue = registryKey.GetValue(value, null);
                  if (regValue != null)
                  {
                    data = (T)Convert.ChangeType(regValue, typeof(T), CultureInfo.InvariantCulture);
                    success = true;
                  }
                }
              }
              catch (IOException)
              {

              }
            }
          }
        }
      }
      return success;
    }

    /// <summary>
    /// Check IIS sites name exist, If yes return enum update, else no return create. 
    /// Result decide action of IISProvicer
    /// </summary>
    /// <param name="siteName">Site name</param>
    /// <returns></returns>
    private ActionIISEnum GetAction(string siteName)
    {
      var sites = Global.Sites;
      siteName = siteName.ToLower();
      foreach (var site in sites)
      {
        if (site.SiteName.ToLower() == siteName)
        {
          return ActionIISEnum.UPDATE;
        }
      }
      return ActionIISEnum.CREATE;
    }

    private bool HandleUpdateIIS()
    {
      try
      {
        var pool = iisManager.ApplicationPools.FirstOrDefault(t => t.Name == _webSiteName);
        var site = iisManager.Sites.FirstOrDefault(s => s.Name == _webSiteName);
        if (site != null && pool != null)
        {
          //_phyPath = GetSitePhysicPath(site);
          OnStateChange?.Invoke(MessageType.MESSAGE, String.Format("Stopping site: {0}", _webSiteName));
          if (site.State != ObjectState.Stopped && site.State != ObjectState.Stopping)
          {
            site.Stop();
          }

          if (pool.State != ObjectState.Stopped && pool.State != ObjectState.Stopping)
          {
            pool.Stop();
          }

          if (site.State == ObjectState.Stopped)
          {
            OnStateChange?.Invoke(MessageType.MESSAGE, "Write source code to folder path"); 
            if (!WriteSourceCodeToFolder(_phyPath, _sourceFileDataPath))
            {
              OnStateChange?.Invoke(MessageType.ERROR, String.Format("Error occurs when copy source web app to folder: {0}", _webSiteName));
              return false;
            }

            Utils.TrySetPermission(_phyPath);

            #region

            if (site.Bindings.Count > 0)
            {
              var bindings = site.Bindings[0];
              bindings.Protocol = _appSettings.IISConfig.Type;
              bindings.BindingInformation = string.Format("*:{0}:{1}", _port, _hostName);
            }

            site.Applications[0].VirtualDirectories["/"].PhysicalPath = _phyPath;
            #endregion

            #region Try convert folder to application
            try
            {
              Site defaultSite = iisManager.Sites[_webSiteName];
              defaultSite.Applications.Add("/client", Path.Combine(_phyPath, "client"));
              defaultSite.Applications.Add("/server", Path.Combine(_phyPath, "server"));
              defaultSite.Applications.Add("/abacus", Path.Combine(_phyPath, "abacus"));
            }
            catch (Exception){}

            #endregion

            iisManager.CommitChanges();
          }
          else
          {
            OnStateChange?.Invoke(MessageType.ERROR, "Error in stopping the web application (IIS)");
            return false;
          }
        }
        OnStateChange?.Invoke(MessageType.MESSAGE, String.Format("Start site: {0}", _webSiteName));
        site.Start();
        pool.Start();
        return true;

      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "Error in updating the web application." + Environment.NewLine + ex.Message);
        return false;
      }
    }

    private bool HandleCreateIIS()
    {
      try
      {
        #region Verify
        if (CheckIsExistApplicationPool(_poolName))
        {
          OnStateChange?.Invoke(MessageType.ERROR, String.Format("The poolname already exists: {0}", _poolName));
          return false;
        }
        if (CheckIsExistWebsiteName(_webSiteName))
        {
          OnStateChange?.Invoke(MessageType.ERROR, String.Format("The site name already exists: {0}", _webSiteName));
          return false;
        }
        #endregion Verify

        if (!WriteSourceCodeToFolder(_phyPath, _sourceFileDataPath))
        {
          OnStateChange?.Invoke(MessageType.ERROR, String.Format("Error occurs when copy source web app to folder: {0}", _webSiteName));
          return false;
        }
        Utils.TrySetPermission(_phyPath);

        if (!CreateAppPoolAndIISWebsite(ManagedPipelineMode.Integrated))
        {
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "Error in creating a web application." + Environment.NewLine + ex.Message);
        return false;
      }
    }

    private bool CreateAppPoolAndIISWebsite(ManagedPipelineMode mode, string runtimeVersion = "v4.0")
    {
      try
      {
        iisManager = new ServerManager();

        #region Create Pool

        ApplicationPool newPool = iisManager.ApplicationPools.Add(_poolName);
        newPool.ManagedRuntimeVersion = runtimeVersion;
        newPool.Enable32BitAppOnWin64 = true;
        newPool.ManagedPipelineMode = mode;

        #endregion Create Pool

        #region Create IIS

        iisManager.Sites.Add(_webSiteName, _appSettings.IISConfig.Type, string.Format("*:{0}:{1}", _port, _hostName), _phyPath);
        iisManager.Sites[_webSiteName].ApplicationDefaults.ApplicationPoolName = _poolName;
        iisManager.Sites[_webSiteName].ServerAutoStart = true;

        foreach (var item in iisManager.Sites[_webSiteName].Applications)
        {
          item.ApplicationPoolName = _poolName;
        }
        
        Site defaultSite = iisManager.Sites[_webSiteName];
        defaultSite.Applications.Add("/client", Path.Combine(_phyPath,"client"));
        defaultSite.Applications.Add("/server", Path.Combine(_phyPath,"server"));
        defaultSite.Applications.Add("/abacus", Path.Combine(_phyPath, "abacus"));

        #endregion Create IIS

        iisManager.CommitChanges();
        return true;
      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "IMPLEMENT CREATE APPPOLE FAIL..." + Environment.NewLine + ex.Message);
        return false;
      }
    }
    private string GetSitePhysicPath(Site siteModel)
    {
      var site = iisManager.Sites.Where(s => s.Name == siteModel.Name).Single();
      var applicationRoot = site.Applications.Where(a => a.Path == "/").Single();
      var virtualRoot = applicationRoot.VirtualDirectories.Where(v => v.Path == "/").Single();

      return virtualRoot.PhysicalPath;
    }

    private void DeleteAllFilesInPath(string pathToDelete)
    {
      DirectoryInfo di = new DirectoryInfo(pathToDelete);

      foreach (FileInfo file in di.GetFiles())
      {
        file.Delete();
      }
      foreach (DirectoryInfo dir in di.GetDirectories())
      {
        dir.Delete(true);
      }
    }

    private void InitConfiguration()
    {
      _poolName = _appSettings.IISConfig.WebSiteName;
      _webSiteName = _appSettings.IISConfig.WebSiteName;
      _hostName = _appSettings.IISConfig.HostName;
      _port = _appSettings.IISConfig.Port;
      _phyPath = _appSettings.IISConfig.PhysicalPath;
      _sourceFileDataPath = Path.Combine(Utils.CurrentFolder, folderApp);
    }

    public bool CheckIsExistApplicationPool(string applicationPoolName)
    {
      bool isExists = false;

      ServerManager serverManager = new ServerManager();
      ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;
      isExists = applicationPoolCollection.Any(x => x.Name == applicationPoolName);
      return isExists;
    }

    public bool CheckIsExistWebsiteName(string applicationPoolName)
    {
      bool isExists = false;

      ServerManager serverManager = new ServerManager();
      ApplicationPoolCollection applicationPoolCollection = serverManager.ApplicationPools;
      isExists = applicationPoolCollection.Any(x => x.Name == applicationPoolName);
      return isExists;
    }

    /// <summary>
    /// Copy all files from sourcePath to folderPath
    /// </summary>
    /// <param name="folderPath">Destination path</param>
    /// <param name="sourcePath">Source path</param>
    private bool WriteSourceCodeToFolder(string folderPath, string sourcePath)
    {
      try
      {
        if (!Directory.Exists(folderPath))
        {
          Directory.CreateDirectory(folderPath);
        }

        if (!Directory.Exists(sourcePath))
        {
          Directory.CreateDirectory(sourcePath);
        }

        foreach (var file in Directory.GetFiles(sourcePath))
        {
          var newFilePath = Path.Combine(folderPath, Path.GetFileName(file));
          var sourceFilePath = Path.Combine(sourcePath, Path.GetFileName(file));

          File.Create(newFilePath).Close();
          File.WriteAllBytes(newFilePath, File.ReadAllBytes(sourceFilePath));
          //SetAccessRights(sourceFilePath);
        }
        foreach (var directory in Directory.GetDirectories(sourcePath))
        {
          WriteSourceCodeToFolder(Path.Combine(folderPath, Path.GetFileName(directory)), directory);
          //SetAccessRights(Path.Combine(folderPath, Path.GetFileName(directory)));
        }

        return true;
      }
      catch (Exception ex)
      {
        OnStateChange?.Invoke(MessageType.ERROR, "Error in coping files..." + Environment.NewLine + ex.Message);
        return false;
      }
    }

    private void SetAccessRights(string path)
    {
      try
      {
        FileSecurity fileSecurity = new FileInfo(path).GetAccessControl();
        AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
        foreach (FileSystemAccessRule rule in rules)
        {
          FileSecurity newFileSecurity = new FileInfo(path).GetAccessControl();
          FileSystemAccessRule newRule = new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, AccessControlType.Allow);
          newFileSecurity.AddAccessRule(newRule);

          FileInfo file = new FileInfo(path);
          file.SetAccessControl(newFileSecurity);
        }
      }
      catch (Exception)
      {
        return;
      }
      
    }

    #endregion HELPER METHOD
  }
}
