using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface IMailService
  {
    Task SendEmailAsync(MailRequest mailRequest);
  }

  public class MailService : IMailService
  {
    private readonly MailSettings _mailSettings;
    public MailService(IOptions<MailSettings> mailSettings)
    {
      _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(MailRequest mailRequest)
    {
      switch (_mailSettings.MailSelector)
      {
        case "Smtp":
          await SendMailSMTP(mailRequest);
          break;
        case "SendGrid":
          await SendMailGrid(mailRequest);
          break;
        default:
          break;
      }
    }
    
    private async Task SendMailSMTP(MailRequest mailRequest)
    {
      if (_mailSettings.Smtp.DisableCertificate)
        NEVER_EAT_POISON_Disable_CertificateValidation();

      var email = new MimeMessage();
      email.Sender = MailboxAddress.Parse(_mailSettings.Smtp.Mail);
      email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
      string agency = Regex.Replace(_mailSettings.Smtp.SenderName, @"[^a-zA-Z]+", "");
      email.Subject = string.IsNullOrEmpty(agency) ? mailRequest.Subject : string.Format("{0}: {1}", agency, mailRequest.Subject);
      var builder = new BodyBuilder();
      if (mailRequest.Attachments != null)
      {
        byte[] fileBytes;
        foreach (var file in mailRequest.Attachments)
        {
          if (file.Length > 0)
          {
            using (var ms = new MemoryStream())
            {
              file.CopyTo(ms);
              fileBytes = ms.ToArray();
            }
            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
          }
        }
      }
      builder.HtmlBody = mailRequest.Body;
      email.Body = builder.ToMessageBody();
      using var smtp = new SmtpClient();
      smtp.Connect(_mailSettings.Smtp.Host, _mailSettings.Smtp.Port, ConvertToSecureSocketOptions(_mailSettings.Smtp.DisableSecureSocket));

      if (_mailSettings.Smtp.DisableXOATH2)
      {
        smtp.AuthenticationMechanisms.Remove("XOAUTH2");
      }

      if (!_mailSettings.Smtp.UseDefaultCredentials)
      {
        smtp.Authenticate(_mailSettings.Smtp.Mail, _mailSettings.Smtp.Password);
      }

      await smtp.SendAsync(email);
      smtp.Disconnect(true);
    }

    private async Task SendMailGrid(MailRequest mailRequest)
    {
      var emailMessage = new SendGridMessage
      {
        From = new EmailAddress(_mailSettings.SendGrid.Account, _mailSettings.SendGrid.SenderName),
        Subject = string.IsNullOrWhiteSpace(mailRequest.Subject) ? string.Empty : mailRequest.Subject
      };

      emailMessage.HtmlContent = mailRequest.Body;
      emailMessage.AddTo(mailRequest.ToEmail);

      var sendGridClient = new SendGridClient(_mailSettings.SendGrid.Key);

      try
      {
        var response = sendGridClient.SendEmailAsync(emailMessage).Result;

        switch (response.StatusCode)
        {
          case System.Net.HttpStatusCode.Accepted:
            break;
          case System.Net.HttpStatusCode.BadRequest:
            break;
          default:
            break;
        }
      }
      catch (System.Exception ex)
      {

      }
    }

    private SecureSocketOptions ConvertToSecureSocketOptions(string option)
    {
      switch (option)
      {
        case "None":
          return SecureSocketOptions.None;

        case "Auto":
          return SecureSocketOptions.Auto;

        case "SslOnConnect":
          return SecureSocketOptions.SslOnConnect;

        case "StartTls":
          return SecureSocketOptions.StartTls;

        case "StartTlsWhenAvailable":
          return SecureSocketOptions.StartTlsWhenAvailable;

        default:
          return SecureSocketOptions.Auto;
      }
    }

    public void NEVER_EAT_POISON_Disable_CertificateValidation()
    {
      ServicePointManager.ServerCertificateValidationCallback =
          delegate (
              object s,
              System.Security.Cryptography.X509Certificates.X509Certificate certificate,
              System.Security.Cryptography.X509Certificates.X509Chain chain,
              System.Net.Security.SslPolicyErrors sslPolicyErrors
          )
          {
            return true;
          };
    }
  }
}
