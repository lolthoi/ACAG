using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.Exchange;
using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Exchange_Tests
{
  [TestClass()]
  public class EWSService_Tests
  {
    const string TEST_Data_Text = "abacustest";
    const int TEST_Data_EXPR_Project_RecNr = 1000;
    const int TEST_Data_NBW_LeArtNr = 1000;
    const string TEST_Data_EXPR_E_Mail = "mbrt@all-consulting.ch";
    DateTime TEST_Data_DateTime = DateTime.UtcNow;

    private _ACAG_AbacusCalendarConnector PrepareAbacusData(int counter)
    {
      var abacusData = new _ACAG_AbacusCalendarConnector()
      {
        EXPR_E_Mail = TEST_Data_EXPR_E_Mail,
        EXPR_Project_RecNr = string.Format("{0}", (TEST_Data_EXPR_Project_RecNr + counter)),
        NBW_LeArtNr = string.Format("{0}", (TEST_Data_NBW_LeArtNr + counter)),
        TEXT = TEST_Data_Text,
        PKT_StartTime = "2021-01-21 10:10:10",
        PKT_EndTime = "2021-01-21 11:11:11",
        INR = "",
        NAME = "",
        VORNAME = "",
        PKT_Event = "",
      };

      return abacusData;
    }

    private List<AppointmentModel> GetAbacusData(int start = 1, int len = 3)
    {
      if (len < 0)
      {
        len = 1;
      }
      var d = DateTime.UtcNow;

      List<AppointmentModel> appointments = new List<AppointmentModel>();
      List<_ACAG_AbacusCalendarConnector> abacusData = new List<_ACAG_AbacusCalendarConnector>();
      for (int i = start; i < (start + len); i++)
      {
        abacusData.Add(PrepareAbacusData(i));
      }

      Dictionary<int, KeyValuePair<int, string>> result = Logic.Abacus.Appointment.ConvertAPIData(abacusData, ref appointments);

      return appointments;
    }

    private Models.ExchangeSettingModel GetExchangeSetting(bool trulyState = true)
    {
      if (trulyState)
      {
        return new Models.ExchangeSettingModel
        {
          Id = 1,
          Name = "TrullySetting",
          ExchangeVersion = "Exchange2016",
          ExchangeUrl = "https://outlook.office365.com/EWS/Exchange.asmx",
          LoginType = 1,
          AzureTenant = "37b78714-33a5-4cc7-93de-74439cc3f18a",
          AzureClientId = "00a1c54c-11ec-41c3-95b0-0b2a2451f38a",
          AzureClientSecret = "V_dc_S9_U0i50rJQLe2I~X_6UMsNz864o1",
          Description = "TrullySetting",
          EmailAddress = "svc_rethink2@all-consulting.ch",
          ServiceUser = "svc_rethink2@all-consulting.ch",
          ServiceUserPassword = "#QIhe!DNCsgk$3qIpmx5rt",
          IsEnabled = true
        };
      }
      else
      {
        return new Models.ExchangeSettingModel();
      }
    }

    [TestMethod()]
    public void Insert_ValidData_Success()
    {
      //expected -> data1
      var data1 = GetAbacusData(1, 1).FirstOrDefault();
      if (data1 == null)
      {
        Assert.Fail();
        return;
      }

      var ews = new EWSService(GetExchangeSetting(true));
      try
      {
        ews.Insert(data1);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
        return;
      }
      //actual -> item
      var item = ews.GetAbacusData(data1);

      Assert.AreEqual(data1.AbacusID, item.AbacusID);
      Assert.AreEqual(data1.MailAccount, item.MailAccount);
      Assert.AreEqual(data1.Subject, item.Subject);
      Assert.AreEqual(data1.Status, item.Status);

      ews.Delete(data1);
    }

    [TestMethod()]
    public void Insert_InValidData_Failed()
    {
      var data1 = new AppointmentModel();
      var ews = new EWSService(GetExchangeSetting(true));
      try
      {
        ews.Insert(data1);
      }
      catch (Exception ex)
      {
        //expected -> error when insert
        Assert.IsTrue(true, ex.Message);
        return;
      }
    }

    [TestMethod()]
    public void Update_ValidData_Success()
    {
      //expected -> data1, we will update it after insert
      var data1 = GetAbacusData(1, 1).FirstOrDefault();
      if (data1 == null)
      {
        Assert.Fail();
        return;
      }
      var ews = new EWSService(GetExchangeSetting(true));
      try
      {
        ews.Insert(data1);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
        return;
      }

      data1.Subject = data1.Subject + "_updated";
      data1.Status = data1.Status + 1;

      try
      {
        ews.Update(data1);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
        return;
      }

      //actual -> item, after updated we get from exchange server to check
      var item = ews.GetAbacusData(data1);

      Assert.AreEqual(data1.AbacusID, item.AbacusID);
      Assert.AreEqual(data1.ExchangeID, item.ExchangeID);
      Assert.AreEqual(data1.MailAccount, item.MailAccount);
      Assert.AreEqual(data1.Subject, item.Subject);
      Assert.AreEqual(data1.Status, item.Status);

      ews.Delete(data1);
    }

    [TestMethod()]
    public void Update_InValidData_Failed()
    {
      var data1 = new AppointmentModel();

      var ews = new EWSService(GetExchangeSetting(true));
      try
      {
        ews.Update(data1);
      }
      catch (Exception ex)
      {
        //expected -> error when update
        Assert.Fail(ex.Message);
        return;
      }
    }

    [TestMethod()]
    public void Delete_ValidData_Success()
    {
      var data1 = GetAbacusData(1, 1).FirstOrDefault();
      if (data1 == null)
      {
        Assert.Fail();
        return;
      }

      var ews = new EWSService(GetExchangeSetting(true));
      try
      {
        ews.Insert(data1);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
        return;
      }

      ews.Delete(data1);
      var item = ews.GetAbacusData(data1);
      //expected -> null, actual -> item
      Assert.AreEqual(null, item);
    }

    [TestMethod()]
    public void Delete_InValidData_Failed()
    {
      var data1 = GetAbacusData(1, 1).FirstOrDefault();
      if (data1 == null)
      {
        Assert.Fail();
        return;
      }

      var ews = new EWSService(GetExchangeSetting(true));
      try
      {
        ews.Insert(data1);
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
        return;
      }

      var data2 = new AppointmentModel();
      ews.Delete(data2);

      var item = ews.GetAbacusData(data1);

      //expected -> data1, actual -> item
      Assert.AreEqual(data1.AbacusID, item.AbacusID);
      Assert.AreEqual(data1.ExchangeID, item.ExchangeID);
      Assert.AreEqual(data1.MailAccount, item.MailAccount);
      Assert.AreEqual(data1.Subject, item.Subject);
      Assert.AreEqual(data1.Status, item.Status);


      ews.Delete(data1);
    }

    [TestMethod()]
    public void Check_ValidData_Success()
    {
      var ewsSetting = GetExchangeSetting(true);

      var ews = new EWSService(ewsSetting);
      var status = ews.ExchangeServerIsValid();
      //expected -> true, actual -> status
      Assert.AreEqual(true, status);
    }

    [TestMethod()]
    public void Check_InValidData_Failed()
    {
      var ewsSetting = GetExchangeSetting(false);

      var ews = new EWSService(ewsSetting);
      var status = ews.ExchangeServerIsValid();
      //expected -> false, actual -> status
      Assert.AreEqual(false, status);
    }
  }
}