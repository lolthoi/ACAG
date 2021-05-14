using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services
{
  [TestClass()]
  public class PayTypeService_Test : TestBase
  {
    #region Variable and Const

    private IPayTypeService _paytypeService;
    private ITenantService _tenantService;
    private IUserService _userService;

    private List<TenantModel> _tenantDatas = new List<TenantModel>();
    private List<PayTypeModel> _paytypeDatas = new List<PayTypeModel>();

    private TenantModel _tenantRandom;
    private UserModel _userRandom;
    private TenantModelForUser _tenantModelForUser;

    private const string userPassWord = "123456";
    private const int testDataPaytypeCode = 1000;
    private const string testDataPaytypeDisplayName = "TEST DATA DISPLAYNAME";
    private const string testDataTenantName = "TEST DATA TENANT RANDOM NAME";

    #endregion Variable and Const

    #region Init Enviroment,Repository, Services

    protected override void InitServices()
    {
      _paytypeService = _scope.ServiceProvider.GetService<IPayTypeService>();
      _tenantService = _scope.ServiceProvider.GetService<ITenantService>();
      _userService = _scope.ServiceProvider.GetService<IUserService>();
    }
    protected override void InitEnvirontment()
    {
      _testService.StartLoginWithAdmin()
       .ThenImplementTest(param =>
       {
         InitDataForUnitTest();
       });
    }
    protected override void CleanEnvirontment()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          _userService.Delete(_userRandom.Id);
          _tenantService.Delete(_tenantRandom.Id);
        });
    }

    #endregion Init Enviroment,Repository

    #region TEST FUNCTION CREATE

    [TestMethod()]
    public void AdminRoleCreate_ValidData_Success()
    {
      _testService
        .StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var expectedModel = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id).Data;
          Assert.IsNotNull(expectedModel);

          Assert.AreEqual(expectedModel.Id, model.Id);
          Assert.AreEqual(expectedModel.Code, model.Code);
          Assert.AreEqual(expectedModel.DisplayName, model.DisplayName);
          Assert.AreEqual(expectedModel.IsAppointmentAwayState, model.IsAppointmentAwayState);
          Assert.AreEqual(expectedModel.IsAppointmentPrivate, model.IsAppointmentPrivate);
          Assert.AreEqual(expectedModel.IsEnabled, model.IsEnabled);
          Assert.AreEqual(expectedModel.Id, model.Id);

          
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleCreate_ValidData_Success()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var expectedModel = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id).Data;
          Assert.IsNotNull(expectedModel);

          Assert.AreEqual(expectedModel.Id, model.Id);
          Assert.AreEqual(expectedModel.Code, model.Code);
          Assert.AreEqual(expectedModel.DisplayName, model.DisplayName);
          Assert.AreEqual(expectedModel.IsAppointmentAwayState, model.IsAppointmentAwayState);
          Assert.AreEqual(expectedModel.IsAppointmentPrivate, model.IsAppointmentPrivate);
          Assert.AreEqual(expectedModel.IsEnabled, model.IsEnabled);
          Assert.AreEqual(expectedModel.Id, model.Id);

          if (expectedModel != null && expectedModel.Id > 0)
            _paytypeService.Delete(expectedModel.TenantId, expectedModel.Id);
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }
    [TestMethod()]
    public void SysAdminRoleCreate_ValidData_Success()
    {
      _testService
        .StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var expectedModel = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id).Data;
          Assert.IsNotNull(expectedModel);

          Assert.AreEqual(expectedModel.Id, model.Id);
          Assert.AreEqual(expectedModel.Code, model.Code);
          Assert.AreEqual(expectedModel.DisplayName, model.DisplayName);
          Assert.AreEqual(expectedModel.IsAppointmentAwayState, model.IsAppointmentAwayState);
          Assert.AreEqual(expectedModel.IsAppointmentPrivate, model.IsAppointmentPrivate);
          Assert.AreEqual(expectedModel.IsEnabled, model.IsEnabled);
          Assert.AreEqual(expectedModel.Id, model.Id);


        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleCreate_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          PayTypeModel nullModel = null;

          var invalidMinCodeModel = PrepareValidPaytypeData(1);
          invalidMinCodeModel.Code = 0;

          var invalidMinDisplayNameModel = PrepareValidPaytypeData(2);
          invalidMinDisplayNameModel.DisplayName = "";

          var invalidMaxDisplayNameModel = PrepareValidPaytypeData(3);
          invalidMaxDisplayNameModel.DisplayName = AppendSpaceToString("This string greater than 250 character", 260);

          var anotherPaytype = PrepareValidPaytypeData(1);
          var createdAnotherPaytype = _paytypeService.Create(anotherPaytype);
          Assert.IsNotNull(createdAnotherPaytype.Data);
          param.CleanData.Add("anotherPaytype", anotherPaytype);

          var invalidExistingPaytypeCode = PrepareValidPaytypeData(4);
          invalidExistingPaytypeCode.Code = createdAnotherPaytype.Data.Code;

          Dictionary<string, PayTypeModel> invalidModels = new Dictionary<string, PayTypeModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidMinCodeModel), invalidMinCodeModel);
          invalidModels.Add(nameof(invalidMinDisplayNameModel), invalidMinDisplayNameModel);
          invalidModels.Add(nameof(invalidMaxDisplayNameModel), invalidMaxDisplayNameModel);
          invalidModels.Add(nameof(invalidExistingPaytypeCode), invalidExistingPaytypeCode);

          #endregion Init data

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _paytypeService.Create(invalidModel.Value); ;

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidMinCodeModel):
                Assert.AreEqual(LangKey.MSG_CODE_NUMBER_RANGE_IS_FROM_1_TO_99999, actualModel.Error.Message);
                break;
              case nameof(invalidMinDisplayNameModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DISPLAY_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxDisplayNameModel):
                Assert.AreEqual(LangKey.MAX_LENGTH_OF_DISPLAY_NAME_IS_250, actualModel.Error.Message);
                break;
              case nameof(invalidExistingPaytypeCode):
                Assert.AreEqual(LangKey.MSG_CODE_USED_FOR_ANOTHER_PAYTYPE, actualModel.Error.Message);
                break;
              default:
                Assert.Fail();
                break;
            }
          }

          #endregion Action
        })
        .ThenCleanDataTest(param =>
        {
          var actualModel = param.CleanData.Get<PayTypeModel>("anotherPaytype");

          if (actualModel != null)
          {
            _paytypeService.Delete(actualModel.TenantId, actualModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleCreate_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data

          PayTypeModel nullModel = null;

          var invalidMinCodeModel = PrepareValidPaytypeData(1);
          invalidMinCodeModel.Code = 0;

          var invalidMinDisplayNameModel = PrepareValidPaytypeData(2);
          invalidMinDisplayNameModel.DisplayName = "";

          var invalidMaxDisplayNameModel = PrepareValidPaytypeData(3);
          invalidMaxDisplayNameModel.DisplayName = AppendSpaceToString("This string greater than 250 character", 260);

          var invalidTenantIdModel = PrepareValidPaytypeData(1);
          invalidTenantIdModel.TenantId = GetInvalidTenantId();

          var anotherPaytype = PrepareValidPaytypeData(1);
          var createdAnotherPaytype = _paytypeService.Create(anotherPaytype);
          Assert.IsNotNull(createdAnotherPaytype.Data);
          param.CleanData.Add("anotherPaytype", anotherPaytype);

          var invalidExistingPaytypeCode = PrepareValidPaytypeData(4);
          invalidExistingPaytypeCode.Code = createdAnotherPaytype.Data.Code;

          Dictionary<string, PayTypeModel> invalidModels = new Dictionary<string, PayTypeModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidMinCodeModel), invalidMinCodeModel);
          invalidModels.Add(nameof(invalidMinDisplayNameModel), invalidMinDisplayNameModel);
          invalidModels.Add(nameof(invalidMaxDisplayNameModel), invalidMaxDisplayNameModel);
          invalidModels.Add(nameof(invalidTenantIdModel), invalidTenantIdModel);
          invalidModels.Add(nameof(invalidExistingPaytypeCode), invalidExistingPaytypeCode);

          #endregion Init data

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _paytypeService.Create(invalidModel.Value); ;

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidMinCodeModel):
                Assert.AreEqual(LangKey.MSG_CODE_NUMBER_RANGE_IS_FROM_1_TO_99999, actualModel.Error.Message);
                break;
              case nameof(invalidMinDisplayNameModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DISPLAY_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxDisplayNameModel):
                Assert.AreEqual(LangKey.MAX_LENGTH_OF_DISPLAY_NAME_IS_250, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdModel):
                Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidExistingPaytypeCode):
                Assert.AreEqual(LangKey.MSG_CODE_USED_FOR_ANOTHER_PAYTYPE, actualModel.Error.Message);
                break;
              default:
                Assert.Fail();
                break;
            }
          }

          #endregion Action
        })
        .ThenCleanDataTest(param =>
        {
          var actualModel = param.CleanData.Get<PayTypeModel>("anotherPaytype");

          if (actualModel != null)
          {
            _paytypeService.Delete(actualModel.TenantId, actualModel.Id);
          }
        });
    }
    [TestMethod()]
    public void SysAdminRoleCreate_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          PayTypeModel nullModel = null;

          var invalidMinCodeModel = PrepareValidPaytypeData(1);
          invalidMinCodeModel.Code = 0;

          var invalidMinDisplayNameModel = PrepareValidPaytypeData(2);
          invalidMinDisplayNameModel.DisplayName = "";

          var invalidMaxDisplayNameModel = PrepareValidPaytypeData(3);
          invalidMaxDisplayNameModel.DisplayName = AppendSpaceToString("This string greater than 250 character", 260);

          var anotherPaytype = PrepareValidPaytypeData(1);
          var createdAnotherPaytype = _paytypeService.Create(anotherPaytype);
          Assert.IsNotNull(createdAnotherPaytype.Data);
          param.CleanData.Add("anotherPaytype", anotherPaytype);

          var invalidExistingPaytypeCode = PrepareValidPaytypeData(4);
          invalidExistingPaytypeCode.Code = createdAnotherPaytype.Data.Code;

          Dictionary<string, PayTypeModel> invalidModels = new Dictionary<string, PayTypeModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidMinCodeModel), invalidMinCodeModel);
          invalidModels.Add(nameof(invalidMinDisplayNameModel), invalidMinDisplayNameModel);
          invalidModels.Add(nameof(invalidMaxDisplayNameModel), invalidMaxDisplayNameModel);
          invalidModels.Add(nameof(invalidExistingPaytypeCode), invalidExistingPaytypeCode);

          #endregion Init data

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _paytypeService.Create(invalidModel.Value); ;

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidMinCodeModel):
                Assert.AreEqual(LangKey.MSG_CODE_NUMBER_RANGE_IS_FROM_1_TO_99999, actualModel.Error.Message);
                break;
              case nameof(invalidMinDisplayNameModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DISPLAY_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxDisplayNameModel):
                Assert.AreEqual(LangKey.MAX_LENGTH_OF_DISPLAY_NAME_IS_250, actualModel.Error.Message);
                break;
              case nameof(invalidExistingPaytypeCode):
                Assert.AreEqual(LangKey.MSG_CODE_USED_FOR_ANOTHER_PAYTYPE, actualModel.Error.Message);
                break;
              default:
                Assert.Fail();
                break;
            }
          }

          #endregion Action
        })
        .ThenCleanDataTest(param =>
        {
          var actualModel = param.CleanData.Get<PayTypeModel>("anotherPaytype");

          if (actualModel != null)
          {
            _paytypeService.Delete(actualModel.TenantId, actualModel.Id);
          }
        });
    }

    #endregion TEST FUNCTION CREATE

    #region TEST FUNCTION GETALL

    [TestMethod()]
    public void AdminRoleGetAll_ValidDataWithEmptySearchText_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          string searchText = "";
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          #endregion Init data

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var beforeListPayType = _paytypeDatas.Where(x => x.TenantId == createdModel.Data.TenantId).ToList();
          var searchResult = _paytypeService.GetAll(createdModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.PayTypeModels.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(beforeListPayType.Count() + 1, searchResult.Data.PayTypeModels.Count());
          Assert.AreEqual(createdModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, tenantCreatedInSearchResult.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, tenantCreatedInSearchResult.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, tenantCreatedInSearchResult.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, tenantCreatedInSearchResult.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetAll_ValidDataWithValidSearchText_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);

          #endregion Init data

          string searchText = model.DisplayName;
          var beforeListPaytype = _paytypeDatas.Where(x => x.DisplayName == searchText).ToList();
          var searchResult = _paytypeService.GetAll(model.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.PayTypeModels.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(searchResult.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(beforeListPaytype.Count() + 1, searchResult.Data.PayTypeModels.Count());
          Assert.AreEqual(createdModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, tenantCreatedInSearchResult.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, tenantCreatedInSearchResult.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, tenantCreatedInSearchResult.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, tenantCreatedInSearchResult.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

          if (createdModel.Data != null && createdModel.Data.Id > 0)
            _paytypeService.Delete(createdModel.Data.TenantId, createdModel.Data.Id);
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetAll_InvalidTenantIdAndSearchTextNull_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          var notExistTenantId = GetInvalidTenantId();

          var searchResult = _paytypeService.GetAll(notExistTenantId, null);

          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, searchResult.Error.Message);
        }).ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleGetAll_ValidDataWithEmptySearchText_Success()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data
          string searchText = "";
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          #endregion Init data

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var beforeListPayType = _paytypeDatas.Where(x => x.TenantId == createdModel.Data.TenantId).ToList();
          var searchResult = _paytypeService.GetAll(createdModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.PayTypeModels.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(beforeListPayType.Count() + 1, searchResult.Data.PayTypeModels.Count());
          Assert.AreEqual(createdModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, tenantCreatedInSearchResult.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, tenantCreatedInSearchResult.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, tenantCreatedInSearchResult.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, tenantCreatedInSearchResult.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetAll_ValidDataWithValidSearchText_Success()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);

          #endregion Init data

          string searchText = model.DisplayName;
          var beforeListPaytype = _paytypeDatas.Where(x => x.DisplayName == searchText).ToList();
          var searchResult = _paytypeService.GetAll(model.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.PayTypeModels.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(searchResult.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(beforeListPaytype.Count() + 1, searchResult.Data.PayTypeModels.Count());
          Assert.AreEqual(createdModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, tenantCreatedInSearchResult.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, tenantCreatedInSearchResult.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, tenantCreatedInSearchResult.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, tenantCreatedInSearchResult.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetAll_InvalidTenantIdAndSearchTextNull_Fail()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          var notExistTenantId = GetInvalidTenantId();

          var searchResult = _paytypeService.GetAll(notExistTenantId, null);

          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, searchResult.Error.Message);
        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod()]
    public void UserRoleGetAll_InvalidTenantId_Fail()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          int invalidTenantId = GetInvalidTenantId();

          var searchResult = _paytypeService.GetAll(invalidTenantId, "");

          Assert.IsNotNull(searchResult);
          Assert.IsNull(searchResult.Data);
          Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, searchResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void SysAdminRoleGetAll_ValidDataWithEmptySearchText_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          string searchText = "";
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          #endregion Init data

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var beforeListPayType = _paytypeDatas.Where(x => x.TenantId == createdModel.Data.TenantId).ToList();
          var searchResult = _paytypeService.GetAll(createdModel.Data.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.PayTypeModels.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(searchResult.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(beforeListPayType.Count() + 1, searchResult.Data.PayTypeModels.Count());
          Assert.AreEqual(createdModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, tenantCreatedInSearchResult.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, tenantCreatedInSearchResult.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, tenantCreatedInSearchResult.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, tenantCreatedInSearchResult.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetAll_ValidDataWithValidSearchText_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);

          #endregion Init data

          string searchText = model.DisplayName;
          var beforeListPaytype = _paytypeDatas.Where(x => x.DisplayName == searchText).ToList();
          var searchResult = _paytypeService.GetAll(model.TenantId, searchText);
          var tenantCreatedInSearchResult = searchResult.Data.PayTypeModels.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(searchResult.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);
          Assert.IsNotNull(tenantCreatedInSearchResult);
          Assert.AreEqual(beforeListPaytype.Count() + 1, searchResult.Data.PayTypeModels.Count());
          Assert.AreEqual(createdModel.Data.Id, tenantCreatedInSearchResult.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, tenantCreatedInSearchResult.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, tenantCreatedInSearchResult.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, tenantCreatedInSearchResult.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, tenantCreatedInSearchResult.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, tenantCreatedInSearchResult.IsEnabled);

          if (createdModel.Data != null && createdModel.Data.Id > 0)
            _paytypeService.Delete(createdModel.Data.TenantId, createdModel.Data.Id);
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetAll_InvalidTenantIdAndSearchTextNull_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          var notExistTenantId = GetInvalidTenantId();

          var searchResult = _paytypeService.GetAll(notExistTenantId, null);

          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, searchResult.Error.Message);
        }).ThenCleanDataTest(param => { });
    }

    #endregion TEST FUNCTION GETALL

    #region TEST FUNCTION UPDATE

    [TestMethod()]
    public void AdminRoleUpdate_ValidData_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          #endregion Init data
          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          createdModel.Data.DisplayName = "Some random name for testing";
          var updateModel = _paytypeService.Update(createdModel.Data);

          Assert.IsNotNull(updateModel.Data);
          Assert.AreEqual(createdModel.Data.Id, updateModel.Data.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, updateModel.Data.DisplayName);
          Assert.AreEqual(createdModel.Data.IsEnabled, updateModel.Data.IsEnabled);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, updateModel.Data.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, updateModel.Data.IsAppointmentPrivate);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleUpdate_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var model2 = PrepareValidPaytypeData(2);
          var anotherPaytype = _paytypeService.Create(model2);
          Assert.IsNotNull(anotherPaytype.Data);
          param.CleanData.Add("AnotherModel", anotherPaytype.Data);


          PayTypeModel nullModel = null;
          var invalidMinCodeModel = PrepareValidPaytypeData(3);
          invalidMinCodeModel.Code = 0;

          var invalidMinDisplayNameModel = PrepareValidPaytypeData(4);
          invalidMinDisplayNameModel.DisplayName = "";

          var invalidMaxDisplayNameModel = PrepareValidPaytypeData(5);
          invalidMaxDisplayNameModel.DisplayName = AppendSpaceToString("This string greater than 250 character", 260);

          var invalidPayTypeIdModel = PrepareValidPaytypeData(6);
          invalidPayTypeIdModel.Id = GetInvalidPaytypeId();

          var invalidExistingCodeModel = PrepareValidPaytypeData(7);
          invalidExistingCodeModel.Id = anotherPaytype.Data.Id;
          invalidExistingCodeModel.TenantId = createdModel.Data.TenantId;
          invalidExistingCodeModel.Code = createdModel.Data.Code;


          Dictionary<string, PayTypeModel> invalidModels = new Dictionary<string, PayTypeModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidMinCodeModel), invalidMinCodeModel);
          invalidModels.Add(nameof(invalidMinDisplayNameModel), invalidMinDisplayNameModel);
          invalidModels.Add(nameof(invalidMaxDisplayNameModel), invalidMaxDisplayNameModel);
          invalidModels.Add(nameof(invalidPayTypeIdModel), invalidPayTypeIdModel);
          invalidModels.Add(nameof(invalidExistingCodeModel), invalidExistingCodeModel);

          #endregion Init data

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _paytypeService.Update(invalidModel.Value); ;

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidMinCodeModel):
                Assert.AreEqual(LangKey.MSG_CODE_NUMBER_RANGE_IS_FROM_1_TO_99999, actualModel.Error.Message);
                break;
              case nameof(invalidMinDisplayNameModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DISPLAY_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxDisplayNameModel):
                Assert.AreEqual(LangKey.MAX_LENGTH_OF_DISPLAY_NAME_IS_250, actualModel.Error.Message);
                break;
              case nameof(invalidPayTypeIdModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidExistingCodeModel):
                Assert.AreEqual(LangKey.MSG_THIS_PAYTYPE_CODE_IS_USED_FOR_ANOTHER_PAYTYPE, actualModel.Error.Message);
                break;
              default:
                Assert.Fail();
                break;
            }
          }

          #endregion Action
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }

          var anotherModel = param.CleanData.Get<PayTypeModel>("AnotherModel");
          if (anotherModel != null)
          {
            _paytypeService.Delete(anotherModel.TenantId, anotherModel.Id);
          }

        });
    }

    [TestMethod()]
    public void UserRoleUpdate_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var model2 = PrepareValidPaytypeData(2);
          var anotherPaytype = _paytypeService.Create(model2);
          Assert.IsNotNull(anotherPaytype.Data);
          param.CleanData.Add("AnotherModel", anotherPaytype.Data);


          PayTypeModel nullModel = null;

          var invalidMinCodeModel = PrepareValidPaytypeData(3);
          invalidMinCodeModel.Code = 0;

          var invalidMinDisplayNameModel = PrepareValidPaytypeData(4);
          invalidMinDisplayNameModel.DisplayName = "";

          var invalidMaxDisplayNameModel = PrepareValidPaytypeData(5);
          invalidMaxDisplayNameModel.DisplayName = AppendSpaceToString("This string greater than 250 character", 260);

          var invalidPayTypeIdModel = PrepareValidPaytypeData(6);
          invalidPayTypeIdModel.Id = GetInvalidPaytypeId();

          var invalidExistingCodeModel = PrepareValidPaytypeData(7);
          invalidExistingCodeModel.Id = anotherPaytype.Data.Id;
          invalidExistingCodeModel.TenantId = createdModel.Data.TenantId;
          invalidExistingCodeModel.Code = createdModel.Data.Code;

          var invalidTenantIdModel = PrepareValidPaytypeData(8);
          invalidTenantIdModel.TenantId = GetInvalidTenantId();

          Dictionary<string, PayTypeModel> invalidModels = new Dictionary<string, PayTypeModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidMinCodeModel), invalidMinCodeModel);
          invalidModels.Add(nameof(invalidMinDisplayNameModel), invalidMinDisplayNameModel);
          invalidModels.Add(nameof(invalidMaxDisplayNameModel), invalidMaxDisplayNameModel);
          invalidModels.Add(nameof(invalidPayTypeIdModel), invalidPayTypeIdModel);
          invalidModels.Add(nameof(invalidExistingCodeModel), invalidExistingCodeModel);
          invalidModels.Add(nameof(invalidTenantIdModel), invalidTenantIdModel);

          #endregion Init data

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _paytypeService.Update(invalidModel.Value); ;

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidMinCodeModel):
                Assert.AreEqual(LangKey.MSG_CODE_NUMBER_RANGE_IS_FROM_1_TO_99999, actualModel.Error.Message);
                break;
              case nameof(invalidMinDisplayNameModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DISPLAY_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxDisplayNameModel):
                Assert.AreEqual(LangKey.MAX_LENGTH_OF_DISPLAY_NAME_IS_250, actualModel.Error.Message);
                break;
              case nameof(invalidPayTypeIdModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidExistingCodeModel):
                Assert.AreEqual(LangKey.MSG_THIS_PAYTYPE_CODE_IS_USED_FOR_ANOTHER_PAYTYPE, actualModel.Error.Message);
                break;
              case nameof(invalidTenantIdModel):
                Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Error.Message);
                break;
              default:
                Assert.Fail();
                break;
            }
          }

          #endregion Action
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }

          var anotherModel = param.CleanData.Get<PayTypeModel>("AnotherModel");
          if (anotherModel != null)
          {
            _paytypeService.Delete(anotherModel.TenantId, anotherModel.Id);
          }

        });
    }

    [TestMethod()]
    public void UserRoleUpdate_ValidData_Success()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);

          #endregion Init data

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          createdModel.Data.DisplayName = "Some random name for testing";
          var updateModel = _paytypeService.Update(createdModel.Data);

          Assert.IsNotNull(updateModel.Data);
          Assert.AreEqual(createdModel.Data.Id, updateModel.Data.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, updateModel.Data.DisplayName);
          Assert.AreEqual(createdModel.Data.IsEnabled, updateModel.Data.IsEnabled);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, updateModel.Data.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, updateModel.Data.IsAppointmentPrivate);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleUpdate_ValidData_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          #endregion Init data
          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          createdModel.Data.DisplayName = "Some random name for testing";
          var updateModel = _paytypeService.Update(createdModel.Data);

          Assert.IsNotNull(updateModel.Data);
          Assert.AreEqual(createdModel.Data.Id, updateModel.Data.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, updateModel.Data.DisplayName);
          Assert.AreEqual(createdModel.Data.IsEnabled, updateModel.Data.IsEnabled);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, updateModel.Data.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, updateModel.Data.IsAppointmentPrivate);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleUpdate_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var model2 = PrepareValidPaytypeData(2);
          var anotherPaytype = _paytypeService.Create(model2);
          Assert.IsNotNull(anotherPaytype.Data);
          param.CleanData.Add("AnotherModel", anotherPaytype.Data);


          PayTypeModel nullModel = null;
          var invalidMinCodeModel = PrepareValidPaytypeData(3);
          invalidMinCodeModel.Code = 0;

          var invalidMinDisplayNameModel = PrepareValidPaytypeData(4);
          invalidMinDisplayNameModel.DisplayName = "";

          var invalidMaxDisplayNameModel = PrepareValidPaytypeData(5);
          invalidMaxDisplayNameModel.DisplayName = AppendSpaceToString("This string greater than 250 character", 260);

          var invalidPayTypeIdModel = PrepareValidPaytypeData(6);
          invalidPayTypeIdModel.Id = GetInvalidPaytypeId();

          var invalidExistingCodeModel = PrepareValidPaytypeData(7);
          invalidExistingCodeModel.Id = anotherPaytype.Data.Id;
          invalidExistingCodeModel.TenantId = createdModel.Data.TenantId;
          invalidExistingCodeModel.Code = createdModel.Data.Code;


          Dictionary<string, PayTypeModel> invalidModels = new Dictionary<string, PayTypeModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(invalidMinCodeModel), invalidMinCodeModel);
          invalidModels.Add(nameof(invalidMinDisplayNameModel), invalidMinDisplayNameModel);
          invalidModels.Add(nameof(invalidMaxDisplayNameModel), invalidMaxDisplayNameModel);
          invalidModels.Add(nameof(invalidPayTypeIdModel), invalidPayTypeIdModel);
          invalidModels.Add(nameof(invalidExistingCodeModel), invalidExistingCodeModel);

          #endregion Init data

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _paytypeService.Update(invalidModel.Value); ;

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidMinCodeModel):
                Assert.AreEqual(LangKey.MSG_CODE_NUMBER_RANGE_IS_FROM_1_TO_99999, actualModel.Error.Message);
                break;
              case nameof(invalidMinDisplayNameModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DISPLAY_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxDisplayNameModel):
                Assert.AreEqual(LangKey.MAX_LENGTH_OF_DISPLAY_NAME_IS_250, actualModel.Error.Message);
                break;
              case nameof(invalidPayTypeIdModel):
                Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, actualModel.Error.Message);
                break;
              case nameof(invalidExistingCodeModel):
                Assert.AreEqual(LangKey.MSG_THIS_PAYTYPE_CODE_IS_USED_FOR_ANOTHER_PAYTYPE, actualModel.Error.Message);
                break;
              default:
                Assert.Fail();
                break;
            }
          }

          #endregion Action
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }

          var anotherModel = param.CleanData.Get<PayTypeModel>("AnotherModel");
          if (anotherModel != null)
          {
            _paytypeService.Delete(anotherModel.TenantId, anotherModel.Id);
          }

        });
    }


    #endregion TEST FUNCTION UPDATE

    #region TEST FUNCTION DELETE

    [TestMethod()]
    public void AdminRoleDelete_ValidData_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          Assert.IsNotNull(createdModel.Data);

          var deleteResult = _paytypeService.Delete(createdModel.Data.TenantId, createdModel.Data.Id);
          #endregion Init data

          Assert.IsNotNull(deleteResult.Data);
          Assert.AreEqual(true, deleteResult.Data);

          var checkExist = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, checkExist.Error.Message);

        }).ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void AdminRoleDelete_InvalidPaytypeId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          var notExistPaytypeId = GetInvalidPaytypeId();
          var deleteResult = _paytypeService.Delete(_tenantRandom.Id, notExistPaytypeId);
          #endregion Init data

          Assert.IsNotNull(deleteResult.Error);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, deleteResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleDelete_ValidData_Success()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          Assert.IsNotNull(createdModel.Data);

          var deleteResult = _paytypeService.Delete(createdModel.Data.TenantId, createdModel.Data.Id);
          #endregion Init data

          Assert.IsNotNull(deleteResult.Data);
          Assert.AreEqual(true, deleteResult.Data);

          var checkExist = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, checkExist.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleDelete_InvalidPaytypeId_Fail()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data
          var notExistPaytypeId = GetInvalidPaytypeId();
          var deleteResult = _paytypeService.Delete(_tenantRandom.Id, notExistPaytypeId);
          #endregion Init data

          Assert.IsNotNull(deleteResult.Error);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, deleteResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void SysAdminRoleDelete_ValidData_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          var model = PrepareValidPaytypeData(1);
          var createdModel = _paytypeService.Create(model);
          Assert.IsNotNull(createdModel.Data);

          var deleteResult = _paytypeService.Delete(createdModel.Data.TenantId, createdModel.Data.Id);
          #endregion Init data

          Assert.IsNotNull(deleteResult.Data);
          Assert.AreEqual(true, deleteResult.Data);

          var checkExist = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, checkExist.Error.Message);
        }).ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void SysAdminRoleDelete_InvalidPaytypeId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          var notExistPaytypeId = GetInvalidPaytypeId();
          var deleteResult = _paytypeService.Delete(_tenantRandom.Id, notExistPaytypeId);
          #endregion Init data

          Assert.IsNotNull(deleteResult.Error);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, deleteResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    #endregion TEST FUNCTION DELETE

    #region TEST FUNCTION GETBYID

    [TestMethod()]
    public void AdminRoleGetById_ValidData_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          var model = PrepareValidPaytypeData(2);
          var createdModel = _paytypeService.Create(model);
          #endregion Init data

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var searchResult = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.IsNotNull(searchResult.Data);
          Assert.AreEqual(createdModel.Data.Id, searchResult.Data.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, searchResult.Data.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, searchResult.Data.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, searchResult.Data.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, searchResult.Data.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, searchResult.Data.IsEnabled);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetById_InvalidPaytypeId_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          int notExistPaytypeId = GetInvalidPaytypeId();
          #endregion Init data

          var searchResult = _paytypeService.GetById(_tenantRandom.Id, notExistPaytypeId);
          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST,searchResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleGetById_ValidData_Success()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data

          var model = PrepareValidPaytypeData(2);
          var createdModel = _paytypeService.Create(model);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion Init data

          Assert.IsNotNull(createdModel.Data);
          var searchResult = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.IsNotNull(searchResult.Data);
          Assert.AreEqual(createdModel.Data.Id, searchResult.Data.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, searchResult.Data.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, searchResult.Data.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, searchResult.Data.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, searchResult.Data.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, searchResult.Data.IsEnabled);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetById_InvalidPaytypeId_Success()
    {
      _testService.StartLoginWithUser(_userRandom.Email, userPassWord)
        .ThenImplementTest(param =>
        {
          #region Init data
          int notExistPaytypeId = GetInvalidPaytypeId();
          #endregion Init data

          var searchResult = _paytypeService.GetById(_tenantRandom.Id, notExistPaytypeId);
          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, searchResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void SysAdminRoleGetById_ValidData_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          var model = PrepareValidPaytypeData(2);
          var createdModel = _paytypeService.Create(model);
          #endregion Init data

          Assert.IsNotNull(createdModel.Data);
          param.CleanData.Add("CreateModel", createdModel.Data);

          var searchResult = _paytypeService.GetById(createdModel.Data.TenantId, createdModel.Data.Id);
          Assert.IsNotNull(searchResult.Data);
          Assert.AreEqual(createdModel.Data.Id, searchResult.Data.Id);
          Assert.AreEqual(createdModel.Data.DisplayName, searchResult.Data.DisplayName);
          Assert.AreEqual(createdModel.Data.Code, searchResult.Data.Code);
          Assert.AreEqual(createdModel.Data.IsAppointmentAwayState, searchResult.Data.IsAppointmentAwayState);
          Assert.AreEqual(createdModel.Data.IsAppointmentPrivate, searchResult.Data.IsAppointmentPrivate);
          Assert.AreEqual(createdModel.Data.IsEnabled, searchResult.Data.IsEnabled);

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<PayTypeModel>("CreateModel");
          if (createModel != null)
          {
            _paytypeService.Delete(createModel.TenantId, createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetById_InvalidPaytypeId_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data
          int notExistPaytypeId = GetInvalidPaytypeId();
          #endregion Init data

          var searchResult = _paytypeService.GetById(_tenantRandom.Id, notExistPaytypeId);
          Assert.IsNull(searchResult.Data);
          Assert.IsNotNull(searchResult.Error);
          Assert.AreEqual(LangKey.MSG_PAYTYPE_DOESNT_EXIST, searchResult.Error.Message);
        })
        .ThenCleanDataTest(param => { });
    }

    #endregion TEST FUNCTION GETBYID

    #region Private Method

    public PayTypeModel PrepareValidPaytypeData(int counter)
    {
      var randomTenant = _tenantRandom;

      PayTypeModel newData = new PayTypeModel();
      newData.TenantId = randomTenant.Id;
      newData.Code = GetRandomInt();
      newData.DisplayName = testDataPaytypeDisplayName + counter.ToString();
      newData.IsAppointmentAwayState = true;
      newData.IsAppointmentPrivate = true;
      newData.IsEnabled = true;

      return newData;
    }

    public TenantModel PrepareValidTenantData(int counter)
    {
      TenantModel newData = new TenantModel();
      newData.Name = testDataTenantName + (counter + GetRandomInt()).ToString();
      newData.Description = testDataTenantName + (counter + GetRandomInt()).ToString();
      newData.Number = new Random().Next(1, 9000);
      newData.ScheduleTimer = 20;
      newData.IsEnabled = true;
      return newData;
    }

    private int GetInvalidTenantId()
    {
      var actualBiggestTenantId = 0;
      if (_tenantDatas.Count > 0)
      {
        actualBiggestTenantId = _tenantDatas.Max(x => x.Id);
      }
      Random random = new Random();
      int result = actualBiggestTenantId + random.Next(1, 1000);
      return result;
    }

    private int GetInvalidPaytypeId()
    {
      var actualBiggestPaytypeId = 0;
      if (_paytypeDatas.Count > 0)
      {
        actualBiggestPaytypeId = _paytypeDatas.Max(x => x.Id);
      }
      Random random = new Random();
      int result = actualBiggestPaytypeId + random.Next(1, 1000);
      return result;
    }

    private int GetRandomInt()
    {
      Random rd = new Random();
      return rd.Next(1, 99999);
    }

    private void InitDataForUnitTest()
    {
      _tenantRandom = PrepareValidTenantData(1);
      var tenantResult = _tenantService.Create(_tenantRandom);
      _tenantModelForUser = new TenantModelForUser()
      {
        Id = _tenantRandom.Id,
        Name = _tenantRandom.Name
      };

      _userRandom = GetRandomUser(_tenantModelForUser);
      var userResult = _userService.Create(_userRandom, "123456");

      _tenantDatas = _tenantService.GetAll().Data.Tenants;
    }

    public string AppendSpaceToString(string str, int numberOfLength)
    {
      StringBuilder sb = new StringBuilder();
      if ((numberOfLength - str.Length) > 0)
      {
        sb.Append(' ', (numberOfLength - str.Length));
      }
      sb.Append(str);
      return sb.ToString();
    }

    #endregion Private Method
  }
}

public static class ExtensionMethods
{
  public static T DeepClone<T>(this T a)
  {
    using (MemoryStream stream = new MemoryStream())
    {
      BinaryFormatter formatter = new BinaryFormatter();
      formatter.Serialize(stream, a);
      stream.Position = 0;
      return (T)formatter.Deserialize(stream);
    }
  }
}