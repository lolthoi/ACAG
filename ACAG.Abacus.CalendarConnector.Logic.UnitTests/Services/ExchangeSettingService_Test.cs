using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services
{
  [TestClass()]
  public class ExchangeSettingService_Test : TestBase
  {
    private IExchangeSettingService _exchangeSettingService;
    private ITenantService _tenantService;
    private IUserService _userService;

    private TenantModel _tenantModel;
    private TenantModelForUser _tenantModelForUser;
    private UserModel _userModel;
    private List<ExchangeVersionModel> _exchangeVersionModels;
    private List<ExchangeLoginTypeModel> _exchangeLoginTypeModels;

    protected override void InitServices()
    {
      _exchangeSettingService = _scope.ServiceProvider.GetService<IExchangeSettingService>();
      _exchangeVersionModels = _exchangeSettingService.GetAllExchangeVersions();
      _exchangeLoginTypeModels = _exchangeSettingService.GetExchangeLoginTypes();
      _tenantService = _scope.ServiceProvider.GetService<ITenantService>();
      _userService = _scope.ServiceProvider.GetService<IUserService>();
    }
    protected override void InitEnvirontment()
    {
      _testService.StartLoginWithAdmin()
          .ThenImplementTest(param =>
          {
            _tenantModel = new TenantModel
            {
              Name = "TenantTest_" + Guid.NewGuid().ToString(),
              Description = "TenantDescription_Test_" + Guid.NewGuid().ToString(),
              IsEnabled = true,
              Number = new Random().Next(10000, 99000),
              ScheduleTimer = new Random().Next(1, 180)
            };
            _tenantService.Create(_tenantModel);

            _tenantModelForUser = new TenantModelForUser()
            {
              Id = _tenantModel.Id,
              Name = _tenantModel.Name
            };


            _userModel = GetRandomUser(_tenantModelForUser);
            _userService.Create(_userModel, "123456");
          });
    }
    protected override void CleanEnvirontment()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          _userService.Delete(_userModel.Id);
          _tenantService.Delete(_tenantModel.Id);
        });
    }

    #region GET_ALL_EXCHANGE_SETTING_BY_TENANT_ID

    [TestMethod()]
    public void SysAdminRoleGetAll_ValidDataWithEmptySearchText_Success()
    {
      _testService
        .StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          string searchText = "";
          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);
          var searchResult = _exchangeSettingService.GetAll(createModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.ExchangeSettings.Where(x => x.Id == createModel.Data.Id).FirstOrDefault();

          #endregion

          #region Get and check data

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(createModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createModel.Data.TenantId, tenantCreatedInSearchResult.TenantId);
          Assert.AreEqual(createModel.Data.Name, tenantCreatedInSearchResult.Name);
          Assert.AreEqual(createModel.Data.HealthStatus, tenantCreatedInSearchResult.HealthStatus);
          Assert.AreEqual(createModel.Data.ExchangeLoginTypeModel.Id, tenantCreatedInSearchResult.LoginType);
          Assert.AreEqual(createModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetAll_ValidDataWithSearchText_Success()
    {
      _testService
        .StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          var model = GetRandomExchangeSettingModel();
          string searchText = model.Name;
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);
          var searchResult = _exchangeSettingService.GetAll(createModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.ExchangeSettings.Where(x => x.Id == createModel.Data.Id).FirstOrDefault();

          #endregion

          #region Get and check data

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(createModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createModel.Data.TenantId, tenantCreatedInSearchResult.TenantId);
          Assert.AreEqual(createModel.Data.Name, tenantCreatedInSearchResult.Name);
          Assert.AreEqual(createModel.Data.HealthStatus, tenantCreatedInSearchResult.HealthStatus);
          Assert.AreEqual(createModel.Data.ExchangeLoginTypeModel.Id, tenantCreatedInSearchResult.LoginType);
          Assert.AreEqual(createModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetAll_InvalidTenantIdAndSearchTextNull_Fail()
    {
      _testService
        .StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {

          #region Init Data

          var searchResult = _exchangeSettingService.GetAll(0, "");

          #endregion

          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(ErrorType.BAD_REQUEST, searchResult.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, searchResult.Error.Message);

          #region Get and check data

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void AdminRoleGetAll_ValidDataWithEmptySearchText_Success()
    {
      _testService
        .StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          string searchText = "";
          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);
          var searchResult = _exchangeSettingService.GetAll(createModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.ExchangeSettings.Where(x => x.Id == createModel.Data.Id).FirstOrDefault();

          #endregion

          #region Get and check data

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(createModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createModel.Data.TenantId, tenantCreatedInSearchResult.TenantId);
          Assert.AreEqual(createModel.Data.Name, tenantCreatedInSearchResult.Name);
          Assert.AreEqual(createModel.Data.HealthStatus, tenantCreatedInSearchResult.HealthStatus);
          Assert.AreEqual(createModel.Data.ExchangeLoginTypeModel.Id, tenantCreatedInSearchResult.LoginType);
          Assert.AreEqual(createModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetAll_ValidDataWithSearchText_Success()
    {
      _testService
        .StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          var model = GetRandomExchangeSettingModel();
          string searchText = model.Name;
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);
          var searchResult = _exchangeSettingService.GetAll(createModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.ExchangeSettings.Where(x => x.Id == createModel.Data.Id).FirstOrDefault();

          #endregion

          #region Get and check data

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(createModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createModel.Data.TenantId, tenantCreatedInSearchResult.TenantId);
          Assert.AreEqual(createModel.Data.Name, tenantCreatedInSearchResult.Name);
          Assert.AreEqual(createModel.Data.HealthStatus, tenantCreatedInSearchResult.HealthStatus);
          Assert.AreEqual(createModel.Data.ExchangeLoginTypeModel.Id, tenantCreatedInSearchResult.LoginType);
          Assert.AreEqual(createModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetAll_InvalidTenantIdAndSearchTextNull_Fail()
    {
      _testService
        .StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {

          #region Init Data

          var searchResult = _exchangeSettingService.GetAll(0, "");

          #endregion

          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(ErrorType.BAD_REQUEST, searchResult.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, searchResult.Error.Message);

          #region Get and check data

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleGetAll_ValidDataWithEmptySearchText_Success()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init Data

          string searchText = "";
          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);
          var searchResult = _exchangeSettingService.GetAll(createModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.ExchangeSettings.Where(x => x.Id == createModel.Data.Id).FirstOrDefault();

          #endregion

          #region Get and check data

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(createModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createModel.Data.TenantId, tenantCreatedInSearchResult.TenantId);
          Assert.AreEqual(createModel.Data.Name, tenantCreatedInSearchResult.Name);
          Assert.AreEqual(createModel.Data.HealthStatus, tenantCreatedInSearchResult.HealthStatus);
          Assert.AreEqual(createModel.Data.ExchangeLoginTypeModel.Id, tenantCreatedInSearchResult.LoginType);
          Assert.AreEqual(createModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetAll_ValidDataWithSearchText_Success()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init Data

          var model = GetRandomExchangeSettingModel();
          string searchText = model.Name;
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);
          var searchResult = _exchangeSettingService.GetAll(createModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.ExchangeSettings.Where(x => x.Id == createModel.Data.Id).FirstOrDefault();

          #endregion

          #region Get and check data

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(createModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createModel.Data.TenantId, tenantCreatedInSearchResult.TenantId);
          Assert.AreEqual(createModel.Data.Name, tenantCreatedInSearchResult.Name);
          Assert.AreEqual(createModel.Data.HealthStatus, tenantCreatedInSearchResult.HealthStatus);
          Assert.AreEqual(createModel.Data.ExchangeLoginTypeModel.Id, tenantCreatedInSearchResult.LoginType);
          Assert.AreEqual(createModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetAll_InvalidTenantIdAndSearchTextNull_Fail()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {

          #region Init Data

          var searchResult = _exchangeSettingService.GetAll(0, "");

          #endregion

          #region Get and check data

          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(ErrorType.BAD_REQUEST, searchResult.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, searchResult.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    #endregion

    #region GET_BY_ID_EXCHANGE_SETTING_BY_TENANT_ID

    [TestMethod]
    public void SysAdminRoleGetById_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          #endregion

          #region Get and check data

          var result = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);

          Assert.AreEqual(model.Id, result.Data.Id);
          Assert.AreEqual(model.Name, result.Data.Name);
          Assert.AreEqual(model.TenantId, result.Data.TenantId);
          Assert.AreEqual(model.ExchangeVersionModel.Name, result.Data.ExchangeVersion);
          Assert.AreEqual(model.ExchangeUrl, result.Data.ExchangeUrl);
          Assert.AreEqual(model.ExchangeLoginTypeModel.Id, result.Data.LoginType);
          Assert.AreEqual(model.AzureClientId, result.Data.AzureClientId);
          Assert.AreEqual(model.AzureClientSecret, result.Data.AzureClientSecret);
          Assert.AreEqual(model.AzureTenant, result.Data.AzureTenant);
          Assert.AreEqual(model.EmailAddress, result.Data.EmailAddress);
          Assert.AreEqual(model.ServiceUser, result.Data.ServiceUser);
          Assert.AreEqual(model.ServiceUserPassword, result.Data.ServiceUserPassword);
          Assert.AreEqual(model.IsEnabled, result.Data.IsEnabled);
          Assert.AreEqual(model.Description, result.Data.Description);
          Assert.AreEqual(model.HealthStatus, result.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetById_InValidExchangeSettingId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _exchangeSettingService.GetById(_tenantModel.Id, 0);

          #endregion

          #region Get and check data

          Assert.IsNull(result.Data);
          Assert.IsNotNull(result.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod]
    public void AdminRoleGetById_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          #endregion

          #region Get and check data

          var result = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);

          Assert.AreEqual(model.Id, result.Data.Id);
          Assert.AreEqual(model.Name, result.Data.Name);
          Assert.AreEqual(model.TenantId, result.Data.TenantId);
          Assert.AreEqual(model.ExchangeVersionModel.Name, result.Data.ExchangeVersion);
          Assert.AreEqual(model.ExchangeUrl, result.Data.ExchangeUrl);
          Assert.AreEqual(model.ExchangeLoginTypeModel.Id, result.Data.LoginType);
          Assert.AreEqual(model.AzureClientId, result.Data.AzureClientId);
          Assert.AreEqual(model.AzureClientSecret, result.Data.AzureClientSecret);
          Assert.AreEqual(model.AzureTenant, result.Data.AzureTenant);
          Assert.AreEqual(model.EmailAddress, result.Data.EmailAddress);
          Assert.AreEqual(model.ServiceUser, result.Data.ServiceUser);
          Assert.AreEqual(model.ServiceUserPassword, result.Data.ServiceUserPassword);
          Assert.AreEqual(model.IsEnabled, result.Data.IsEnabled);
          Assert.AreEqual(model.Description, result.Data.Description);
          Assert.AreEqual(model.HealthStatus, result.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetById_InValidExchangeSettingId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _exchangeSettingService.GetById(_tenantModel.Id, 0);

          #endregion

          #region Get and check data

          Assert.IsNull(result.Data);
          Assert.IsNotNull(result.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod]
    public void UserRoleGetById_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          #endregion

          #region Get and check data

          var result = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);

          Assert.AreEqual(model.Id, result.Data.Id);
          Assert.AreEqual(model.Name, result.Data.Name);
          Assert.AreEqual(model.TenantId, result.Data.TenantId);
          Assert.AreEqual(model.ExchangeVersionModel.Name, result.Data.ExchangeVersion);
          Assert.AreEqual(model.ExchangeUrl, result.Data.ExchangeUrl);
          Assert.AreEqual(model.ExchangeLoginTypeModel.Id, result.Data.LoginType);
          Assert.AreEqual(model.AzureClientId, result.Data.AzureClientId);
          Assert.AreEqual(model.AzureClientSecret, result.Data.AzureClientSecret);
          Assert.AreEqual(model.AzureTenant, result.Data.AzureTenant);
          Assert.AreEqual(model.EmailAddress, result.Data.EmailAddress);
          Assert.AreEqual(model.ServiceUser, result.Data.ServiceUser);
          Assert.AreEqual(model.ServiceUserPassword, result.Data.ServiceUserPassword);
          Assert.AreEqual(model.IsEnabled, result.Data.IsEnabled);
          Assert.AreEqual(model.Description, result.Data.Description);
          Assert.AreEqual(model.HealthStatus, result.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetById_InValidExchangeSettingId_Fail()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _exchangeSettingService.GetById(_tenantModel.Id, 0);

          #endregion

          #region Get and check data

          Assert.IsNull(result.Data);
          Assert.IsNotNull(result.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    #endregion

    #region CREATE_EXCHANGE_SETTING_BY_TENANT_ID

    [TestMethod]
    public void SysAdminRoleAddExchangeSetting_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createModel.Data);
          Assert.IsNull(createModel.Error);
          var actualModel = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(actualModel.Data);
          Assert.IsNull(actualModel.Error);

          Assert.AreEqual(model.Id, actualModel.Data.Id);
          Assert.AreEqual(model.Name, actualModel.Data.Name);
          Assert.AreEqual(model.TenantId, actualModel.Data.TenantId);
          Assert.AreEqual(model.ExchangeVersionModel.Name, actualModel.Data.ExchangeVersion);
          Assert.AreEqual(model.ExchangeUrl, actualModel.Data.ExchangeUrl);
          Assert.AreEqual(model.ExchangeLoginTypeModel.Id, actualModel.Data.LoginType);
          Assert.AreEqual(model.AzureClientId, actualModel.Data.AzureClientId);
          Assert.AreEqual(model.AzureClientSecret, actualModel.Data.AzureClientSecret);
          Assert.AreEqual(model.AzureTenant, actualModel.Data.AzureTenant);
          Assert.AreEqual(model.EmailAddress, actualModel.Data.EmailAddress);
          Assert.AreEqual(model.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(model.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(model.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(model.Description, actualModel.Data.Description);
          Assert.AreEqual(model.HealthStatus, actualModel.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void SysAdminRoleAddExchangeSetting_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          var invalidNameIsAlreadyExists = GetRandomExchangeSettingModel();
          invalidNameIsAlreadyExists.Name = createModel.Data.Name;


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);
          invalidModels.Add(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists);
          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.Create(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNameIsAlreadyExists):
                Assert.AreEqual(ErrorType.DUPLICATED, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleAddExchangeSetting_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createModel.Data);
          Assert.IsNull(createModel.Error);
          var actualModel = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(actualModel.Data);
          Assert.IsNull(actualModel.Error);

          Assert.AreEqual(model.Id, actualModel.Data.Id);
          Assert.AreEqual(model.Name, actualModel.Data.Name);
          Assert.AreEqual(model.TenantId, actualModel.Data.TenantId);
          Assert.AreEqual(model.ExchangeVersionModel.Name, actualModel.Data.ExchangeVersion);
          Assert.AreEqual(model.ExchangeUrl, actualModel.Data.ExchangeUrl);
          Assert.AreEqual(model.ExchangeLoginTypeModel.Id, actualModel.Data.LoginType);
          Assert.AreEqual(model.AzureClientId, actualModel.Data.AzureClientId);
          Assert.AreEqual(model.AzureClientSecret, actualModel.Data.AzureClientSecret);
          Assert.AreEqual(model.AzureTenant, actualModel.Data.AzureTenant);
          Assert.AreEqual(model.EmailAddress, actualModel.Data.EmailAddress);
          Assert.AreEqual(model.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(model.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(model.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(model.Description, actualModel.Data.Description);
          Assert.AreEqual(model.HealthStatus, actualModel.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleAddExchangeSetting_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          var invalidNameIsAlreadyExists = GetRandomExchangeSettingModel();
          invalidNameIsAlreadyExists.Name = createModel.Data.Name;


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);
          invalidModels.Add(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists);
          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.Create(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNameIsAlreadyExists):
                Assert.AreEqual(ErrorType.DUPLICATED, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void UserRoleAddExchangeSetting_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createModel.Data);
          Assert.IsNull(createModel.Error);
          var actualModel = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(actualModel.Data);
          Assert.IsNull(actualModel.Error);

          Assert.AreEqual(model.Id, actualModel.Data.Id);
          Assert.AreEqual(model.Name, actualModel.Data.Name);
          Assert.AreEqual(model.TenantId, actualModel.Data.TenantId);
          Assert.AreEqual(model.ExchangeVersionModel.Name, actualModel.Data.ExchangeVersion);
          Assert.AreEqual(model.ExchangeUrl, actualModel.Data.ExchangeUrl);
          Assert.AreEqual(model.ExchangeLoginTypeModel.Id, actualModel.Data.LoginType);
          Assert.AreEqual(model.AzureClientId, actualModel.Data.AzureClientId);
          Assert.AreEqual(model.AzureClientSecret, actualModel.Data.AzureClientSecret);
          Assert.AreEqual(model.AzureTenant, actualModel.Data.AzureTenant);
          Assert.AreEqual(model.EmailAddress, actualModel.Data.EmailAddress);
          Assert.AreEqual(model.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(model.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(model.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(model.Description, actualModel.Data.Description);
          Assert.AreEqual(model.HealthStatus, actualModel.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void UserRoleAddExchangeSetting_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          var invalidNameIsAlreadyExists = GetRandomExchangeSettingModel();
          invalidNameIsAlreadyExists.Name = createModel.Data.Name;


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);
          invalidModels.Add(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists);
          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.Create(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.NO_DATA_ROLE, actualModel.Error.Type);
                Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNameIsAlreadyExists):
                Assert.AreEqual(ErrorType.DUPLICATED, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    #endregion

    #region UPDATE_EXCHANGE_SETTING_BY_TENANT_ID

    [TestMethod]
    public void SysAdminRoleUpdateExchangeSetting_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          var modelForUpdate = GetRandomExchangeSettingModel();
          modelForUpdate.Id = createModel.Data.Id;
          var updateModel = _exchangeSettingService.Update(modelForUpdate);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createModel.Data);
          Assert.IsNull(createModel.Error);
          var actualModel = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(actualModel.Data);
          Assert.IsNull(actualModel.Error);

          Assert.AreEqual(modelForUpdate.Id, actualModel.Data.Id);
          Assert.AreEqual(modelForUpdate.Name, actualModel.Data.Name);
          Assert.AreEqual(modelForUpdate.TenantId, actualModel.Data.TenantId);
          Assert.AreEqual(modelForUpdate.ExchangeVersionModel.Name, actualModel.Data.ExchangeVersion);
          Assert.AreEqual(modelForUpdate.ExchangeUrl, actualModel.Data.ExchangeUrl);
          Assert.AreEqual(modelForUpdate.ExchangeLoginTypeModel.Id, actualModel.Data.LoginType);
          Assert.AreEqual(modelForUpdate.AzureClientId, actualModel.Data.AzureClientId);
          Assert.AreEqual(modelForUpdate.AzureClientSecret, actualModel.Data.AzureClientSecret);
          Assert.AreEqual(modelForUpdate.AzureTenant, actualModel.Data.AzureTenant);
          Assert.AreEqual(modelForUpdate.EmailAddress, actualModel.Data.EmailAddress);
          Assert.AreEqual(modelForUpdate.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(modelForUpdate.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(modelForUpdate.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(modelForUpdate.Description, actualModel.Data.Description);
          Assert.AreEqual(modelForUpdate.HealthStatus, actualModel.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void SysAdminRoleUpdateExchangeSetting_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          var createModel = _exchangeSettingService.Create(GetRandomExchangeSettingModel());
          param.CleanData.Add("CreateModel1", createModel.Data);
          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;
          invalidTenantIdInvalid.Id = createModel.Data.Id;

          ExchangeSettingModel invalidTenantIdNotFound = GetRandomExchangeSettingModel();
          invalidTenantIdNotFound.Id = 1000;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);

          var model = GetRandomExchangeSettingModel();
          var createModel2 = _exchangeSettingService.Create(model);
          var createModel3 = _exchangeSettingService.Create(GetRandomExchangeSettingModel());
          param.CleanData.Add("CreateModel2", createModel2.Data);
          param.CleanData.Add("CreateModel3", createModel3.Data);

          var invalidNameIsAlreadyExists = createModel3.Data;
          invalidNameIsAlreadyExists.Name = createModel.Data.Name;


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidTenantIdNotFound), invalidTenantIdNotFound);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);
          invalidModels.Add(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists);
          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdNotFound):
                Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNameIsAlreadyExists):
                Assert.AreEqual(ErrorType.DUPLICATED, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel1");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
          var createModel2 = param.CleanData.Get<ExchangeSettingModel>("CreateModel2");
          if (createModel2 != null)
          {
            _exchangeSettingService.Delete(createModel2.TenantId, createModel2.Id);
          }
          var createModel3 = param.CleanData.Get<ExchangeSettingModel>("CreateModel3");
          if (createModel3 != null)
          {
            _exchangeSettingService.Delete(createModel3.TenantId, createModel3.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleUpdateExchangeSetting_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          var modelForUpdate = GetRandomExchangeSettingModel();
          modelForUpdate.Id = createModel.Data.Id;
          var updateModel = _exchangeSettingService.Update(modelForUpdate);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createModel.Data);
          Assert.IsNull(createModel.Error);
          var actualModel = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(actualModel.Data);
          Assert.IsNull(actualModel.Error);

          Assert.AreEqual(modelForUpdate.Id, actualModel.Data.Id);
          Assert.AreEqual(modelForUpdate.Name, actualModel.Data.Name);
          Assert.AreEqual(modelForUpdate.TenantId, actualModel.Data.TenantId);
          Assert.AreEqual(modelForUpdate.ExchangeVersionModel.Name, actualModel.Data.ExchangeVersion);
          Assert.AreEqual(modelForUpdate.ExchangeUrl, actualModel.Data.ExchangeUrl);
          Assert.AreEqual(modelForUpdate.ExchangeLoginTypeModel.Id, actualModel.Data.LoginType);
          Assert.AreEqual(modelForUpdate.AzureClientId, actualModel.Data.AzureClientId);
          Assert.AreEqual(modelForUpdate.AzureClientSecret, actualModel.Data.AzureClientSecret);
          Assert.AreEqual(modelForUpdate.AzureTenant, actualModel.Data.AzureTenant);
          Assert.AreEqual(modelForUpdate.EmailAddress, actualModel.Data.EmailAddress);
          Assert.AreEqual(modelForUpdate.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(modelForUpdate.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(modelForUpdate.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(modelForUpdate.Description, actualModel.Data.Description);
          Assert.AreEqual(modelForUpdate.HealthStatus, actualModel.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleUpdateExchangeSetting_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          var createModel = _exchangeSettingService.Create(GetRandomExchangeSettingModel());
          param.CleanData.Add("CreateModel1", createModel.Data);
          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;
          invalidTenantIdInvalid.Id = createModel.Data.Id;

          ExchangeSettingModel invalidTenantIdNotFound = GetRandomExchangeSettingModel();
          invalidTenantIdNotFound.Id = 1000;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);

          var model = GetRandomExchangeSettingModel();
          var createModel2 = _exchangeSettingService.Create(model);
          var createModel3 = _exchangeSettingService.Create(GetRandomExchangeSettingModel());
          param.CleanData.Add("CreateModel2", createModel2.Data);
          param.CleanData.Add("CreateModel3", createModel3.Data);

          var invalidNameIsAlreadyExists = createModel3.Data;
          invalidNameIsAlreadyExists.Name = createModel.Data.Name;


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidTenantIdNotFound), invalidTenantIdNotFound);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);
          invalidModels.Add(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists);
          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdNotFound):
                Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNameIsAlreadyExists):
                Assert.AreEqual(ErrorType.DUPLICATED, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel1");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
          var createModel2 = param.CleanData.Get<ExchangeSettingModel>("CreateModel2");
          if (createModel2 != null)
          {
            _exchangeSettingService.Delete(createModel2.TenantId, createModel2.Id);
          }
          var createModel3 = param.CleanData.Get<ExchangeSettingModel>("CreateModel3");
          if (createModel3 != null)
          {
            _exchangeSettingService.Delete(createModel3.TenantId, createModel3.Id);
          }
        });
    }

    [TestMethod]
    public void UserRoleUpdateExchangeSetting_ValidExchangeSettingId_Success()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createModel = _exchangeSettingService.Create(model);
          param.CleanData.Add("CreateModel", createModel.Data);

          var modelForUpdate = GetRandomExchangeSettingModel();
          modelForUpdate.Id = createModel.Data.Id;
          var updateModel = _exchangeSettingService.Update(modelForUpdate);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createModel.Data);
          Assert.IsNull(createModel.Error);
          var actualModel = _exchangeSettingService.GetById(createModel.Data.TenantId, createModel.Data.Id);
          Assert.IsNotNull(actualModel.Data);
          Assert.IsNull(actualModel.Error);

          Assert.AreEqual(modelForUpdate.Id, actualModel.Data.Id);
          Assert.AreEqual(modelForUpdate.Name, actualModel.Data.Name);
          Assert.AreEqual(modelForUpdate.TenantId, actualModel.Data.TenantId);
          Assert.AreEqual(modelForUpdate.ExchangeVersionModel.Name, actualModel.Data.ExchangeVersion);
          Assert.AreEqual(modelForUpdate.ExchangeUrl, actualModel.Data.ExchangeUrl);
          Assert.AreEqual(modelForUpdate.ExchangeLoginTypeModel.Id, actualModel.Data.LoginType);
          Assert.AreEqual(modelForUpdate.AzureClientId, actualModel.Data.AzureClientId);
          Assert.AreEqual(modelForUpdate.AzureClientSecret, actualModel.Data.AzureClientSecret);
          Assert.AreEqual(modelForUpdate.AzureTenant, actualModel.Data.AzureTenant);
          Assert.AreEqual(modelForUpdate.EmailAddress, actualModel.Data.EmailAddress);
          Assert.AreEqual(modelForUpdate.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(modelForUpdate.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(modelForUpdate.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(modelForUpdate.Description, actualModel.Data.Description);
          Assert.AreEqual(modelForUpdate.HealthStatus, actualModel.Data.HealthStatus);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void UserRoleUpdateExchangeSetting_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          var createModel = _exchangeSettingService.Create(GetRandomExchangeSettingModel());
          param.CleanData.Add("CreateModel1", createModel.Data);
          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;
          invalidTenantIdInvalid.Id = createModel.Data.Id;

          ExchangeSettingModel invalidTenantIdNotFound = GetRandomExchangeSettingModel();
          invalidTenantIdNotFound.Id = 1000;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);

          var model = GetRandomExchangeSettingModel();
          var createModel2 = _exchangeSettingService.Create(model);
          var createModel3 = _exchangeSettingService.Create(GetRandomExchangeSettingModel());
          param.CleanData.Add("CreateModel2", createModel2.Data);
          param.CleanData.Add("CreateModel3", createModel3.Data);

          var invalidNameIsAlreadyExists = createModel3.Data;
          invalidNameIsAlreadyExists.Name = createModel.Data.Name;


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidTenantIdNotFound), invalidTenantIdNotFound);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);
          invalidModels.Add(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists);
          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.NO_DATA_ROLE, actualModel.Error.Type);
                Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdNotFound):
                Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNameIsAlreadyExists):
                Assert.AreEqual(ErrorType.DUPLICATED, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel1");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
          var createModel2 = param.CleanData.Get<ExchangeSettingModel>("CreateModel2");
          if (createModel2 != null)
          {
            _exchangeSettingService.Delete(createModel2.TenantId, createModel2.Id);
          }
          var createModel3 = param.CleanData.Get<ExchangeSettingModel>("CreateModel3");
          if (createModel3 != null)
          {
            _exchangeSettingService.Delete(createModel3.TenantId, createModel3.Id);
          }
        });
    }

    #endregion

    #region DELETE_EXCHANGE_SETTING_BY_TENANT_ID

    [TestMethod()]
    public void SysAdminRoleDelete_ValidData_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createdModel = _exchangeSettingService.Create(model);
          Assert.IsNotNull(createdModel.Data);

          var deleteResult = _exchangeSettingService.Delete(createdModel.Data.TenantId, createdModel.Data.Id);

          #endregion Init data

          Assert.IsNotNull(deleteResult.Data);
          Assert.AreEqual(true, deleteResult.Data);

          var actualModel = _exchangeSettingService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.IsNull(actualModel.Data);
          Assert.IsNotNull(actualModel.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, actualModel.Error.Message);

        }).ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void SysAdminRoleDelete_InvalidExchangeSetting_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          int ExchangeSettingId = 0;
          var deleteResult = _exchangeSettingService.Delete(_tenantModel.Id, ExchangeSettingId);

          #endregion Init data

          Assert.IsNotNull(deleteResult.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, deleteResult.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, deleteResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void AdminRoleDelete_ValidData_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createdModel = _exchangeSettingService.Create(model);
          Assert.IsNotNull(createdModel.Data);

          var deleteResult = _exchangeSettingService.Delete(createdModel.Data.TenantId, createdModel.Data.Id);

          #endregion Init data

          Assert.IsNotNull(deleteResult.Data);
          Assert.AreEqual(true, deleteResult.Data);

          var actualModel = _exchangeSettingService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.IsNull(actualModel.Data);
          Assert.IsNotNull(actualModel.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, actualModel.Error.Message);

        }).ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void AdminRoleDelete_InvalidExchangeSetting_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          int ExchangeSettingId = 0;
          var deleteResult = _exchangeSettingService.Delete(_tenantModel.Id, ExchangeSettingId);

          #endregion Init data

          Assert.IsNotNull(deleteResult.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, deleteResult.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, deleteResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleDelete_ValidData_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = GetRandomExchangeSettingModel();
          var createdModel = _exchangeSettingService.Create(model);
          Assert.IsNotNull(createdModel.Data);

          var deleteResult = _exchangeSettingService.Delete(createdModel.Data.TenantId, createdModel.Data.Id);

          #endregion Init data

          Assert.IsNotNull(deleteResult.Data);
          Assert.AreEqual(true, deleteResult.Data);

          var actualModel = _exchangeSettingService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.IsNull(actualModel.Data);
          Assert.IsNotNull(actualModel.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, actualModel.Error.Message);

        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleDelete_InvalidExchangeSettingId_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          int ExchangeSettingId = 0;
          var deleteResult = _exchangeSettingService.Delete(_tenantModel.Id, ExchangeSettingId);

          #endregion Init data

          Assert.IsNotNull(deleteResult.Error);
          Assert.IsNotNull(deleteResult.Data);
          Assert.AreEqual(false, deleteResult.Data);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, deleteResult.Error.Message);
          Assert.AreEqual(ErrorType.NOT_EXIST, deleteResult.Error.Type);
        })
        .ThenCleanDataTest(param => { });
    }


    #endregion

    #region CHECK_CONNECTION

    [TestMethod]
    public void SysAdminRoleCheckConnection_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.CheckConnection(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleCheckConnection_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.CheckConnection(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod]
    public void UserRoleCheckConnection_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init Data

          ExchangeSettingModel nullModel = null;

          ExchangeSettingModel invalidTenantIdInvalid = GetRandomExchangeSettingModel();
          invalidTenantIdInvalid.TenantId = 0;

          ExchangeSettingModel invalidNullName = GetRandomExchangeSettingModel();
          invalidNullName.Name = "";

          ExchangeSettingModel invalidMaxlengthName = GetRandomExchangeSettingModel();
          invalidMaxlengthName.Name = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh";

          ExchangeSettingModel invalidNullExchangeVersionModel = GetRandomExchangeSettingModel();
          invalidNullExchangeVersionModel.ExchangeVersionModel = null;

          ExchangeSettingModel invalidNullExchangeLoginTypeModel = GetRandomExchangeSettingModel();
          invalidNullExchangeLoginTypeModel.ExchangeLoginTypeModel = null;

          ExchangeSettingModel invalidNullExchangeUrl = GetRandomExchangeSettingModel();
          invalidNullExchangeUrl.ExchangeUrl = null;

          ExchangeSettingModel invalidMaxlengthExchangeUrl = GetRandomExchangeSettingModel();
          invalidMaxlengthExchangeUrl.ExchangeUrl = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureTenant = GetRandomExchangeSettingModel();
          invalidNullAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureTenant.AzureTenant = "";

          ExchangeSettingModel invalidMaxlengthAzureTenant = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureTenant.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureTenant.AzureTenant = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientId = GetRandomExchangeSettingModel();
          invalidNullAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientId.AzureClientId = null;

          ExchangeSettingModel invalidMaxlengthAzureClientId = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientId.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientId.AzureClientId = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullAzureClientSecret = GetRandomExchangeSettingModel();
          invalidNullAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidNullAzureClientSecret.AzureClientSecret = null;

          ExchangeSettingModel invalidMaxlengthAzureClientSecret = GetRandomExchangeSettingModel();
          invalidMaxlengthAzureClientSecret.ExchangeLoginTypeModel.Type = ExchangeLoginEnumType.WebLogin;
          invalidMaxlengthAzureClientSecret.AzureClientSecret = AppendSpaceToString("This string greater than 250 character", 260);

          ExchangeSettingModel invalidNullEmaillAddress = GetRandomExchangeSettingModel();
          invalidNullEmaillAddress.EmailAddress = "";

          ExchangeSettingModel invalidFormatIncorrecEmailAddress = GetRandomExchangeSettingModel();
          invalidFormatIncorrecEmailAddress.EmailAddress = "abcdef@gmail..com";

          ExchangeSettingModel invalidMaxlengthEmailAddress = GetRandomExchangeSettingModel();
          invalidMaxlengthEmailAddress.EmailAddress = "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com";

          ExchangeSettingModel invalidNullServiceUser = GetRandomExchangeSettingModel();
          invalidNullServiceUser.ServiceUser = null;

          ExchangeSettingModel invalidMaxlengthServiceUser = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUser.ServiceUser = AppendSpaceToString("This string greater than 250 character", 55);

          ExchangeSettingModel invalidNullServiceUserPassword = GetRandomExchangeSettingModel();
          invalidNullServiceUserPassword.ServiceUserPassword = null;

          ExchangeSettingModel invalidMaxlengthServiceUserPassword = GetRandomExchangeSettingModel();
          invalidMaxlengthServiceUserPassword.ServiceUserPassword = AppendSpaceToString("This string greater than 250 character", 66);


          Dictionary<string, ExchangeSettingModel> invalidModels = new Dictionary<string, ExchangeSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidTenantIdInvalid), invalidTenantIdInvalid);
          invalidModels.Add(nameof(invalidNullName), invalidNullName);
          invalidModels.Add(nameof(invalidMaxlengthName), invalidMaxlengthName);
          invalidModels.Add(nameof(invalidNullExchangeVersionModel), invalidNullExchangeVersionModel);
          invalidModels.Add(nameof(invalidNullExchangeLoginTypeModel), invalidNullExchangeLoginTypeModel);
          invalidModels.Add(nameof(invalidNullExchangeUrl), invalidNullExchangeUrl);
          invalidModels.Add(nameof(invalidMaxlengthExchangeUrl), invalidMaxlengthExchangeUrl);
          invalidModels.Add(nameof(invalidNullAzureTenant), invalidNullAzureTenant);
          invalidModels.Add(nameof(invalidMaxlengthAzureTenant), invalidMaxlengthAzureTenant);
          invalidModels.Add(nameof(invalidNullAzureClientId), invalidNullAzureClientId);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientId), invalidMaxlengthAzureClientId);
          invalidModels.Add(nameof(invalidNullAzureClientSecret), invalidNullAzureClientSecret);
          invalidModels.Add(nameof(invalidMaxlengthAzureClientSecret), invalidMaxlengthAzureClientSecret);
          invalidModels.Add(nameof(invalidNullEmaillAddress), invalidNullEmaillAddress);
          invalidModels.Add(nameof(invalidFormatIncorrecEmailAddress), invalidFormatIncorrecEmailAddress);
          invalidModels.Add(nameof(invalidMaxlengthEmailAddress), invalidMaxlengthEmailAddress);
          invalidModels.Add(nameof(invalidNullServiceUser), invalidNullServiceUser);
          invalidModels.Add(nameof(invalidMaxlengthServiceUser), invalidMaxlengthServiceUser);
          invalidModels.Add(nameof(invalidNullServiceUserPassword), invalidNullServiceUserPassword);
          invalidModels.Add(nameof(invalidMaxlengthServiceUserPassword), invalidMaxlengthServiceUserPassword);

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _exchangeSettingService.CheckConnection(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.NO_DATA_ROLE, actualModel.Error.Type);
                Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdInvalid):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeVersionModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeLoginTypeModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidNullExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthExchangeUrl):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_TENANT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureTenant):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientId):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthAzureClientSecret):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmaillAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmailAddress):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUser):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthServiceUserPassword):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50, actualModel.Error.Message);
                break;

              default:
                Assert.Fail();
                break;
            }

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<ExchangeSettingModel>("CreateModel");
          if (createModel != null)
          {
            _exchangeSettingService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    #endregion

    #region private

    private ExchangeSettingModel GetRandomExchangeSettingModel()
    {
      var user = "user" + Guid.NewGuid().ToString().Replace("-", "");
      return new ExchangeSettingModel()
      {
        TenantId = _tenantModel.Id,
        Name = "Name_" + Guid.NewGuid().ToString(),
        ExchangeVersionModel = _exchangeVersionModels.Count > 0 ? _exchangeVersionModels[0] : null,
        ExchangeUrl = "ExchangeUrl_" + Guid.NewGuid().ToString(),
        ExchangeLoginTypeModel = _exchangeLoginTypeModels.Count > 0 ? _exchangeLoginTypeModels[0] : null,
        AzureClientId = "AzureClientId_" + Guid.NewGuid().ToString(),
        AzureClientSecret = "AzureClientSecret_" + Guid.NewGuid().ToString(),
        AzureTenant = "AzureTenant_" + Guid.NewGuid().ToString(),
        EmailAddress = user + "@kloon.vn",
        ServiceUser = "ServiceUser_" + Guid.NewGuid().ToString(),
        ServiceUserPassword = "" + Guid.NewGuid().ToString(),
        IsEnabled = false,
        Description = "Description_" + Guid.NewGuid().ToString(),
        HealthStatus = false
      };
    }

    private string AppendSpaceToString(string str, int numberOfLength)
    {
      StringBuilder sb = new StringBuilder();
      if ((numberOfLength - str.Length) > 0)
      {
        sb.Append(' ', (numberOfLength - str.Length));
      }
      sb.Append(str);
      return sb.ToString();
    }

    #endregion
  }
}
