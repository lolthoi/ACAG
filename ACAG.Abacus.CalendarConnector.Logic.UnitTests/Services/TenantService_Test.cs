using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services
{
  [TestClass()]
  public class TenantService_Test : TestBase
  {

    private ITenantService _tenantService;
    private IUserService _userService;

    private TenantModel _tenantModel;
    private TenantModel _tenantModel2;
    private TenantModelForUser _tenantModelForUser;
    private TenantModelForUser _tenantModelForUser2;
    private UserModel _userModel;
    private UserModel _userModel2;
    private const string NO_ROLE = "No role";

    public TenantService_Test() : base()
    {
    }

    protected override void InitServices()
    {
      _userService = _scope.ServiceProvider.GetService<IUserService>();
      _tenantService = _scope.ServiceProvider.GetService<ITenantService>();
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


            _tenantModel2 = new TenantModel
            {
              Name = "TenantTest_" + Guid.NewGuid().ToString(),
              Description = "TenantDescription_Test_" + Guid.NewGuid().ToString(),
              IsEnabled = true,
              Number = new Random().Next(10000, 99000),
              ScheduleTimer = new Random().Next(1, 180)
            };
            _tenantService.Create(_tenantModel2);

            _tenantModelForUser2 = new TenantModelForUser()
            {
              Id = _tenantModel2.Id,
              Name = _tenantModel2.Name
            };

            _userModel2 = GetRandomUser(_tenantModelForUser2);
            _userService.Create(_userModel2, "123456");
          });
    }

    protected override void CleanEnvirontment()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          _userService.Delete(_userModel.Id);
          _tenantService.Delete(_tenantModel.Id);
          _tenantService.Delete(_tenantModel2.Id);
        });
    }

    #region GET_ALL_TENANT

    [TestMethod()]
    public void SysAdminRoleGetAll_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();

          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          var result = _tenantService.GetAll("");
          var actual = result.Data.Tenants.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.IsNotNull(actual);
          Assert.AreEqual(tenantModel.Id, actual.Id);
          Assert.AreEqual(tenantModel.Description, actual.Description);
          Assert.AreEqual(tenantModel.Number, actual.Number);
          Assert.AreEqual(tenantModel.ScheduleTimer, actual.ScheduleTimer);
          Assert.AreEqual(tenantModel.IsEnabled, actual.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetAllWithSearch_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {

          #region Init data

          TenantModel tenantModel = GetRandomTenant();

          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          var result = _tenantService.GetAll(createdModel.Data.Name);
          var actual = result.Data.Tenants.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.IsNotNull(actual);
          Assert.AreEqual(tenantModel.Id, actual.Id);
          Assert.AreEqual(tenantModel.Description, actual.Description);
          Assert.AreEqual(tenantModel.Number, actual.Number);
          Assert.AreEqual(tenantModel.ScheduleTimer, actual.ScheduleTimer);
          Assert.AreEqual(tenantModel.IsEnabled, actual.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetAll_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();

          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          var result = _tenantService.GetAll("");
          var actual = result.Data.Tenants.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.IsNotNull(actual);
          Assert.AreEqual(tenantModel.Id, actual.Id);
          Assert.AreEqual(tenantModel.Description, actual.Description);
          Assert.AreEqual(tenantModel.Number, actual.Number);
          Assert.AreEqual(tenantModel.ScheduleTimer, actual.ScheduleTimer);
          Assert.AreEqual(tenantModel.IsEnabled, actual.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetAllWithSearch_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {

          #region Init data

          TenantModel tenantModel = GetRandomTenant();

          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          var result = _tenantService.GetAll(createdModel.Data.Name);
          var actual = result.Data.Tenants.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.IsNotNull(actual);
          Assert.AreEqual(tenantModel.Id, actual.Id);
          Assert.AreEqual(tenantModel.Description, actual.Description);
          Assert.AreEqual(tenantModel.Number, actual.Number);
          Assert.AreEqual(tenantModel.ScheduleTimer, actual.ScheduleTimer);
          Assert.AreEqual(tenantModel.IsEnabled, actual.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetAll_Sucess()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {

          var data = _tenantService.GetAll("");

          #region Get and check data

          Assert.IsNotNull(data.Data);
          Assert.IsNull(data.Error);
          Assert.AreEqual(1, data.Data.Tenants.Count);
          

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    #endregion

    #region GET_BY_TENANT_ID

    [TestMethod()]
    public void SysAdminRoleGetById_ValidTenantId_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          var result = _tenantService.GetById(createdModel.Data.Id);
          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.AreEqual(tenantModel.Id, result.Data.Id);
          Assert.AreEqual(tenantModel.Name, result.Data.Name);
          Assert.AreEqual(tenantModel.Description, result.Data.Description);
          Assert.AreEqual(tenantModel.IsEnabled, result.Data.IsEnabled);
          Assert.AreEqual(tenantModel.Number, result.Data.Number);
          Assert.AreEqual(tenantModel.ScheduleTimer, result.Data.ScheduleTimer);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetById_InValidTenantId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _tenantService.GetById(0);

          #endregion

          #region Get and check data

          Assert.IsNull(result.Data);
          Assert.IsNotNull(result.Error);
          Assert.AreEqual(ErrorType.BAD_REQUEST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, result.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod()]
    public void AdminRoleGetById_ValidTenantId_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          var result = _tenantService.GetById(createdModel.Data.Id);
          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.AreEqual(tenantModel.Id, result.Data.Id);
          Assert.AreEqual(tenantModel.Name, result.Data.Name);
          Assert.AreEqual(tenantModel.Description, result.Data.Description);
          Assert.AreEqual(tenantModel.IsEnabled, result.Data.IsEnabled);
          Assert.AreEqual(tenantModel.Number, result.Data.Number);
          Assert.AreEqual(tenantModel.ScheduleTimer, result.Data.ScheduleTimer);

          #endregion
        })
        .ThenCleanDataTest(param => 
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetById_InValidTenantId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _tenantService.GetById(0);

          #endregion

          #region Get and check data

          Assert.IsNull(result.Data);
          Assert.IsNotNull(result.Error);
          Assert.AreEqual(ErrorType.BAD_REQUEST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, result.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod()]
    public void UserRoleGetById_ValidTenantId_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _tenantService.GetById(_userModel.Tenant.Id);

          #endregion

          #region Get and check data

          var actual = result.Data;

          Assert.IsNull(result.Error);
          Assert.IsNotNull(actual);
          Assert.AreEqual(_userModel.Tenant.Id, result.Data.Id);
          Assert.AreEqual(_userModel.Tenant.Name, result.Data.Name);


          #endregion
        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod()]
    public void UserRoleGetById_InValidTenantId_Fail()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _tenantService.GetById(0);

          #endregion

          #region Get and check data

          Assert.IsNull(result.Data);
          Assert.IsNotNull(result.Error);
          Assert.AreEqual(ErrorType.BAD_REQUEST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, result.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    #endregion

    #region ADD_TENANT

    [TestMethod]
    public void SysAdminRoleAddTenant_Valid_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createdModel.Data);
          var actualModel = _tenantService.GetById(createdModel.Data.Id).Data;
          Assert.IsNotNull(actualModel);
          Assert.AreEqual(tenantModel.Id, actualModel.Id);
          Assert.AreEqual(tenantModel.Description, actualModel.Description);
          Assert.AreEqual(tenantModel.Number, actualModel.Number);
          Assert.AreEqual(tenantModel.ScheduleTimer, actualModel.ScheduleTimer);
          Assert.AreEqual(tenantModel.IsEnabled, actualModel.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void SysAdminRoleAddTenant_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {

          #region Init Data

          TenantModel nullModel = null;

          TenantModel invalidNullName = GetRandomTenant();
          invalidNullName.Name = "";

          List<TenantModel> invalidMaxlengthName = new List<TenantModel>();
          foreach (var item in InvalidMaxLengthName())
          {
            var tenant = GetRandomTenant();
            tenant.Name = item;
            invalidMaxlengthName.Add(tenant);
          }

          List<TenantModel> invalidNumberLarger100000OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidNumberLarger100000OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.Number = item;
            invalidNumberLarger100000OrSmaller1.Add(tenant);
          }

          List<TenantModel> invalidSchedulerLarger180OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidSchedulerLarger180OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.ScheduleTimer = item;
            invalidSchedulerLarger180OrSmaller1.Add(tenant);
          }

          TenantModel invalidNameIsAlreadyExists = GetRandomTenant();
          invalidNameIsAlreadyExists.Name = _tenantModel.Name;

          List<InvalidModel<TenantModel>> invalidModels = new List<InvalidModel<TenantModel>>();
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(nullModel), nullModel));
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNullName), invalidNullName));
          foreach (var item in invalidMaxlengthName)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidMaxlengthName), item));
          }
          foreach (var item in invalidNumberLarger100000OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNumberLarger100000OrSmaller1), item));
          }
          foreach (var item in invalidSchedulerLarger180OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidSchedulerLarger180OrSmaller1), item));
          }
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _tenantService.Create(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNumberLarger100000OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NUMBER_MUST_BETWEEN_1_AND_99999, actualModel.Error.Message);
                break;
              case nameof(invalidSchedulerLarger180OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SCHEDULER_SHOULD_BE_BETWEEN_1_AND_180, actualModel.Error.Message);
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

        });
    }

    [TestMethod]
    public void AdminRoleAddTenant_Valid_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createdModel.Data);
          var actualModel = _tenantService.GetById(createdModel.Data.Id).Data;
          Assert.IsNotNull(actualModel);
          Assert.AreEqual(tenantModel.Id, actualModel.Id);
          Assert.AreEqual(tenantModel.Description, actualModel.Description);
          Assert.AreEqual(tenantModel.Number, actualModel.Number);
          Assert.AreEqual(tenantModel.ScheduleTimer, actualModel.ScheduleTimer);
          Assert.AreEqual(tenantModel.IsEnabled, actualModel.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleAddTenant_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {

          #region Init Data

          TenantModel nullModel = null;

          TenantModel invalidNullName = GetRandomTenant();
          invalidNullName.Name = "";

          List<TenantModel> invalidMaxlengthName = new List<TenantModel>();
          foreach (var item in InvalidMaxLengthName())
          {
            var tenant = GetRandomTenant();
            tenant.Name = item;
            invalidMaxlengthName.Add(tenant);
          }

          List<TenantModel> invalidNumberLarger100000OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidNumberLarger100000OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.Number = item;
            invalidNumberLarger100000OrSmaller1.Add(tenant);
          }

          List<TenantModel> invalidSchedulerLarger180OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidSchedulerLarger180OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.ScheduleTimer = item;
            invalidSchedulerLarger180OrSmaller1.Add(tenant);
          }

          TenantModel invalidNameIsAlreadyExists = GetRandomTenant();
          invalidNameIsAlreadyExists.Name = _tenantModel.Name;

          List<InvalidModel<TenantModel>> invalidModels = new List<InvalidModel<TenantModel>>();
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(nullModel), nullModel));
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNullName), invalidNullName));
          foreach (var item in invalidMaxlengthName)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidMaxlengthName), item));
          }
          foreach (var item in invalidNumberLarger100000OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNumberLarger100000OrSmaller1), item));
          }
          foreach (var item in invalidSchedulerLarger180OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidSchedulerLarger180OrSmaller1), item));
          }
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _tenantService.Create(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNumberLarger100000OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NUMBER_MUST_BETWEEN_1_AND_99999, actualModel.Error.Message);
                break;
              case nameof(invalidSchedulerLarger180OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SCHEDULER_SHOULD_BE_BETWEEN_1_AND_180, actualModel.Error.Message);
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

        });
    }

    [TestMethod]
    public void UserRoleAddTenant_NoPermission_Fail()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);

          #endregion

          #region Get and check data


          Assert.IsNotNull(createdModel.Error);
          Assert.IsNull(createdModel.Data);
          Assert.AreEqual(ErrorType.NO_ROLE, createdModel.Error.Type);
          Assert.AreEqual(NO_ROLE, createdModel.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    #endregion

    #region UPDATE_TENANT

    [TestMethod]
    public void SysAdminRoleUpdateTenant_Valid_Success()
    {
      _testService
        .StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);
          var modelUpdate = GetRandomTenant();
          modelUpdate.Id = createdModel.Data.Id;
          var updateTenantModel = _tenantService.Update(modelUpdate);

          #endregion

          #region Get and check data

          Assert.IsNotNull(updateTenantModel.Data);
          var actualModel = _tenantService.GetById(updateTenantModel.Data.Id).Data;
          Assert.IsNotNull(actualModel);
          Assert.AreEqual(modelUpdate.Id, actualModel.Id);
          Assert.AreEqual(modelUpdate.Number, actualModel.Number);
          Assert.AreEqual(modelUpdate.ScheduleTimer, actualModel.ScheduleTimer);
          Assert.AreEqual(modelUpdate.IsEnabled, actualModel.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void SysAdminRoleUpdateTenant_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {

          #region Init Data

          TenantModel nullModel = null;

          TenantModel invalidNullName = GetRandomTenant();
          invalidNullName.Id = _tenantModel.Id;
          invalidNullName.Name = "";

          List<TenantModel> invalidMaxlengthName = new List<TenantModel>();
          foreach (var item in InvalidMaxLengthName())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.Name = item;
            invalidMaxlengthName.Add(tenant);
          }

          List<TenantModel> invalidNumberLarger100000OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidNumberLarger100000OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.Number = item;
            invalidNumberLarger100000OrSmaller1.Add(tenant);
          }

          List<TenantModel> invalidSchedulerLarger180OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidSchedulerLarger180OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.ScheduleTimer = item;
            invalidSchedulerLarger180OrSmaller1.Add(tenant);
          }

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);
          var invalidNameIsAlreadyExists = GetRandomTenant();
          invalidNameIsAlreadyExists.Id = createdModel.Data.Id;
          invalidNameIsAlreadyExists.Name = _tenantModel.Name;

          List<InvalidModel<TenantModel>> invalidModels = new List<InvalidModel<TenantModel>>();
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(nullModel), nullModel));
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNullName), invalidNullName));
          foreach (var item in invalidMaxlengthName)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidMaxlengthName), item));
          }
          foreach (var item in invalidNumberLarger100000OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNumberLarger100000OrSmaller1), item));
          }
          foreach (var item in invalidSchedulerLarger180OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidSchedulerLarger180OrSmaller1), item));
          }
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _tenantService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNumberLarger100000OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NUMBER_MUST_BETWEEN_1_AND_99999, actualModel.Error.Message);
                break;
              case nameof(invalidSchedulerLarger180OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SCHEDULER_SHOULD_BE_BETWEEN_1_AND_180, actualModel.Error.Message);
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
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleUpdateTenant_Valid_Success()
    {
      _testService
        .StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);
          var modelUpdate = GetRandomTenant();
          modelUpdate.Id = createdModel.Data.Id;
          var updateTenantModel = _tenantService.Update(modelUpdate);

          #endregion

          #region Get and check data

          Assert.IsNotNull(updateTenantModel.Data);
          var actualModel = _tenantService.GetById(updateTenantModel.Data.Id).Data;
          Assert.IsNotNull(actualModel);
          Assert.AreEqual(modelUpdate.Id, actualModel.Id);
          Assert.AreEqual(modelUpdate.Number, actualModel.Number);
          Assert.AreEqual(modelUpdate.ScheduleTimer, actualModel.ScheduleTimer);
          Assert.AreEqual(modelUpdate.IsEnabled, actualModel.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleUpdateTenant_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {

          #region Init Data

          TenantModel nullModel = null;

          TenantModel invalidNullName = GetRandomTenant();
          invalidNullName.Id = _tenantModel.Id;
          invalidNullName.Name = "";

          List<TenantModel> invalidMaxlengthName = new List<TenantModel>();
          foreach (var item in InvalidMaxLengthName())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.Name = item;
            invalidMaxlengthName.Add(tenant);
          }

          List<TenantModel> invalidNumberLarger100000OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidNumberLarger100000OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.Number = item;
            invalidNumberLarger100000OrSmaller1.Add(tenant);
          }

          List<TenantModel> invalidSchedulerLarger180OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidSchedulerLarger180OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.ScheduleTimer = item;
            invalidSchedulerLarger180OrSmaller1.Add(tenant);
          }

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          param.CleanData.Add("CreateModel", createdModel.Data);
          var invalidNameIsAlreadyExists = GetRandomTenant();
          invalidNameIsAlreadyExists.Id = createdModel.Data.Id;
          invalidNameIsAlreadyExists.Name = _tenantModel.Name;

          List<InvalidModel<TenantModel>> invalidModels = new List<InvalidModel<TenantModel>>();
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(nullModel), nullModel));
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNullName), invalidNullName));
          foreach (var item in invalidMaxlengthName)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidMaxlengthName), item));
          }
          foreach (var item in invalidNumberLarger100000OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNumberLarger100000OrSmaller1), item));
          }
          foreach (var item in invalidSchedulerLarger180OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidSchedulerLarger180OrSmaller1), item));
          }
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _tenantService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNumberLarger100000OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NUMBER_MUST_BETWEEN_1_AND_99999, actualModel.Error.Message);
                break;
              case nameof(invalidSchedulerLarger180OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SCHEDULER_SHOULD_BE_BETWEEN_1_AND_180, actualModel.Error.Message);
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
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void UserRoleUpdateTenant_Valid_Success()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var modelUpdate = GetRandomTenant();
          modelUpdate.Id = _tenantModelForUser.Id;
          var updateTenantModel = _tenantService.Update(modelUpdate);

          #endregion

          #region Get and check data

          Assert.IsNotNull(updateTenantModel.Data);
          var actualModel = _tenantService.GetById(updateTenantModel.Data.Id).Data;
          Assert.IsNotNull(actualModel);
          Assert.AreEqual(modelUpdate.Id, actualModel.Id);
          Assert.AreEqual(modelUpdate.Number, actualModel.Number);
          Assert.AreEqual(modelUpdate.ScheduleTimer, actualModel.ScheduleTimer);
          Assert.AreEqual(modelUpdate.IsEnabled, actualModel.IsEnabled);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod]
    public void UserRoleUpdateTenant_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {

          #region Init Data

          TenantModel nullModel = null;

          TenantModel invalidNullName = GetRandomTenant();
          invalidNullName.Id = _tenantModel.Id;
          invalidNullName.Name = "";

          List<TenantModel> invalidMaxlengthName = new List<TenantModel>();
          foreach (var item in InvalidMaxLengthName())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.Name = item;
            invalidMaxlengthName.Add(tenant);
          }

          List<TenantModel> invalidNumberLarger100000OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidNumberLarger100000OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.Number = item;
            invalidNumberLarger100000OrSmaller1.Add(tenant);
          }

          List<TenantModel> invalidSchedulerLarger180OrSmaller1 = new List<TenantModel>();
          foreach (var item in InvalidSchedulerLarger180OrSmaller1())
          {
            var tenant = GetRandomTenant();
            tenant.Id = _tenantModel.Id;
            tenant.ScheduleTimer = item;
            invalidSchedulerLarger180OrSmaller1.Add(tenant);
          }

          var invalidNameIsAlreadyExists = GetRandomTenant();
          invalidNameIsAlreadyExists.Id = _tenantModel.Id;
          invalidNameIsAlreadyExists.Name = _tenantModel2.Name;

          List<InvalidModel<TenantModel>> invalidModels = new List<InvalidModel<TenantModel>>();
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(nullModel), nullModel));
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNullName), invalidNullName));
          foreach (var item in invalidMaxlengthName)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidMaxlengthName), item));
          }
          foreach (var item in invalidNumberLarger100000OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNumberLarger100000OrSmaller1), item));
          }
          foreach (var item in invalidSchedulerLarger180OrSmaller1)
          {
            invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidSchedulerLarger180OrSmaller1), item));
          }
          invalidModels.Add(new InvalidModel<TenantModel>(nameof(invalidNameIsAlreadyExists), invalidNameIsAlreadyExists));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _tenantService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS, actualModel.Error.Message);
                break;
              case nameof(invalidNumberLarger100000OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_NUMBER_MUST_BETWEEN_1_AND_99999, actualModel.Error.Message);
                break;
              case nameof(invalidSchedulerLarger180OrSmaller1):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_SCHEDULER_SHOULD_BE_BETWEEN_1_AND_180, actualModel.Error.Message);
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
          var createModel = param.CleanData.Get<TenantModel>("CreateModel");
          if (createModel != null)
          {
            _tenantService.Delete(createModel.Id);
          }
        });
    }

    #endregion

    #region DELETE_TENANT

    [TestMethod]
    public void SysAdminRoleDeteleTenant_ValidTeanantId_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          var result = _tenantService.Delete(createdModel.Data.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.AreEqual(true, result.Data);

          var actualModel = _tenantService.GetById(createdModel.Data.Id);
          Assert.IsNull(actualModel.Data);
          Assert.IsNotNull(actualModel.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, actualModel.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod]
    public void SysAdminRoleDeteleTenant_InValidTeanantId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _tenantService.Delete(0);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Error);
          Assert.IsNotNull(result.Data);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod]
    public void SysAdminRoleDeteleTenant_DeleteDuplicated_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          _tenantService.Delete(createdModel.Data.Id);
          var result = _tenantService.Delete(createdModel.Data.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Error);
          Assert.IsNotNull(result.Data);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod]
    public void AdminRoleDeteleTenant_ValidTeanantId_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          var result = _tenantService.Delete(createdModel.Data.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.AreEqual(true, result.Data);

          var actualModel = _tenantService.GetById(createdModel.Data.Id);
          Assert.IsNull(actualModel.Data);
          Assert.IsNotNull(actualModel.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
          Assert.AreEqual(LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED, actualModel.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod]
    public void AdminRoleDeteleTenant_InValidTeanantId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _tenantService.Delete(0);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Error);
          Assert.IsNotNull(result.Data);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod]
    public void AdminRoleDeteleTenant_DeleteDuplicated_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          TenantModel tenantModel = GetRandomTenant();
          var createdModel = _tenantService.Create(tenantModel);
          _tenantService.Delete(createdModel.Data.Id);
          var result = _tenantService.Delete(createdModel.Data.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Error);
          Assert.IsNotNull(result.Data);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleDeleteTenant_NoPermission_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {

          #region Init data

          TenantModel model = GetRandomTenant();
          var createdModel = _tenantService.Delete(model.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(createdModel.Error);
          Assert.IsNotNull(createdModel.Data);
          Assert.AreEqual(createdModel.Data, false);
          Assert.AreEqual(ErrorType.NO_ROLE, createdModel.Error.Type);
          Assert.AreEqual(NO_ROLE, createdModel.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    #endregion

    #region Private method

    private TenantModel GetRandomTenant()
    {
      return new TenantModel()
      {
        Name = "TenantTest_" + Guid.NewGuid().ToString(),
        Description = "TenantDescription_Test_" + Guid.NewGuid().ToString(),
        IsEnabled = true,
        Number = new Random().Next(10000, 99000),
        ScheduleTimer = 20
      };
    }

    private List<string> InvalidMaxLengthName()
    {
      List<string> names = new List<string>()
      {
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
        "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
        "ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc"
      };
      return names;
    }

    private List<int> InvalidSchedulerLarger180OrSmaller1()
    {
      List<int> scheduler = new List<int>()
      {
        -1,
        181, 
        0,
        1000,
        -500,
        -1111
      };
      return scheduler;
    }

    private List<int> InvalidNumberLarger100000OrSmaller1()
    {
      List<int> numbers = new List<int>()
      {
        -1,
        100000,
        0,
        999999,
        -500,
        -1111
      };
      return numbers;
    }
    #endregion
  }
}
