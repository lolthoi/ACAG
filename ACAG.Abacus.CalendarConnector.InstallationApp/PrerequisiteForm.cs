using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.Administration;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public partial class PrerequisiteForm : Form
  {

    private CommandExecuter _command;
    public PrerequisiteForm()
    {
      InitializeComponent();
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
      Hide();
      var site = GetSiteByName(cbxSites.Text);
      if (site == null)
      {
        site = new SiteInfo
        {
          SiteName = cbxSites.Text
        };
      }
      var installForm = new SetupForm(site);
      installForm.Owner = this;
      installForm.ShowDialog(this);
      Show();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private bool IsInstallUrlRewrite(out string version)
    {
      try
      {
        var path = Path.Combine(Environment.SystemDirectory, @"inetsrv\rewrite.dll");
        var infor = FileVersionInfo.GetVersionInfo(path);
        version = infor.ProductVersion;

        var arrs = infor.ProductVersion.Split('.');
        if (int.Parse(arrs[0]) >= 7 && int.Parse(arrs[1]) >= 1 && int.Parse(arrs[2]) >= 761)
          return true;
        return false;
      }
      catch
      {
        version = "Unknown";
        return false;
      }
    }

    private void InstallUrlRewrite(CommandExecuter command)
    {
      var installed = IsInstallUrlRewrite(out string version);
      if (installed)
      {
        return;
      }

      string urlrewrite2 = "urlrewrite2.exe";
      if (File.Exists(urlrewrite2))
      {
        command.Execute(installed, true, false, new CommandInfo(true, urlrewrite2, "", "WebPlatformInstaller", ""), null);
        return;
      }
      using (var file = new OpenFileDialog()
      {
        Filter = "urlrewrite2 setup file | urlrewrite2.exe",
        Title = "Choose a urlrewrite2 setup file",
        AutoUpgradeEnabled = false
      })
      {
        this.InvokeControl(() =>
        {
          var ok = file.ShowDialog(this) == DialogResult.OK;
          if (ok)
          {
            urlrewrite2 = file.FileName;
            command.Execute(installed, true, false, new CommandInfo(true, urlrewrite2, "", "WebPlatformInstaller", ""), null);
          }
        });
      }
    }

    private void OpenUrl(string uri)
    {

      var psi = new ProcessStartInfo();
      psi.UseShellExecute = true;
      psi.FileName = uri;
      Process.Start(psi);

    }
    private void InstallNetCoreHosting(CommandExecuter command)
    {
      var netHostingInstalled = IISProvicer.IsNetCoreInstalled(out string netCoreVersion);
      if (netHostingInstalled)
        return;

      string dotnetHosting = "dotnet-hosting-3.1.12-win.exe";
      if (File.Exists(dotnetHosting))
      {
        command.Execute(netHostingInstalled, true, false, new CommandInfo(true, dotnetHosting, "", "dotnet-hosting-", ""), null);
        return;
      }
      this.InvokeControl(() =>
      {
        var messageBox = MessageBox.Show(this, "Open site to download dotnet-hosting setup file?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (messageBox == DialogResult.Yes)
        {
          OpenUrl("https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-5.0.4-windows-hosting-bundle-installer");
        }

        using (var file = new OpenFileDialog()
        {
          Filter = "dotnet-hosting setup file | dotnet-hosting-*.exe",
          Title = "Choose a dotnet-hosting setup file",
          AutoUpgradeEnabled = false
        })
        {
          var ok = file.ShowDialog(this) == DialogResult.OK;
          if (ok)
          {
            dotnetHosting = file.FileName;
            command.Execute(netHostingInstalled, true, false, new CommandInfo(true, dotnetHosting, "", "dotnet-hosting-", ""), null);
          }
        }
      });
    }


    private void PrerequisiteForm_Load(object sender, EventArgs e)
    {

      _command = new CommandExecuter();
      _command.Notify += (message, isError) =>
      {
        this.InvokeControl(() =>
        {
          MessageBox.Show(this,
              message,
              "Error",
              MessageBoxButtons.OK,
              MessageBoxIcon.Error);
        });
      };

      LoadInfo();
    }

    private void BindInfoToIIS(SetupInfo.Detail infor, int processValue)
    {
      var iisInstalled = IISProvicer.IsIISInstalled(out string iisVersion);
      infor.Status = iisInstalled ? 1 : 0;
      infor.Version = new SetupInfo.VersionValue(iisVersion);

      this.InvokeControl(() =>
      {
        lblIisStatus.Text = iisInstalled ? "Installed" : "Not installed";
        lblIisStatus.ForeColor = iisInstalled ? Color.Blue : Color.Red;
        lblIisVersion.Text = iisVersion;

        progressBar1.Value += processValue;
      });
    }

    private void BindDotNetCoreInfo(SetupInfo.Detail infor, int processValue)
    {

      var isNetCoreInstalled = IISProvicer.IsNetCoreInstalled(out string netCoreVersion);
      infor.Status = isNetCoreInstalled ? 1 : 0;
      infor.Version = new SetupInfo.VersionValue(netCoreVersion);

      this.InvokeControl(() =>
      {
        lblNetCoreStatus.Text = isNetCoreInstalled ? "Installed" : "Not installed";
        lblNetCoreStatus.ForeColor = isNetCoreInstalled ? Color.Blue : Color.Red;

        btnInstallNetCore.Enabled = !isNetCoreInstalled;

        lblNetCoreVersion.Text = netCoreVersion;

        progressBar1.Value += processValue;
      });
    }

    private void BindUrlRewrite(SetupInfo.Detail infor, int processValue)
    {
      var isUrlRewriteInstalled = IsInstallUrlRewrite(out string urlRewriteVersion);
      infor.Status = isUrlRewriteInstalled ? 1 : 0;
      infor.Version = new SetupInfo.VersionValue(urlRewriteVersion);

      this.InvokeControl(() =>
      {
        lblUrlRewriteStatus.Text = isUrlRewriteInstalled ? "Installed" : "Not installed";
        lblUrlRewriteStatus.ForeColor = isUrlRewriteInstalled ? Color.Blue : Color.Red;

        btnInstallUrlRewrite.Enabled = !isUrlRewriteInstalled;

        lblUrlRewriteVersion.Text = urlRewriteVersion;

        progressBar1.Value += processValue;
      });
    }

    private void BindInfoToSqlServer(SetupInfo.Detail infor, int processValue)
    {
      var sqlServerInstalled = SQLProvider.IsSqlServerInstalled();
      var sqlVersion = sqlServerInstalled ? SQLProvider.GetSqlServerVersion() : "Not installed";
      infor.Status = sqlServerInstalled ? 1 : 0;
      infor.Version = new SetupInfo.VersionValue(sqlVersion == null ? "Unknown" : sqlVersion);

      this.InvokeControl(() =>
      {
        lblSqlStatus.Text = sqlServerInstalled ? "Installed" : "Not installed";
        lblSqlStatus.ForeColor = sqlServerInstalled ? Color.Blue : Color.Red;
        lblSqlVersion.Text = sqlVersion;
        lblSqlVersion.ForeColor = sqlServerInstalled && sqlVersion != null ? Color.Black : Color.Red;

        progressBar1.Value += processValue;
      });
    }

    private void BindSiteControl(int processValue)
    {
      var abacusSites = Global.AbacusSites;

      this.InvokeControl(() =>
      {
        cbxSites.DisplayMember = "SiteName";
        cbxSites.DataSource = abacusSites;

        if (abacusSites != null && abacusSites.Count > 0)
        {
          cbxSites.SelectedItem = abacusSites.First();
        }
        progressBar1.Value += processValue;
      });
    }

    private void BindInfoToCurrentApp(SiteInfo site)
    {
      if (site != null)
      {
        var dllPath = Path.Combine(site.PhysicalPath + "\\server", IISProvicer.APP_DLL_NAME);
        var version = Utils.GetAppVersion(dllPath);
        site.AppVersion = new SetupInfo.VersionValue(version);

        var appSettingsPath = Path.Combine(site.PhysicalPath + "\\server", "appsettings.json");
        site.MailSettings = Utils.ReadEmailSetting(appSettingsPath);
      }

      this.InvokeControl(() =>
      {
        if (site == null)
        {
          lblSiteStatus.Text = "(New)";
          lblSiteAppVersion.Text = "Not installed";

          lblSiteStatus.ForeColor = Color.Red;
          lblSiteAppVersion.ForeColor = Color.Black;
        }
        else
        {
          var isInstalled = site.State.HasValue;

          lblSiteStatus.Text = isInstalled ? site.State.Value.ToString() : "Not installed";
          lblSiteAppVersion.Text = site.AppVersion.Data == null ? "Unknown" : site.AppVersion.Data;

          lblSiteStatus.ForeColor = isInstalled && (site.State == ObjectState.Started || site.State == ObjectState.Starting)
            ? Color.Blue
            : Color.Red;
          lblSiteAppVersion.ForeColor = site.AppVersion.Data == null ? Color.Red : Color.Black;
        }
        if (progressBar1.Value == 0)
        {
          progressBar1.Value = 40;
        }
      });
    }

    private void BindInfoToCurrentDatabase(SiteInfo site)
    {
      if (site != null && site.DbInfo == null)
      {
        this.InvokeControl(() =>
        {
          lblDbName.Text = "...";
          lblDbStatus.Text = "...";
          lblDbVersion.Text = "...";

          lblDbName.ForeColor = Color.Black;
          lblDbStatus.ForeColor = Color.Black;
          lblDbVersion.ForeColor = Color.Black;
        });

        var appSettings = Path.Combine(site.PhysicalPath + "\\server", "appsettings.json");
        var connectionString = Utils.ReadConnectionString(appSettings);

        var dbVersion = Utils.GetCurrentDbVersion(connectionString, out string dbName, out bool? canConnected);

        site.IsDbConnected = canConnected;
        site.DbInfo = new SetupInfo.Detail
        {
          Name = dbName,
          Version = new SetupInfo.VersionValue(dbVersion),
          Tag = connectionString
        };
      }

      this.InvokeControl(() =>
      {
        var siteName = cbxSites.Text;

        if (site == null)
        {
          lblDbName.Text = "Not installed";
          lblDbStatus.Text = "Not installed";
          lblDbVersion.Text = "Not installed";

          lblDbName.ForeColor = Color.Black;
          lblDbStatus.ForeColor = Color.Black;
          lblDbVersion.ForeColor = Color.Black;
        }
        else if (siteName.ToLower().Trim() == site.SiteName.ToLower().Trim())
        {
          lblDbName.Text = site.DbInfo.Name ?? "Unknown";
          lblDbStatus.Text = site.IsDbConnected == null ? "Unknown" : (site.IsDbConnected.Value ? "Successful" : "Failed");
          lblDbVersion.Text = site.DbInfo.Version.Data == null ? "Unknown" : site.DbInfo.Version.Data;

          lblDbName.ForeColor = site.DbInfo.Name == null ? Color.Red : Color.Black;
          lblDbStatus.ForeColor = site.IsDbConnected == true ? Color.Blue : Color.Red;
          lblDbVersion.ForeColor = site.DbInfo.Version.Data == null ? Color.Red : Color.Black;
        }

        if (progressBar1.Value != 100)
        {
          progressBar1.Value = 100;
        }
      });
    }

    private void BindPackageControl(SetupInfo.PackageVersion info, int processValue)
    {
      var packageAppVersion = Utils.GetPackageAppVersion();
      var packageDbVersion = Utils.GetPackageDbVersion();

      info.AppVersion = new SetupInfo.VersionValue(packageAppVersion);
      info.DatabaseVersion = new SetupInfo.VersionValue(packageDbVersion);

      this.InvokeControl(() =>
      {
        lblPackageAppVersion.Text = packageAppVersion == null ? "Unknown" : packageAppVersion;
        lblPackageDbVersion.Text = packageDbVersion == null ? "Unknown" : packageDbVersion;

        lblPackageAppVersion.ForeColor = packageAppVersion == null ? Color.Red : Color.Black;
        lblPackageDbVersion.ForeColor = packageDbVersion == null ? Color.Red : Color.Black;

        progressBar1.Value += processValue;
      });

    }

    private void cbxSites_TextChanged(object sender, EventArgs e)
    {
      btnNext.Enabled = false;
      var siteName = cbxSites.Text;
      if (siteName == string.Empty)
        return;

      Task.Factory.StartNew(() =>
      {
        var site = GetSiteByName(siteName);
        if (site != null)
        {
          this.InvokeControl(() =>
          {
            if (progressBar1.Value != 0)
            {
              progressBar1.Value = 0;
            }
          });
        }

        BindInfoToCurrentApp(site);
        BindInfoToCurrentDatabase(site);

        this.InvokeControl(() =>
        {
          siteName = cbxSites.Text;
          if (site == null || siteName.ToLower().Trim() == site.SiteName.ToLower().Trim())
          {
            var isOkay = Global.Infor.CanbeUprade(site);
            btnNext.Enabled = isOkay;
          }
        });
      });
    }

    private SiteInfo GetSiteByName(string siteName)
    {
      var sites = Global.Sites;
      var site = sites.Find(t => t.SiteName.Trim().ToLower() == siteName.Trim().ToLower());
      return site;
    }

    private void btnInstallNetCore_Click(object sender, EventArgs e)
    {
      InstallNetCoreHosting(_command);

      Task.Factory.StartNew(() =>
      {
        BindDotNetCoreInfo(Global.Infor.NetCore, 100);
      });
    }

    private void btnInstallUrlRewrite_Click(object sender, EventArgs e)
    {
      InstallUrlRewrite(_command);

      Task.Factory.StartNew(() =>
      {
        BindUrlRewrite(Global.Infor.UrlRewrite, 100);
      });
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
      LoadInfo();
    }

    private void LoadInfo()
    {
      progressBar1.Value = 0;

      Task.Factory.StartNew(() =>
      {
        var executeFilePath = System.Windows.Forms.Application.ExecutablePath;
        var executableFileInfo = new FileInfo(executeFilePath);
        Utils.CurrentFolder = executableFileInfo.DirectoryName;

        BindInfoToIIS(Global.Infor.IIS, 10);

        BindDotNetCoreInfo(Global.Infor.NetCore, 10);

        BindUrlRewrite(Global.Infor.UrlRewrite, 10);

        BindInfoToSqlServer(Global.Infor.SqlServer, 20);

        BindPackageControl(Global.Infor.Package, 20);

        BindSiteControl(30);

      });
    }
  }

  public static class Global
  {
    public static List<SiteInfo> _sites;
    public static List<SiteInfo> Sites
    {
      get
      {
        if (_sites == null)
        {
          _sites = IISProvicer.GetAllSites();
        }
        return _sites;
      }
    }

    public static List<SiteInfo> _abacusSites;
    public static List<SiteInfo> AbacusSites
    {
      get
      {
        if (_abacusSites == null)
        {
          _abacusSites = IISProvicer.GetAbacusSites();
        }
        return _abacusSites;
      }
    }

    public static SetupInfo Infor { get; private set; } = new SetupInfo();

  }
  public class SetupInfo
  {
    public SetupInfo()
    {
      IIS = new Detail();
      NetCore = new Detail();
      UrlRewrite = new Detail();
      SqlServer = new Detail();
      Package = new PackageVersion();
    }

    public Detail IIS { get; set; }

    public Detail NetCore { get; set; }

    public Detail UrlRewrite { get; set; }

    public Detail SqlServer { get; set; }

    public PackageVersion Package { get; set; }

    public bool CanbeUprade(SiteInfo site)
    {
      var res = IIS.Status == 1
        && NetCore.Status == 1
        && UrlRewrite.Status == 1
      && (site == null
      || (site.AppVersion == null && site.DbInfo == null)
      || (site.AppVersion.Data != null && (Package.AppVersion.Value > site.AppVersion.Value || Package.DatabaseVersion.Value > site.DbInfo.Version.Value)));

      return res;
    }

    public class PackageVersion
    {
      public VersionValue AppVersion { get; set; }
      public VersionValue DatabaseVersion { get; set; }
    }

    public class Detail
    {
      public string Name { get; set; }
      public int Status { get; set; }

      public VersionValue Version { get; set; }

      public object Tag { get; set; }
    }

    public class VersionValue
    {
      public VersionValue(string version)
      {
        Data = version;
      }

      public string Data { get; private set; }

      private int? _value;
      public int Value
      {
        get
        {
          if (_value == null)
          {
            _value = Data == null
              ? 0
              : ConvertVersion(Data);
          }
          return _value.Value;
        }
      }

      private int ConvertVersion(string version)
      {
        return int.TryParse(version.Replace(".", ""), out int value) ? value : 0;
      }
    }
  }
}
