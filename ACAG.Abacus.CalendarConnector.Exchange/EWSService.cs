using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Identity.Client;

namespace ACAG.Abacus.CalendarConnector.Exchange
{
  public delegate void ExchangeDataInserted(AppointmentModel data);
  public class EWSService
  {
    public event ExchangeDataInserted OnExchangeDataInserted;
    public ExchangeSettingModel Setting { get; set; }

    public EWSService(ExchangeSettingModel setting)
    {
      Setting = setting;
    }

    private bool RedirectionUrlValidationCallback(string redirectionUrl)
    {
      bool result = false;

      var redirectionUri = new Uri(redirectionUrl);
 
      if (redirectionUri.Scheme == "https")
      {
        result = true;
      }

      return result;
    }

    private ExchangeService CreateExchangeService(string impersonate)
    {
      ExchangeService exchangeService;
      switch (Setting.ExchangeVersion)
      {
        case "Exchange2007_SP1":
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            break;
          }
        case "Exchange2010":
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2010);
            break;
          }
        case "Exchange2010_SP1":
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2010_SP1);
            break;
          }
        case "Exchange2010_SP2":
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2010_SP2);
            break;
          }
        case "Exchange2013":
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2013);
            break;
          }
        case "Exchange2013_SP1":
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
            break;
          }
        case "Office365":
          {
            exchangeService = new ExchangeService();
            break;
          }
        case "Exchange2015":
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2015);
            break;
          }
        case "Exchange2016":
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2016);
            break;
          }
        default:
          {
            exchangeService = new ExchangeService(ExchangeVersion.Exchange2016);
            break;
          }
      }
      switch (Setting.LoginType)
      {
        case 1:
          var cca = ConfidentialClientApplicationBuilder
              .Create(Setting.AzureClientId)
              .WithClientSecret(Setting.AzureClientSecret)
              .WithTenantId(Setting.AzureTenant)
              .Build();

          var ewsScopes = new string[] { "https://outlook.office365.com/.default" };

          var authResult = cca.AcquireTokenForClient(ewsScopes).ExecuteAsync().Result;
          exchangeService.UseDefaultCredentials = false;
          exchangeService.Credentials = new OAuthCredentials(authResult.AccessToken);

          break;
        case 2:
          {
            exchangeService.UseDefaultCredentials = false;
            exchangeService.Credentials = new NetworkCredential(Setting.ServiceUser, Setting.ServiceUserPassword);
            break;
          }
      }
      if (string.IsNullOrEmpty(Setting.ExchangeUrl))
      {
        exchangeService.AutodiscoverUrl(Setting.ServiceUser, RedirectionUrlValidationCallback);
      }
      else
      {
        exchangeService.Url = new Uri(Setting.ExchangeUrl);
      }

      if (!string.IsNullOrWhiteSpace(impersonate))
      {
        exchangeService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, impersonate);
        exchangeService.HttpHeaders.Add("X-AnchorMailbox", impersonate);
      }

      return exchangeService;
    }

    private Item GetAppointmentItem(string appointmentId, string impersonate)
    {
      if (string.IsNullOrEmpty(impersonate)) return null;

      var s = CreateExchangeService(impersonate);
      var folderInfo = Folder.Bind(s, WellKnownFolderName.Calendar).Result;

      if (folderInfo == null || folderInfo?.Id == null)
        throw new Exception("Can not get Exchange Folder information*");

      var filterProp = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, EWSConfig.EXTEND_SUBCRIPTION_ID, MapiPropertyType.String);
      var filter = new SearchFilter.IsEqualTo(filterProp, appointmentId);
      var res = s.FindItems(folderInfo.Id, filter, new ItemView(1)).Result;

      var item = res.FirstOrDefault();

      if (item == null) return null;

      var properties = new PropertySet(EWSConfig.SelectedProperties);
      var status = item.Load(properties).Result;

      if (status.OverallResult != ServiceResult.Success) return null;

      return item;
    }

    private AppointmentModel GetAbacusData(Item a)
    {
      if (a == null) return null;

      string userDataHex;
      a.TryGetProperty(new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, EWSConfig.EXTEND_SUBCRIPTION_VALUE, MapiPropertyType.String), out userDataHex);

      if (string.IsNullOrEmpty(userDataHex)) return null;

      return EWSHelper.StringToObject<AppointmentModel>(userDataHex);
    }

    public AppointmentModel GetAbacusData(AppointmentModel data)
    {
      var userId = EWSHelper.CreateItemId(data, Setting.TenantId);
      var item = GetAppointmentItem(userId, data.MailAccount);
      return GetAbacusData(item);
    }

    public void Insert(AppointmentModel data)
    {
      var s = CreateExchangeService(data.MailAccount);
      var a = new Appointment(s);

      var folderInfo = Folder.Bind(s, WellKnownFolderName.Calendar).Result;

      if (folderInfo == null || folderInfo?.Id == null)
        throw new Exception("Can not get Exchange Folder information*");

      var status = s.CreateItems(new List<Appointment>() { a }, folderInfo.Id, MessageDisposition.SaveOnly, SendInvitationsMode.SendToNone).Result;
      if (status.OverallResult == ServiceResult.Success)
      {
        var userId = EWSHelper.CreateItemId(data, Setting.TenantId);

        var properties = new PropertySet(EWSConfig.SelectedProperties);
        var item = Appointment.Bind(s, a.Id, properties).Result;

        item.Subject = data.Subject;
        item.Start = data.DateTimeStart;
        item.End = data.DateTimeEnd;
        item.Sensitivity = data.IsPrivate ? Sensitivity.Private : Sensitivity.Normal;
        item.LegacyFreeBusyStatus = data.IsOutOfOffice ? LegacyFreeBusyStatus.OOF : LegacyFreeBusyStatus.Free;

        data.ExchangeID = item.Id.UniqueId;
              
        item.SetExtendedProperty(new ExtendedPropertyDefinition(
            DefaultExtendedPropertySet.PublicStrings
            , EWSConfig.EXTEND_SUBCRIPTION_ID
            , MapiPropertyType.String)
            , userId);
        item.SetExtendedProperty(new ExtendedPropertyDefinition(
            DefaultExtendedPropertySet.PublicStrings
            , EWSConfig.EXTEND_SUBCRIPTION_VALUE
            , MapiPropertyType.String)
            , EWSHelper.ObjectToString(data));
        var x = item.Update(ConflictResolutionMode.AlwaysOverwrite, SendInvitationsOrCancellationsMode.SendToNone).Result;

        OnExchangeDataInserted?.Invoke(data);
      }
    }

    public void Update(AppointmentModel data)
    {
      if (data == null || data.AbacusID < 0 || string.IsNullOrEmpty(data.Subject))
      {
        return;
      }

      var userId = EWSHelper.CreateItemId(data, Setting.TenantId);

      var item = GetAppointmentItem(userId, data.MailAccount) as Appointment;

      item.Subject = data.Subject;
      item.Start = data.DateTimeStart;
      item.End = data.DateTimeEnd;
      item.Sensitivity = data.IsPrivate ? Sensitivity.Private : Sensitivity.Normal;
      item.LegacyFreeBusyStatus = data.IsOutOfOffice ? LegacyFreeBusyStatus.OOF : LegacyFreeBusyStatus.Free;
             
      item.SetExtendedProperty(new ExtendedPropertyDefinition(
          DefaultExtendedPropertySet.PublicStrings
          , EWSConfig.EXTEND_SUBCRIPTION_ID
          , MapiPropertyType.String)
          , userId);
      item.SetExtendedProperty(new ExtendedPropertyDefinition(
          DefaultExtendedPropertySet.PublicStrings
          , EWSConfig.EXTEND_SUBCRIPTION_VALUE
          , MapiPropertyType.String)
          , EWSHelper.ObjectToString(data));

      var x = item.Update(ConflictResolutionMode.AlwaysOverwrite, SendInvitationsOrCancellationsMode.SendToNone).Result;
    }

    public void Delete(AppointmentModel data)
    {
      var userId = EWSHelper.CreateItemId(data, Setting.TenantId);
      var a = GetAppointmentItem(userId, data.MailAccount);
      if (a == null) return;
      var item = a as Appointment;
      var x = item.Delete(DeleteMode.MoveToDeletedItems, SendCancellationsMode.SendToNone).Result;
    }

    public string ExchangeServerIsValidDetail()
    {
      StreamingSubscriptionConnection _streamSubscriptionConnection = null;

      try
      {
        string serviceUser = Setting.LoginType == 1 ? Setting.ServiceUser : null;
        var s = CreateExchangeService(serviceUser);
        var streamingSubscription = s.SubscribeToStreamingNotifications(
          new FolderId[] { new FolderId(WellKnownFolderName.Calendar) },
          new EventType[] {
          EventType.Created,
          EventType.Modified,
          EventType.Deleted,
          EventType.FreeBusyChanged
          })
          .Result;
        _streamSubscriptionConnection = new StreamingSubscriptionConnection(s, 1);
        _streamSubscriptionConnection.AddSubscription(streamingSubscription);
        _streamSubscriptionConnection.Open();
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
      finally
      {
        if (_streamSubscriptionConnection != null && _streamSubscriptionConnection.IsOpen)
        {
          _streamSubscriptionConnection.Close();
          _streamSubscriptionConnection = null;
        }
      }

      return string.Empty;
    }

    public bool ExchangeServerIsValid()
    {
      return string.IsNullOrEmpty(ExchangeServerIsValidDetail());
    }

  }
}
