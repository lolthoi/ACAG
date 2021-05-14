using Microsoft.Web.Administration;

namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public partial class SiteInfo
  {
    public ObjectState? State { get; set; }

    public string SiteName { get; set; }

    public string PhysicalPath { get; set; }

    public string Type { get; set; }

    public string HostName { get; set; }

    public string Port { get; set; }
  }

  public partial class SiteInfo
  {
    public SetupInfo.VersionValue AppVersion { get; set; }

    public MailSettings MailSettings { get; set; }

    public SetupInfo.Detail DbInfo { get; set; }

    public bool? IsDbConnected { get; set; }
  }
}
