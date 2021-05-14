using System;
using System.Collections.Generic;

using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Abacus.UnitTests
{
  [TestClass()]
  public class TestConnection : TestBase
  {
    #region Constants
    const bool USE_TestSever = false;

    const int TEST_Tenant_Number = 200;
    const string TEST_Tenant_Name = "Tenant Unit Test";
    const string TEST_Tenant_Description = "Tenant Unit Test Description";
    const string TEST_AbacusSetting_Name = "AbacusSetting Unit Test";
    const string TEST_AbacusSetting_Description = "AbacusSetting Unit Test Description";
    const string TEST_AbacusSetting_ServiceUser = "acag";
    const string TEST_AbacusSetting_ServicePasswordCrypted = "RFY2TYecnOiRQX4F52n30g==";

    #region ALLCUS
    const string TEST_AbacusSetting_ServiceServerName = "allcus.all-consulting.ch";
    const int TEST_AbacusSetting_ServicePort = 443;
    const bool TEST_AbacusSetting_UseSsl = true;
    #endregion

    #region ABATEST01
    const string TEST_Testserver_AbacusSetting_ServiceServerName = "acabatest01";
    const int TEST_Testserver_AbacusSetting_ServicePort = 40000;
    const bool TEST_Testserver_AbacusSetting_UseSsl = false;
    #endregion
    #endregion

    #region Valid Data
    [TestMethod]
    public void TestConnectionAPI_GetWithValidConnectionDetails_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      DateTime start = DateTime.Now;
      var result = WrapperAPI.GetRequest("/api/testconnection", tenant);
      DateTime stop = DateTime.Now;
      var data = result.ToContentString();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Contains("Connection successful!"));
      System.Diagnostics.Debug.WriteLine($"Duration: {stop.Subtract(start)}");
    }
    #endregion

    #region Invalid Data
    #region Tenant
    #region Number
    [TestMethod]
    public void TestConnectionAPI_GetWithTenantNumberInvalid_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        666,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }
    #endregion

    #region Name
    [TestMethod]
    public void TestConnectionAPI_GetWithTenantNameEmpty_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        string.Empty,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Count > 0);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithTenantNameNull_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        null,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Count > 0);
    }
    #endregion

    #region Description
    [TestMethod]
    public void TestConnectionAPI_GetWithTenantDescriptionEmpty_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        string.Empty,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Count > 0);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithTenantDescriptionNull_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        null,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Count > 0);
    }
    #endregion
    #endregion

    #region AbacusSetting
    #region Name
    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingNameEmpty_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        string.Empty,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Count > 0);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingNameNull_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        null,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Count > 0);
    }
    #endregion

    #region Description
    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingDescriptionEmpty_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        string.Empty,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Count > 0);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingDescriptionNull_Success()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        null,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsTrue(result.IsSuccessStatusCode);
      Assert.IsNotNull(data);
      Assert.IsTrue(data.Count > 0);
    }
    #endregion

    #region ServiceServerName
    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServiceServerNameNotExists_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        "Not-Exists",
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServiceServerNameEmpty_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        string.Empty,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServiceServerNameNull_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        null,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }
    #endregion

    #region ServicePort
    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServicePortNotExists_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        666,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }
    #endregion

    #region ServiceSecurity
    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServiceSecurityWrong_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? !TEST_Testserver_AbacusSetting_UseSsl : !TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }
    #endregion

    #region ServiceServerUser
    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServiceUserNotExists_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        "Not-Exists",
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServiceUserEmpty_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        string.Empty,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServiceUserNull_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        null,
        TEST_AbacusSetting_ServicePasswordCrypted);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }
    #endregion

    #region ServiceServerPassword
    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServicePasswordNotExists_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        "Not-Exists");

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServicePasswordEmpty_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        string.Empty);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }

    [TestMethod]
    public void TestConnectionAPI_GetWithAbacusSettingServicePasswordNull_Fail()
    {
      // arrange
      AbacusSettingModel abacusSetting = new AbacusSettingModel(
        TEST_AbacusSetting_Name,
        TEST_AbacusSetting_Description,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServiceServerName : TEST_AbacusSetting_ServiceServerName,
        USE_TestSever ? TEST_Testserver_AbacusSetting_ServicePort : TEST_AbacusSetting_ServicePort,
        USE_TestSever ? TEST_Testserver_AbacusSetting_UseSsl : TEST_AbacusSetting_UseSsl,
        TEST_AbacusSetting_ServiceUser,
        null);

      TenantModel tenant = new TenantModel(
        TEST_Tenant_Name,
        TEST_Tenant_Description,
        TEST_Tenant_Number,
        abacusSetting);

      // act
      var result = WrapperAPI.GetRequest("/api/reportaccessor", tenant);
      var data = result.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();

      // assert
      Assert.IsFalse(result.IsSuccessStatusCode);
      Assert.IsNull(data);
    }
    #endregion
    #endregion
    #endregion
  }
}
