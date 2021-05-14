using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ACAG.Abacus.CalendarConnector.Logic
{
  public class MailSettings
  {
    public string MailSelector { get; set; }
    public SMTP Smtp { get; set; }
    public SendGrid SendGrid { get; set; }
  }
  public class SMTP
  {
    public string SenderName { get; set; }
    public string Mail { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string DisableSecureSocket { get; set; }
    public bool DisableXOATH2 { get; set; }
    public bool DisableCertificate { get; set; }
    public bool UseDefaultCredentials { get; set; }

  }

  public class SendGrid
  {
    public string SenderName { get; set; }
    public string Account { get; set; }
    public string APIURL { get; set; }
    public string Key { get; set; }

  }

  public class MailRequest
  {
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<IFormFile> Attachments { get; set; }
  }
}
