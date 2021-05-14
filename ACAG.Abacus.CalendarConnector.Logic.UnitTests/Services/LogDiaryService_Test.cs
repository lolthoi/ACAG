using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services
{
  [TestClass()]
  public class LogDiaryService_Test : TestBase
  {
    private ITenantService _tenantService;
    private IUserService _userService;
    private ILogDiaryService _logDiaryService;

    private TenantModel _tenantModel;
    private TenantModelForUser _tenantModelForUser; 
    private UserModel _userModel;

    protected override void InitServices()
    {
      _tenantService = _scope.ServiceProvider.GetService<ITenantService>();
      _userService = _scope.ServiceProvider.GetService<IUserService>();
      _logDiaryService = _scope.ServiceProvider.GetService<ILogDiaryService>();
    }

    protected override void InitEnvirontment()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(currentUser =>
        {
          
          _tenantModel = MakeTenantModel();
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

    #region GetByTenant(int tenantId)

    [TestMethod()]
    public void SysAdminRoleGetByTenant_WithExistingTenant_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          var actualResult = _logDiaryService.GetByTenant(createdTenantModel.Data.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.IsTrue(actualResult.Data.Count == 1);
          Assert.AreEqual(createdLogDiaryModel.Data.Id, actualResult.Data[0].Id);
          Assert.AreEqual(createdLogDiaryModel.Data.Data, actualResult.Data[0].Data);
          Assert.AreEqual(createdLogDiaryModel.Data.Error, actualResult.Data[0].Error);
          Assert.AreEqual(createdLogDiaryModel.Data.IsEnabled, actualResult.Data[0].IsEnabled);
          Assert.AreEqual(createdLogDiaryModel.Data.TenantId, actualResult.Data[0].TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetByTenant_WithExistingTenant_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          var actualResult = _logDiaryService.GetByTenant(createdTenantModel.Data.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.IsTrue(actualResult.Data.Count == 1);
          Assert.AreEqual(createdLogDiaryModel.Data.Id, actualResult.Data[0].Id);
          Assert.AreEqual(createdLogDiaryModel.Data.Data, actualResult.Data[0].Data);
          Assert.AreEqual(createdLogDiaryModel.Data.Error, actualResult.Data[0].Error);
          Assert.AreEqual(createdLogDiaryModel.Data.IsEnabled, actualResult.Data[0].IsEnabled);
          Assert.AreEqual(createdLogDiaryModel.Data.TenantId, actualResult.Data[0].TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetByTenant_WithExistingTenant_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var logDiaryModel = MakeLogDiaryModel(_tenantModel.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          var actualResult = _logDiaryService.GetByTenant(_tenantModel.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.IsTrue(actualResult.Data.Count == 1);
          Assert.AreEqual(createdLogDiaryModel.Data.Id, actualResult.Data[0].Id);
          Assert.AreEqual(createdLogDiaryModel.Data.Data, actualResult.Data[0].Data);
          Assert.AreEqual(createdLogDiaryModel.Data.Error, actualResult.Data[0].Error);
          Assert.AreEqual(createdLogDiaryModel.Data.IsEnabled, actualResult.Data[0].IsEnabled);
          Assert.AreEqual(createdLogDiaryModel.Data.TenantId, actualResult.Data[0].TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(_tenantModel.Id, listIds);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetByTenant_WithInvalidTenantId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(currentUser =>
        {
          var actualResult = _logDiaryService.GetByTenant(0);

          Assert.IsNull(actualResult.Data);
          Assert.IsNotNull(actualResult.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualResult.Error.Message);
        });
    }

    [TestMethod()]
    public void AdminRoleGetByTenant_WithInvalidTenantId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(currentUser =>
        {
          var actualResult = _logDiaryService.GetByTenant(0);

          Assert.IsNull(actualResult.Data);
          Assert.IsNotNull(actualResult.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualResult.Error.Message);
        });
    }

    [TestMethod()]
    public void UserRoleGetByTenant_WithInvalidTenantId_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(currentUser =>
        {
          var testingResult = _logDiaryService.GetByTenant(-1);

          Assert.IsNull(testingResult.Data);
          Assert.IsNotNull(testingResult.Error);
          Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, testingResult.Error.Message);
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetByTenant_WithNonExistingTenant_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init

          var tenent = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenent);

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          #endregion

          #region Arrange

          _tenantService.Delete(createdTenantModel.Data.Id);

          var actualResult = _logDiaryService.GetByTenant(createdTenantModel.Data.Id);

          Assert.IsNull(actualResult.Data);
          Assert.IsNotNull(actualResult.Error);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, actualResult.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetByTenant_WithNonExistingTenant_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init

          var tenent = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenent);

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          #endregion

          #region Arrange

          _tenantService.Delete(createdTenantModel.Data.Id);

          var actualResult = _logDiaryService.GetByTenant(createdTenantModel.Data.Id);

          Assert.IsNull(actualResult.Data);
          Assert.IsNotNull(actualResult.Error);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, actualResult.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetByTenant_WithNonExistingTenant_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(currentUser =>
        {
          var actualResult = _logDiaryService.GetByTenant(Int32.MaxValue);

          Assert.IsNull(actualResult.Data);
          Assert.IsNotNull(actualResult.Error);
          Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualResult.Error.Message);
        });
    }

    #endregion

    #region Create(LogDiaryModel logDiaryModel)

    [TestMethod()]
    public void SysAdminRoleCreate_CorrectModel_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var expectedModel = MakeLogDiaryModel(createdTenantModel.Data.Id);

          #endregion

          #region Arrange

          var actualModel = _logDiaryService.Create(expectedModel);

          param.CleanData.Add("actualModel", actualModel.Data);

          Assert.AreEqual(expectedModel.DateTime, actualModel.Data.DateTime);
          Assert.AreEqual(expectedModel.Data, actualModel.Data.Data);
          Assert.AreEqual(expectedModel.Error, actualModel.Data.Error);
          Assert.AreEqual(expectedModel.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(expectedModel.TenantId, actualModel.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var actualModel = param.CleanData.Get<LogDiaryModel>("actualModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (actualModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(actualModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleCreate_CorrectModel_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var expectedModel = MakeLogDiaryModel(createdTenantModel.Data.Id);

          #endregion

          #region Arrange

          var actualModel = _logDiaryService.Create(expectedModel);

          param.CleanData.Add("actualModel", actualModel.Data);

          Assert.AreEqual(expectedModel.DateTime, actualModel.Data.DateTime);
          Assert.AreEqual(expectedModel.Data, actualModel.Data.Data);
          Assert.AreEqual(expectedModel.Error, actualModel.Data.Error);
          Assert.AreEqual(expectedModel.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(expectedModel.TenantId, actualModel.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var actualModel = param.CleanData.Get<LogDiaryModel>("actualModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (actualModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(actualModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }
        });
    }

    [TestMethod()]
    public void UserRoleCreate_CorrectModel_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var expectedModel = MakeLogDiaryModel(_tenantModel.Id);

          #endregion

          #region Arrange

          var actualModel = _logDiaryService.Create(expectedModel);

          param.CleanData.Add("actualModel", actualModel.Data);

          Assert.AreEqual(expectedModel.DateTime, actualModel.Data.DateTime);
          Assert.AreEqual(expectedModel.Data, actualModel.Data.Data);
          Assert.AreEqual(expectedModel.Error, actualModel.Data.Error);
          Assert.AreEqual(expectedModel.IsEnabled, actualModel.Data.IsEnabled);
          Assert.AreEqual(expectedModel.TenantId, actualModel.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var actualModel = param.CleanData.Get<LogDiaryModel>("actualModel");

          if (actualModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(actualModel.Id);

            _logDiaryService.DisableRange(actualModel.Id, listIds);
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

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          #region Prepare invalid models

          LogDiaryModel nullModel = null;

          var nullDateTimeModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidDateTime: true,
            isNull: true);

          var nullDataModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidData: true,
            isNull: true);

          var emptyDataModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidData: true,
            isEmpty: true);

          var nullErrorModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidError: true,
            isNull: true);

          var emptyErrorModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidError: true,
            isEmpty: true);

          #endregion

          Dictionary<string, LogDiaryModel> invalidModels = new Dictionary<string, LogDiaryModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(nullDateTimeModel), nullDateTimeModel);
          invalidModels.Add(nameof(nullDataModel), nullDataModel);
          invalidModels.Add(nameof(emptyDataModel), emptyDataModel);
          invalidModels.Add(nameof(nullErrorModel), nullErrorModel);
          invalidModels.Add(nameof(emptyErrorModel), emptyErrorModel);

          #endregion

          #region Arrange

          Dictionary<string, ResultModel<LogDiaryModel>> actualModels = new Dictionary<string, ResultModel<LogDiaryModel>>();

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _logDiaryService.Create(invalidModel.Value);
            actualModels.Add(invalidModel.Key, actualModel);
          }

          foreach (var actualModel in actualModels)
          {
            Assert.IsNull(actualModel.Value.Data);
            Assert.IsNotNull(actualModel.Value.Error);

            switch (actualModel.Value.Error.Message)
            {
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullDateTimeModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullDataModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(emptyDataModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullErrorModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(emptyErrorModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
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
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
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

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          #region Prepare invalid models

          LogDiaryModel nullModel = null;

          var nullDateTimeModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidDateTime: true,
            isNull: true);

          var nullDataModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidData: true,
            isNull: true);

          var emptyDataModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidData: true,
            isEmpty: true);

          var nullErrorModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidError: true,
            isNull: true);

          var emptyErrorModel = MakeLogDiaryModel(
            tenantId: createdTenantModel.Data.Id,
            invalidError: true,
            isEmpty: true);

          #endregion

          Dictionary<string, LogDiaryModel> invalidModels = new Dictionary<string, LogDiaryModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(nullDateTimeModel), nullDateTimeModel);
          invalidModels.Add(nameof(nullDataModel), nullDataModel);
          invalidModels.Add(nameof(emptyDataModel), emptyDataModel);
          invalidModels.Add(nameof(nullErrorModel), nullErrorModel);
          invalidModels.Add(nameof(emptyErrorModel), emptyErrorModel);

          #endregion

          #region Arrange

          Dictionary<string, ResultModel<LogDiaryModel>> actualModels = new Dictionary<string, ResultModel<LogDiaryModel>>();

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _logDiaryService.Create(invalidModel.Value);
            actualModels.Add(invalidModel.Key, actualModel);
          }

          foreach (var actualModel in actualModels)
          {
            Assert.IsNull(actualModel.Value.Data);
            Assert.IsNotNull(actualModel.Value.Error);

            switch (actualModel.Value.Error.Message)
            {
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullDateTimeModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullDataModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(emptyDataModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullErrorModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(emptyErrorModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
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
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleCreate_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(currentUser =>
        {
          #region Init data

          #region Prepare invalid models

          LogDiaryModel nullModel = null;

          var nullDateTimeModel = MakeLogDiaryModel(
            tenantId: _tenantModel.Id,
            invalidDateTime: true,
            isNull: true);

          var nullDataModel = MakeLogDiaryModel(
            tenantId: _tenantModel.Id,
            invalidData: true,
            isNull: true);

          var emptyDataModel = MakeLogDiaryModel(
            tenantId: _tenantModel.Id,
            invalidData: true,
            isEmpty: true);

          var nullErrorModel = MakeLogDiaryModel(
            tenantId: _tenantModel.Id,
            invalidError: true,
            isNull: true);

          var emptyErrorModel = MakeLogDiaryModel(
            tenantId: _tenantModel.Id,
            invalidError: true,
            isEmpty: true);

          #endregion

          Dictionary<string, LogDiaryModel> invalidModels = new Dictionary<string, LogDiaryModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(nullDateTimeModel), nullDateTimeModel);
          invalidModels.Add(nameof(nullDataModel), nullDataModel);
          invalidModels.Add(nameof(emptyDataModel), emptyDataModel);
          invalidModels.Add(nameof(nullErrorModel), nullErrorModel);
          invalidModels.Add(nameof(emptyErrorModel), emptyErrorModel);

          #endregion

          #region Arrange

          Dictionary<string, ResultModel<LogDiaryModel>> actualModels = new Dictionary<string, ResultModel<LogDiaryModel>>();

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _logDiaryService.Create(invalidModel.Value);
            actualModels.Add(invalidModel.Key, actualModel);
          }

          foreach (var actualModel in actualModels)
          {
            Assert.IsNull(actualModel.Value.Data);
            Assert.IsNotNull(actualModel.Value.Error);

            switch (actualModel.Value.Error.Message)
            {
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullDateTimeModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullDataModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(emptyDataModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullErrorModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(emptyErrorModel):
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                break;
              default:
                Assert.Fail();
                break;
            }
          }

          #endregion
        });
    }

    #endregion

    #region DeleteAll(int tenantId)

    [TestMethod()]
    public void SysAdminRoleDeleteAll_WithExistingTenantAndExistingLogs_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          var isDeleted = _logDiaryService.DeleteAll(createdLogDiaryModel.Data.TenantId);
          var logAfterDeleted = _logDiaryService.GetByTenant(createdLogDiaryModel.Data.TenantId);

          Assert.IsTrue(isDeleted.Data);
          Assert.IsTrue(logAfterDeleted.Data.Count == 0);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdLogDiaryModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleDeleteAll_WithExistingTenantAndExistingLogs_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          var isDeleted = _logDiaryService.DeleteAll(createdLogDiaryModel.Data.TenantId);
          var logAfterDeleted = _logDiaryService.GetByTenant(createdLogDiaryModel.Data.TenantId);

          Assert.IsTrue(isDeleted.Data);
          Assert.IsTrue(logAfterDeleted.Data.Count == 0);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdLogDiaryModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleDeleteAll_WithExistingTenantAndExistingLogs_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var logDiaryModel = MakeLogDiaryModel(_tenantModel.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          var isDeleted = _logDiaryService.DeleteAll(_tenantModel.Id);
          var logAfterDeleted = _logDiaryService.GetByTenant(_tenantModel.Id);

          Assert.IsTrue(isDeleted.Data);
          Assert.IsTrue(logAfterDeleted.Data.Count == 0);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdLogDiaryModel.Id, listIds);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleDeleteAll_WithExistingTenantAndNotExistingLogs_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          #endregion

          #region Arrange

          var actual = _logDiaryService.DeleteAll(createdTenantModel.Data.Id);

          Assert.IsTrue(actual.Data);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleDeleteAll_WithExistingTenantAndNotExistingLogs_Success() 
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          #endregion

          #region Arrange

          var actual = _logDiaryService.DeleteAll(createdTenantModel.Data.Id);

          Assert.IsTrue(actual.Data);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleDeleteAll_WithExistingTenantAndNotExistingLogs_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Arrange

          var actual = _logDiaryService.DeleteAll(_tenantModel.Id);

          Assert.IsTrue(actual.Data);

          #endregion
        });
    }

    [TestMethod()]
    public void SysAdminRoleDeleteAll_WithNotExistingTenant_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          _tenantService.Delete(createdTenantModel.Data.Id);

          var actual = _logDiaryService.DeleteAll(createdTenantModel.Data.Id);

          Assert.IsFalse(actual.Data);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, actual.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleDeleteAll_WithNotExistingTenant_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          _tenantService.Delete(createdTenantModel.Data.Id);

          var actual = _logDiaryService.DeleteAll(createdTenantModel.Data.Id);

          Assert.IsFalse(actual.Data);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, actual.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleDeleteAll_WithInvalidTenantId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(currentUser =>
        {
          var isDeleted = _logDiaryService.DeleteAll(-1);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, isDeleted.Error.Message);
        });
    }

    [TestMethod()]
    public void AdminRoleDeleteAll_WithInvalidTenantId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(currentUser =>
        {
          var isDeleted = _logDiaryService.DeleteAll(-1);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, isDeleted.Error.Message);
        });
    }

    [TestMethod()]
    public void UserRoleDeleteAll_WithInvalidTenantId_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(currentUser =>
        {
          var isDeleted = _logDiaryService.DeleteAll(-1);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, isDeleted.Error.Message);
        });
    }

    #endregion

    #region DisableRange(int tenantId, List<int> ids);

    [TestMethod()]
    public void SysAdminRoleDisableRange_WithExistingLogDiaryId_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          List<int> listIds = new List<int>();
          listIds.Add(createdLogDiaryModel.Data.Id);

          var beforDeletedModel = _logDiaryService.GetByTenant(createdLogDiaryModel.Data.TenantId);
          var isDeleted = _logDiaryService.DisableRange(createdLogDiaryModel.Data.TenantId, listIds);
          var afterDeletedModel = _logDiaryService.GetByTenant(createdLogDiaryModel.Data.TenantId);

          Assert.IsTrue(beforDeletedModel.Data.Count == 1);
          Assert.IsTrue(isDeleted.Data);
          Assert.IsTrue(afterDeletedModel.Data.Count == 0);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdLogDiaryModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleDisableRange_WithExistingLogDiaryId_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          List<int> listIds = new List<int>();
          listIds.Add(createdLogDiaryModel.Data.Id);

          var beforDeletedModel = _logDiaryService.GetByTenant(createdLogDiaryModel.Data.TenantId);
          var isDeleted = _logDiaryService.DisableRange(createdLogDiaryModel.Data.TenantId, listIds);
          var afterDeletedModel = _logDiaryService.GetByTenant(createdLogDiaryModel.Data.TenantId);

          Assert.IsTrue(beforDeletedModel.Data.Count == 1);
          Assert.IsTrue(isDeleted.Data);
          Assert.IsTrue(afterDeletedModel.Data.Count == 0);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdLogDiaryModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleDisableRange_WithExistingLogDiaryId_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var logDiaryModel = MakeLogDiaryModel(_tenantModel.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          List<int> listIds = new List<int>();
          listIds.Add(createdLogDiaryModel.Data.Id);

          var beforDeletedModel = _logDiaryService.GetByTenant(createdLogDiaryModel.Data.TenantId);
          var isDeleted = _logDiaryService.DisableRange(createdLogDiaryModel.Data.TenantId, listIds);
          var afterDeletedModel = _logDiaryService.GetByTenant(createdLogDiaryModel.Data.TenantId);

          Assert.IsTrue(beforDeletedModel.Data.Count == 1);
          Assert.IsTrue(isDeleted.Data);
          Assert.IsTrue(afterDeletedModel.Data.Count == 0);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");

          if(createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdLogDiaryModel.Id, listIds);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleDisableRange_WithNonExistingLogDiaryId_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          List<int> listIds = new List<int>();
          listIds.Add(createdLogDiaryModel.Data.Id);

          var stDelete = _logDiaryService.DisableRange(createdTenantModel.Data.Id, listIds);
          var ndDelete = _logDiaryService.DisableRange(createdTenantModel.Data.Id, listIds);

          Assert.IsTrue(stDelete.Data);
          Assert.IsTrue(ndDelete.Data);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleDisableRange_WithNonExistingLogDiaryId_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          List<int> listIds = new List<int>();
          listIds.Add(createdLogDiaryModel.Data.Id);

          var stDelete = _logDiaryService.DisableRange(createdTenantModel.Data.Id, listIds);
          var ndDelete = _logDiaryService.DisableRange(createdTenantModel.Data.Id, listIds);

          Assert.IsTrue(stDelete.Data);
          Assert.IsTrue(ndDelete.Data);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleDisableRange_WithNonExistingLogDiary_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var logDiaryModel = MakeLogDiaryModel(_tenantModel.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          List<int> listIds = new List<int>();
          listIds.Add(createdLogDiaryModel.Data.Id);

          var stDelete = _logDiaryService.DisableRange(_tenantModel.Id, listIds);
          var ndDelete = _logDiaryService.DisableRange(_tenantModel.Id, listIds);

          Assert.IsTrue(stDelete.Data);
          Assert.IsTrue(ndDelete.Data);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(_tenantModel.Id, listIds);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleDisableRange_WithNonExistingTenantId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          _tenantService.Delete(createdTenantModel.Data.Id);

          List<int> listIds = new List<int>();
          listIds.Add(createdLogDiaryModel.Data.Id);

          var actual = _logDiaryService.DisableRange(createdTenantModel.Data.Id, listIds);

          Assert.IsFalse(actual.Data);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, actual.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleDisableRange_WithNonExistingTenantId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var tenantModel = MakeTenantModel();
          var createdTenantModel = _tenantService.Create(tenantModel);
          if (createdTenantModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdTenantModel", createdTenantModel.Data);

          var logDiaryModel = MakeLogDiaryModel(createdTenantModel.Data.Id);
          var createdLogDiaryModel = _logDiaryService.Create(logDiaryModel);
          if (createdLogDiaryModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdLogDiaryModel", createdLogDiaryModel.Data);

          #endregion

          #region Arrange

          _tenantService.Delete(createdTenantModel.Data.Id);

          List<int> listIds = new List<int>();
          listIds.Add(createdLogDiaryModel.Data.Id);

          var actual = _logDiaryService.DisableRange(createdTenantModel.Data.Id, listIds);

          Assert.IsFalse(actual.Data);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, actual.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdLogDiaryModel = param.CleanData.Get<LogDiaryModel>("createdLogDiaryModel");
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");

          if (createdLogDiaryModel != null)
          {
            List<int> listIds = new List<int>();
            listIds.Add(createdLogDiaryModel.Id);

            _logDiaryService.DisableRange(createdTenantModel.Id, listIds);
          }

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleDisableRange_WithInvalidLogDiaryId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(currentUser =>
        {
          List<int> listIds = new List<int>();
          listIds.Add(0);

          var isDeleted = _logDiaryService.DisableRange(_tenantModel.Id, listIds);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, isDeleted.Error.Message);
        });
    }

    [TestMethod()]
    public void AdminRoleDisableRange_WithInvalidLogDiaryId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(currentUser =>
        {
          List<int> listIds = new List<int>();
          listIds.Add(0);

          var isDeleted = _logDiaryService.DisableRange(_tenantModel.Id, listIds);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, isDeleted.Error.Message);
        });
    }

    [TestMethod()]
    public void UserRoleDisableRange_WithInvalidLogDiaryId_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(currentUser =>
        {
          List<int> listIds = new List<int>();
          listIds.Add(0);

          var isDeleted = _logDiaryService.DisableRange(_tenantModel.Id, listIds);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, isDeleted.Error.Message);
        });
    }

    #endregion

    #region Private methods

    private TenantModel MakeTenantModel()
    {
      return new TenantModel()
      {
        Name = "NameTest_" + Guid.NewGuid().ToString(),
        Description = "DescriptionTest_" + Guid.NewGuid().ToString(),
        IsEnabled = true,
        Number = new Random().Next(10000, 99000),
        ScheduleTimer = new Random().Next(1, 180)
      };
    }

    private LogDiaryModel MakeLogDiaryModel(
      int tenantId,
      int logDiaryModelId = 1,
      bool invalidDateTime = false,
      bool invalidData = false,
      bool invalidError = false,
      bool isNull = false,
      bool isEmpty = false)
    {
      LogDiaryModel result = null;

      if (isNull)
      {
        result = new LogDiaryModel()
        {
          Id = logDiaryModelId,
          DateTime = invalidDateTime ? null : DateTime.UtcNow,
          Data = invalidData ? null : "ValidDate_" + Guid.NewGuid().ToString(),
          Error = invalidError ? null : "ValidError_" + Guid.NewGuid().ToString(),
          IsEnabled = true,
          TenantId = tenantId
        };
      }
      else if (isEmpty)
      {
        result = new LogDiaryModel()
        {
          Id = logDiaryModelId,
          DateTime = DateTime.UtcNow,
          Data = invalidData ? String.Empty : "ValidDate_" + Guid.NewGuid().ToString(),
          Error = invalidError ? String.Empty : "ValidError_" + Guid.NewGuid().ToString(),
          IsEnabled = true,
          TenantId = tenantId
        };
      }
      else
      {
        result = new LogDiaryModel()
        {
          DateTime = DateTime.UtcNow,
          Data = "DataTest_" + Guid.NewGuid().ToString(),
          Error = "ErrorTest_" + Guid.NewGuid().ToString(),
          IsEnabled = true,
          TenantId = tenantId
        };
      }

      return result;
    }

    #endregion
  }
}
