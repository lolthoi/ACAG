using System;
using System.Collections.Generic;

using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;

namespace ACAG.Abacus.CalendarConnector.Logic.Abacus
{
  public static class Appointment
  {
    /// <summary>
    /// Converts the API data.
    /// Dictionary Key => AbacusID
    /// Dictionary Value => Status, AppointmentModel
    /// </summary>
    /// <param name="abacusData">The abacus data.</param>
    /// <param name="appointments">The appointments.</param>
    /// <returns></returns>
    public static Dictionary<int, KeyValuePair<int, string>> ConvertAPIData(List<_ACAG_AbacusCalendarConnector> abacusData, ref List<AppointmentModel> appointments)
    {
      Dictionary<int, KeyValuePair<int, string>> result = new Dictionary<int, KeyValuePair<int, string>>();
      KeyValuePair<int, string> appointmentResult;
      AppointmentModel appointment = null;

      for (int i = 1; i <= abacusData.Count; i++)
      {
        appointment = new AppointmentModel();
        appointmentResult = Convert(abacusData[i-1], ref appointment);
        appointments.Add(appointment);
        result.Add(i, appointmentResult);
      }

      return result;
    }

    private static KeyValuePair<int, string> Convert(_ACAG_AbacusCalendarConnector item, ref AppointmentModel appointment)
    {
      KeyValuePair<int, string> result = new KeyValuePair<int, string>();
      int error = 0;
      long abacusID = 0;
      string mailAccount = null;
      string subject = null;
      DateTime dateTimeStart = DateTime.MinValue;
      DateTime dateTimeEnd = DateTime.MinValue;
      bool isPrivate = false;
      bool isOutOfOffice = false;
      long paytypeCode = 0;
      string logInfo = null;

      #region AbacusID
      if (!long.TryParse(item.EXPR_Project_RecNr, out abacusID))
      {
        abacusID = 0;
        result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "AbacusID Error");
        error++;
      }
      if (abacusID == 0)
      {
        error++;
        result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "AbacusID Error");
      }
      #endregion

      #region Mail Account
      if (error == 0)
      {
        try
        {
          var mail = new System.Net.Mail.MailAddress(item.EXPR_E_Mail);
          mailAccount = mail.Address;
        }
        catch (Exception ex)
        {
          mailAccount = null;
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, $"EMail Error: {ex.Message}");
          error++;
        }
      }
      #endregion

      #region Subject
      if (error == 0)
      {
        subject = item.TEXT;
 
        if (string.IsNullOrEmpty(subject))
        {
          error++;
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "Subject Error");
        }
      }
      #endregion

      #region DateTimeStart
      if (error == 0)
      {
        if (!DateTime.TryParse(item.PKT_StartTime, out dateTimeStart))
        {
          dateTimeStart = DateTime.MinValue;
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "StartTime Error");
          error++;
        }
      }
      #endregion

      #region DateTimeEnd
      if (error == 0)
      {
        if (!DateTime.TryParse(item.PKT_StartTime, out dateTimeStart))
        {
          dateTimeStart = DateTime.MinValue;
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "StartTime Error");
          error++;
        }
        if (!DateTime.TryParse(item.PKT_EndTime, out dateTimeEnd))
        {
          dateTimeEnd = DateTime.MinValue;
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "EndTime Error");
          error++;
        }

        if (dateTimeStart > dateTimeEnd)
        {
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "EndTime Error");
          error++;
        }
      }
      #endregion

      #region PaytypeCode
      if (error == 0)
      {
        if (!long.TryParse(item.NBW_LeArtNr, out paytypeCode))
        {
          paytypeCode = 0;
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "Paytype Error");
          error++;
        }
        if (paytypeCode == 0)
        {
          error++;
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "Paytype Error");
        }
      }
      #endregion

      #region LogInfo
      if (error == 0)
      {
        logInfo = $"Event:{item.PKT_Event} Mail:{item.EXPR_E_Mail} AddressNo:{item.INR} for User: {item.NAME}, {item.VORNAME}";

        if (string.IsNullOrEmpty(logInfo))
        {
          error++;
          result = new KeyValuePair<int, string>((int)EnumAppointmentState.Error, "Loginfo Error");
        }
      }
      #endregion

      if (error == 0)
      {
        appointment = new AppointmentModel(
          0,
          abacusID,
          null,
          DateTime.Now,
          mailAccount,
          subject,
          dateTimeStart,
          dateTimeEnd,
          isPrivate,
          isOutOfOffice,
          (int)EnumAppointmentState.New,
          paytypeCode.ToString(),
          logInfo);

        result = new KeyValuePair<int, string>((int)EnumAppointmentState.New, string.Empty);
      }

      return result;
    }
  }
}
