namespace ACAG.Abacus.CalendarConnector.InstallationApp
{
  public class AppSettings
  {
    public IISConfig IISConfig { get; set; }
    public SQLConfig SQLConfig { get; set; }

    public MailSettings EmailSettings { get; set; }
  }
  public class IISConfig
  {
    public string Type { get; set; }
    public string WebSiteName { get; set; }
    public string HostName { get; set; }
    public string PhysicalPath { get; set; }
    public int Port { get; set; }
  }

  public class SQLConfig
  {
    public string ConnectionString { get; set; }

    public string Catalog { get; set; }

    public string Username { get; set; }
    public string Password { get; set; }

    public string AdminUsername { get; set; }
    public string AdminPassword { get; set; }
  }

  public class MailSettings
  {
    public string EmailSelector { get; set; }
    public SmtpEmail Smtp { get; set; }
    public SendGridEmail SendGrid { get; set; }
  }

  public class SendGridEmail
  {
    public string SenderName { get; set; }
    public string Account { get; set; }
    public string Key { get; set; }
  }

  public class SmtpEmail
  {
    public string SenderName { get; set; }
    public string Mail { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
  }
}
