using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services
{
  [TestClass()]
  public class UserService_Test : TestBase
  {

    #region Variable and Constructor

    private ITenantService _tenantService;
    private IUserService _userService;

    private TenantModel _tenantModel;
    private TenantModelForUser _tenantModelForUser;
    private UserModel _userModel;
    private const string NO_ROLE = "No role";

    public UserService_Test()
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

    #endregion

    #region GET_ALL

    [TestMethod()]
    public void SysAdminRoleGetAll_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var culture = GetRandomCulture();
          var role = GetRandomAppRole();

          UserModel model = GetRandomUser(_tenantModelForUser);

          var createdModel = _userService.Create(model, "123456");

          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          var data = _userService.GetAll("");
          var actual = data.Data.Users.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNull(data.Error);
          Assert.IsNotNull(actual);
          Assert.AreEqual(model.Id, actual.Id);
          Assert.AreEqual(model.Email, actual.Email);
          Assert.AreEqual(model.FirstName, actual.FirstName);
          Assert.AreEqual(model.LastName, actual.LastName);
          Assert.AreEqual(model.AppRole.Code, actual.RoleName);

          #endregion

        }).ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null)
          {
            _userService.Delete(createModel.Id);
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

          var culture = GetRandomCulture();
          var role = GetRandomAppRole();

          UserModel model = GetRandomUser(_tenantModelForUser);

          var createdModel = _userService.Create(model, "123456");

          param.CleanData.Add("CreateModel", createdModel.Data);

          #endregion

          #region Get and check data

          var data = _userService.GetAll("");
          var actual = data.Data.Users.Where(x => x.Id == createdModel.Data.Id).FirstOrDefault();

          Assert.IsNull(data.Error);
          Assert.IsNotNull(actual);
          Assert.AreEqual(model.Id, actual.Id);
          Assert.AreEqual(model.Email, actual.Email);
          Assert.AreEqual(model.FirstName, actual.FirstName);
          Assert.AreEqual(model.LastName, actual.LastName);
          Assert.AreEqual(model.AppRole.Code, actual.RoleName);

          #endregion

        }).ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null)
          {
            _userService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetAll_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {

          var expected = _userModel;

          var data = _userService.GetAll("");

          #region Get and check data

          Assert.IsNotNull(data.Error);
          Assert.IsNull(data.Data);
          Assert.AreEqual(ErrorType.NO_ROLE, data.Error.Type);
          Assert.AreEqual(NO_ROLE, data.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    #endregion

    #region GET_ID

    [TestMethod()]
    public void SysAdminRoleGetById_ValidUserId_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var user = GetRandomUser(_tenantModelForUser);

          var expectation = _userService.Create(user, "123456");

          #endregion

          #region Get and check data

          if (expectation.Data != null)
          {
            var result = _userService.GetById(expectation.Data.Id);

            param.CleanData.Add("CreateModel", expectation.Data);
            var actual = result.Data;

            Assert.IsNull(result.Error);
            Assert.IsNotNull(actual);

            Assert.AreEqual(user.Id, actual.Id);
            Assert.AreEqual(user.Email, actual.Email);
            Assert.AreEqual(user.FirstName, actual.FirstName);
            Assert.AreEqual(user.LastName, actual.LastName);
            Assert.AreEqual(user.RoleName, actual.RoleName);
            Assert.AreEqual(user.TenantName, actual.TenantName);
          }

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null)
          {
            _userService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetById_InValidUserId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _userService.GetById(0);

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

    [TestMethod()]
    public void AdminRoleGetById_ValidUserId_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var user = GetRandomUser(_tenantModelForUser);

          var expectation = _userService.Create(user, "123456");

          #endregion

          #region Get and check data

          if (expectation.Data != null)
          {
            var result = _userService.GetById(expectation.Data.Id);

            param.CleanData.Add("CreateModel", expectation.Data);
            var actual = result.Data;

            Assert.IsNull(result.Error);
            Assert.IsNotNull(actual);

            Assert.AreEqual(user.Id, actual.Id);
            Assert.AreEqual(user.Email, actual.Email);
            Assert.AreEqual(user.FirstName, actual.FirstName);
            Assert.AreEqual(user.LastName, actual.LastName);
            Assert.AreEqual(user.RoleName, actual.RoleName);
            Assert.AreEqual(user.TenantName, actual.TenantName);
          }

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null)
          {
            _userService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetById_InValidUserId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _userService.GetById(0);

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

    [TestMethod()]
    public void UserRoleGetById_ValidUserId_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _userService.GetById(_userModel.Id);

          #endregion

          #region Get and check data

          var actual = result.Data;

          Assert.IsNull(result.Error);
          Assert.IsNotNull(actual);

          Assert.AreEqual(_userModel.Id, actual.Id);
          Assert.AreEqual(_userModel.Email, actual.Email);
          Assert.AreEqual(_userModel.FirstName, actual.FirstName);
          Assert.AreEqual(_userModel.LastName, actual.LastName);
          Assert.AreEqual(_userModel.RoleName, actual.RoleName);
          Assert.AreEqual(_userModel.TenantName, actual.TenantName);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod()]
    public void UserRoleGetById_InValidUserId_Fail()
    {
      _testService
        .StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _userService.GetById(0);

          #endregion

          #region Get and check data

          Assert.IsNull(result.Data);
          Assert.IsNotNull(result.Error);
          Assert.AreEqual(ErrorType.NO_DATA_ROLE, result.Error.Type);
          Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, result.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    #endregion

    #region ADD_USER

    [TestMethod()]
    public void SysAdminRoleAddUser_ValidUser_Success()
    {

      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {

          #region Init data

          var expectedModel = GetRandomUser(_tenantModelForUser);

          var createdModel = _userService.Create(expectedModel, "123456");

          #endregion

          #region Get and check data

          Assert.IsNotNull(createdModel.Data);

          param.CleanData.Add("CreateModel", createdModel.Data);

          var actualModel = _userService.GetById(createdModel.Data.Id).Data;

          Assert.IsNotNull(actualModel);
          Assert.AreEqual(expectedModel.Id, actualModel.Id);
          Assert.AreEqual(expectedModel.FirstName, actualModel.FirstName);
          Assert.AreEqual(expectedModel.LastName, actualModel.LastName);
          Assert.AreEqual(expectedModel.Email, actualModel.Email);
          Assert.AreEqual(expectedModel.Comment, actualModel.Comment);
          Assert.AreEqual(expectedModel.IsEnabled, actualModel.IsEnabled);
          Assert.AreEqual(expectedModel.AppRole.Id, actualModel.AppRole.Id);
          Assert.AreEqual(expectedModel.AppRole.IsAdministrator, actualModel.AppRole.IsAdministrator);
          Assert.AreEqual(expectedModel.AppRole.Code, actualModel.AppRole.Code);
          Assert.AreEqual(expectedModel.Language.Id, actualModel.Language.Id);
          Assert.AreEqual(expectedModel.Language.DisplayName, actualModel.Language.DisplayName);

          if (!actualModel.AppRole.IsAdministrator)
          {
            Assert.AreEqual(expectedModel.Tenant.Id, actualModel.Tenant.Id);
            Assert.AreEqual(expectedModel.Tenant.Name, actualModel.Tenant.Name);
          }

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null)
          {
            _userService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void SysAdminRoleAddUser_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel nullModel = null;

          var invalidNullFirstName = GetRandomUser(_tenantModelForUser);
          invalidNullFirstName.FirstName = "";

          List<UserModel> invalidMinlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMinLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.FirstName = item;
            invalidMinlengthFirstName.Add(user);
          }

          List<UserModel> invalidMaxlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.FirstName = item;
            invalidMaxlengthFirstName.Add(user);
          }

          var invalidNullLastName = GetRandomUser(_tenantModelForUser);
          invalidNullLastName.LastName = "";

          List<UserModel> invalidMinlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMinLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.LastName = item;
            invalidMinlengthLastName.Add(user);
          }

          List<UserModel> invalidMaxlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.LastName = item;
            invalidMaxlengthLastName.Add(user);
          }


          var invalidNullEmail = GetRandomUser(_tenantModelForUser);
          invalidNullEmail.Email = "";

          var invalidEmailIsAlreadyExists = GetRandomUser(_tenantModelForUser);
          invalidEmailIsAlreadyExists.Email = _userModel.Email;

          List<UserModel> invalidMaxlengthEmail = new List<UserModel>();
          foreach (var item in InvalidMaxlengthEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Email = item;
            invalidMaxlengthEmail.Add(user);
          }

          List<UserModel> invalidFormatIncorrecEmail = new List<UserModel>();
          foreach (var item in InvalidFormatIncorrecEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Email = item;
            invalidFormatIncorrecEmail.Add(user);
          }

          var invalidRoleIsAdminAndTenantNotNull = GetRandomUser(_tenantModelForUser);
          invalidRoleIsAdminAndTenantNotNull.AppRole.IsAdministrator = true;
          invalidRoleIsAdminAndTenantNotNull.Tenant = _tenantModelForUser;

          List<InvalidModel<UserModel>> invalidModels = new List<InvalidModel<UserModel>>();
          invalidModels.Add(new InvalidModel<UserModel>(nameof(nullModel), nullModel));

          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullFirstName), invalidNullFirstName));
          foreach (var item in invalidMinlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthFirstName), item));
          }

          foreach (var item in invalidMaxlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthFirstName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullLastName), invalidNullLastName));
          foreach (var item in invalidMinlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthLastName), item));
          }
          foreach (var item in invalidMaxlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthLastName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullEmail), invalidNullEmail));
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidEmailIsAlreadyExists), invalidEmailIsAlreadyExists));
          foreach (var item in invalidMaxlengthEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthEmail), item));
          }
          foreach (var item in invalidFormatIncorrecEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidFormatIncorrecEmail), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidRoleIsAdminAndTenantNotNull), invalidRoleIsAdminAndTenantNotNull));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _userService.Create(invalidModel.Value, "123456");

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_FIRST_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMinlengthFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MIN_LENGTH_OF_FIRST_NAME_IS_3, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_FIRST_NAME_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LAST_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMinlengthLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MIN_LENGTH_OF_LAST_NAME_IS_3, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_LAST_NAME_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidEmailIsAlreadyExists):
                Assert.AreEqual(ErrorType.DUPLICATED, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_ENTERED_EMAIL_ALREADY_EXISTS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidRoleIsAdminAndTenantNotNull):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.IF_THE_USER_IS_AN_ADMINISTRATOR_THE_TENANT_MUST_BE_EMPTY, actualModel.Error.Message);
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

    [TestMethod()]
    public void AdminRoleAddUser_ValidUser_Success()
    {

      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {

          #region Init data

          var expectedModel = GetRandomUser(_tenantModelForUser);

          var createdModel = _userService.Create(expectedModel, "123456");

          #endregion

          #region Get and check data

          Assert.IsNotNull(createdModel.Data);

          param.CleanData.Add("CreateModel", createdModel.Data);

          var actualModel = _userService.GetById(createdModel.Data.Id).Data;

          Assert.IsNotNull(actualModel);
          Assert.AreEqual(expectedModel.Id, actualModel.Id);
          Assert.AreEqual(expectedModel.FirstName, actualModel.FirstName);
          Assert.AreEqual(expectedModel.LastName, actualModel.LastName);
          Assert.AreEqual(expectedModel.Email, actualModel.Email);
          Assert.AreEqual(expectedModel.Comment, actualModel.Comment);
          Assert.AreEqual(expectedModel.IsEnabled, actualModel.IsEnabled);
          Assert.AreEqual(expectedModel.AppRole.Id, actualModel.AppRole.Id);
          Assert.AreEqual(expectedModel.AppRole.IsAdministrator, actualModel.AppRole.IsAdministrator);
          Assert.AreEqual(expectedModel.AppRole.Code, actualModel.AppRole.Code);
          Assert.AreEqual(expectedModel.Language.Id, actualModel.Language.Id);
          Assert.AreEqual(expectedModel.Language.DisplayName, actualModel.Language.DisplayName);

          if (!actualModel.AppRole.IsAdministrator)
          {
            Assert.AreEqual(expectedModel.Tenant.Id, actualModel.Tenant.Id);
            Assert.AreEqual(expectedModel.Tenant.Name, actualModel.Tenant.Name);
          }

          #endregion

        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null)
          {
            _userService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleAddUser_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel nullModel = null;

          var invalidNullFirstName = GetRandomUser(_tenantModelForUser);
          invalidNullFirstName.FirstName = "";

          List<UserModel> invalidMinlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMinLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.FirstName = item;
            invalidMinlengthFirstName.Add(user);
          }

          List<UserModel> invalidMaxlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.FirstName = item;
            invalidMaxlengthFirstName.Add(user);
          }

          var invalidNullLastName = GetRandomUser(_tenantModelForUser);
          invalidNullLastName.LastName = "";

          List<UserModel> invalidMinlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMinLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.LastName = item;
            invalidMinlengthLastName.Add(user);
          }

          List<UserModel> invalidMaxlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.LastName = item;
            invalidMaxlengthLastName.Add(user);
          }


          var invalidNullEmail = GetRandomUser(_tenantModelForUser);
          invalidNullEmail.Email = "";

          var invalidEmailIsAlreadyExists = GetRandomUser(_tenantModelForUser);
          invalidEmailIsAlreadyExists.Email = _userModel.Email;

          List<UserModel> invalidMaxlengthEmail = new List<UserModel>();
          foreach (var item in InvalidMaxlengthEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Email = item;
            invalidMaxlengthEmail.Add(user);
          }

          List<UserModel> invalidFormatIncorrecEmail = new List<UserModel>();
          foreach (var item in InvalidFormatIncorrecEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Email = item;
            invalidFormatIncorrecEmail.Add(user);
          }

          var invalidRoleIsAdminAndTenantNotNull = GetRandomUser(_tenantModelForUser);
          invalidRoleIsAdminAndTenantNotNull.AppRole.IsAdministrator = true;
          invalidRoleIsAdminAndTenantNotNull.Tenant = _tenantModelForUser;

          List<InvalidModel<UserModel>> invalidModels = new List<InvalidModel<UserModel>>();
          invalidModels.Add(new InvalidModel<UserModel>(nameof(nullModel), nullModel));

          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullFirstName), invalidNullFirstName));
          foreach (var item in invalidMinlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthFirstName), item));
          }

          foreach (var item in invalidMaxlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthFirstName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullLastName), invalidNullLastName));
          foreach (var item in invalidMinlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthLastName), item));
          }
          foreach (var item in invalidMaxlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthLastName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullEmail), invalidNullEmail));
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidEmailIsAlreadyExists), invalidEmailIsAlreadyExists));
          foreach (var item in invalidMaxlengthEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthEmail), item));
          }
          foreach (var item in invalidFormatIncorrecEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidFormatIncorrecEmail), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidRoleIsAdminAndTenantNotNull), invalidRoleIsAdminAndTenantNotNull));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _userService.Create(invalidModel.Value, "123456");

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_FIRST_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMinlengthFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MIN_LENGTH_OF_FIRST_NAME_IS_3, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_FIRST_NAME_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LAST_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMinlengthLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MIN_LENGTH_OF_LAST_NAME_IS_3, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_LAST_NAME_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidEmailIsAlreadyExists):
                Assert.AreEqual(ErrorType.DUPLICATED, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_ENTERED_EMAIL_ALREADY_EXISTS, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidRoleIsAdminAndTenantNotNull):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.IF_THE_USER_IS_AN_ADMINISTRATOR_THE_TENANT_MUST_BE_EMPTY, actualModel.Error.Message);
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

    [TestMethod()]
    public void UserRoleAddUser_NoPermission_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {

          #region Init data

          UserModel model = GetRandomUser(_tenantModelForUser);
          var createdModel = _userService.Create(model, "123456");

          #endregion

          #region Get and check data

          Assert.IsNotNull(createdModel.Error);
          Assert.IsNull(createdModel.Data);
          Assert.AreEqual(ErrorType.NO_ROLE, createdModel.Error.Type);
          Assert.AreEqual(NO_ROLE, createdModel.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    #endregion

    #region UPDATE_USER

    [TestMethod]
    public void SysAdminRoleUpdateUser_Valid_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel model = GetRandomUser(_tenantModelForUser);
          var createdModel = _userService.Create(model, "123456");

          UserModel actual = GetRandomUser(_tenantModelForUser);
          actual.Email = createdModel.Data.Email;
          actual.Id = createdModel.Data.Id;

          var expectedModel = _userService.Update(actual);

          #endregion

          #region Get and check data

          Assert.IsNotNull(expectedModel.Data);
          param.CleanData.Add("CreateModel", expectedModel.Data);
          var actualModel = _userService.GetById(expectedModel.Data.Id).Data;

          Assert.IsNotNull(actualModel);
          Assert.AreEqual(expectedModel.Data.Id, actualModel.Id);
          Assert.AreEqual(expectedModel.Data.FirstName, actualModel.FirstName);
          Assert.AreEqual(expectedModel.Data.LastName, actualModel.LastName);
          Assert.AreEqual(expectedModel.Data.Email, actualModel.Email);
          Assert.AreEqual(expectedModel.Data.Comment, actualModel.Comment);
          Assert.AreEqual(expectedModel.Data.IsEnabled, actualModel.IsEnabled);
          Assert.AreEqual(expectedModel.Data.AppRole.Id, actualModel.AppRole.Id);
          Assert.AreEqual(expectedModel.Data.AppRole.IsAdministrator, actualModel.AppRole.IsAdministrator);
          Assert.AreEqual(expectedModel.Data.AppRole.Code, actualModel.AppRole.Code);
          Assert.AreEqual(expectedModel.Data.Language.Id, actualModel.Language.Id);
          Assert.AreEqual(expectedModel.Data.Language.DisplayName, actualModel.Language.DisplayName);
          if (!actualModel.AppRole.IsAdministrator)
          {
            Assert.AreEqual(expectedModel.Data.Tenant.Id, actualModel.Tenant.Id);
            Assert.AreEqual(expectedModel.Data.Tenant.Name, actualModel.Tenant.Name);
          }
          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null && createModel.Id > 0)
          {
            _userService.Delete(createModel.Id);
          }
        });

    }

    [TestMethod]
    public void SysAdminRoleUpdateUser_InvalidModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel nullModel = null;

          UserModel invalidNullFirstName = GetRandomUser(_tenantModelForUser);
          invalidNullFirstName.FirstName = "";

          List<UserModel> invalidMinlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMinLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.FirstName = item;
            invalidMinlengthFirstName.Add(user);
          }

          List<UserModel> invalidMaxlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.FirstName = item;
            invalidMaxlengthFirstName.Add(user);
          }

          UserModel invalidNullLastName = GetRandomUser(_tenantModelForUser);
          invalidNullLastName.LastName = "";

          List<UserModel> invalidMinlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMinLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.LastName = item;
            invalidMinlengthLastName.Add(user);
          }

          List<UserModel> invalidMaxlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.LastName = item;
            invalidMaxlengthLastName.Add(user);
          }

          UserModel invalidNullEmail = GetRandomUser(_tenantModelForUser);
          invalidNullEmail.Email = "";

          UserModel invalidIdNotFound = GetRandomUser(_tenantModelForUser);
          invalidIdNotFound.Id = 0;

          List<UserModel> invalidMaxlengthEmail = new List<UserModel>();
          foreach (var item in InvalidMaxlengthEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Email = item;
            invalidMaxlengthEmail.Add(user);
          }

          List<UserModel> invalidFormatIncorrecEmail = new List<UserModel>();
          foreach (var item in InvalidFormatIncorrecEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Email = item;
            invalidFormatIncorrecEmail.Add(user);
          }

          UserModel modelForCreate = GetRandomUser(_tenantModelForUser);
          var createModel = _userService.Create(modelForCreate, "123456");

          var invalidRoleIsAdminAndTenantNotNull = _userService.GetById(createModel.Data.Id).Data;
          invalidRoleIsAdminAndTenantNotNull.AppRole.IsAdministrator = true;
          invalidRoleIsAdminAndTenantNotNull.Tenant = _tenantModelForUser;
          var updateModel = _userService.Update(invalidRoleIsAdminAndTenantNotNull);

          param.CleanData.Add("CreateModel", updateModel.Data);

          List<InvalidModel<UserModel>> invalidModels = new List<InvalidModel<UserModel>>();
          invalidModels.Add(new InvalidModel<UserModel>(nameof(nullModel), nullModel));
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidIdNotFound), invalidIdNotFound));
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullFirstName), invalidNullFirstName));
          foreach (var item in invalidMinlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthFirstName), item));
          }

          foreach (var item in invalidMaxlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthFirstName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullLastName), invalidNullLastName));
          foreach (var item in invalidMinlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthLastName), item));
          }
          foreach (var item in invalidMaxlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthLastName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullEmail), invalidNullEmail));
          foreach (var item in invalidMaxlengthEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthEmail), item));
          }
          foreach (var item in invalidFormatIncorrecEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidFormatIncorrecEmail), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidRoleIsAdminAndTenantNotNull), invalidRoleIsAdminAndTenantNotNull));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _userService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_FIRST_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMinlengthFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MIN_LENGTH_OF_FIRST_NAME_IS_3, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_FIRST_NAME_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LAST_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMinlengthLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MIN_LENGTH_OF_LAST_NAME_IS_3, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_LAST_NAME_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidIdNotFound):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.ID_DOES_NOT_MATCH, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidRoleIsAdminAndTenantNotNull):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.IF_THE_USER_IS_AN_ADMINISTRATOR_THE_TENANT_MUST_BE_EMPTY, actualModel.Error.Message);
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
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null)
          {
            _userService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void AdminRoleUpdateUser_Valid_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel model = GetRandomUser(_tenantModelForUser);
          var createdModel = _userService.Create(model, "123456");

          UserModel actual = GetRandomUser(_tenantModelForUser);
          actual.Email = createdModel.Data.Email;
          actual.Id = createdModel.Data.Id;

          var expectedModel = _userService.Update(actual);

          #endregion

          #region Get and check data

          Assert.IsNotNull(expectedModel.Data);
          param.CleanData.Add("CreateModel", expectedModel.Data);
          var actualModel = _userService.GetById(expectedModel.Data.Id).Data;

          Assert.IsNotNull(actualModel);
          Assert.AreEqual(expectedModel.Data.Id, actualModel.Id);
          Assert.AreEqual(expectedModel.Data.FirstName, actualModel.FirstName);
          Assert.AreEqual(expectedModel.Data.LastName, actualModel.LastName);
          Assert.AreEqual(expectedModel.Data.Email, actualModel.Email);
          Assert.AreEqual(expectedModel.Data.Comment, actualModel.Comment);
          Assert.AreEqual(expectedModel.Data.IsEnabled, actualModel.IsEnabled);
          Assert.AreEqual(expectedModel.Data.AppRole.Id, actualModel.AppRole.Id);
          Assert.AreEqual(expectedModel.Data.AppRole.IsAdministrator, actualModel.AppRole.IsAdministrator);
          Assert.AreEqual(expectedModel.Data.AppRole.Code, actualModel.AppRole.Code);
          Assert.AreEqual(expectedModel.Data.Language.Id, actualModel.Language.Id);
          Assert.AreEqual(expectedModel.Data.Language.DisplayName, actualModel.Language.DisplayName);
          if (!actualModel.AppRole.IsAdministrator)
          {
            Assert.AreEqual(expectedModel.Data.Tenant.Id, actualModel.Tenant.Id);
            Assert.AreEqual(expectedModel.Data.Tenant.Name, actualModel.Tenant.Name);
          }
          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null && createModel.Id > 0)
          {
            _userService.Delete(createModel.Id);
          }
        });

    }

    [TestMethod]
    public void AdminRoleUpdateUser_InvalidModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel nullModel = null;

          UserModel invalidNullFirstName = GetRandomUser(_tenantModelForUser);
          invalidNullFirstName.FirstName = "";

          List<UserModel> invalidMinlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMinLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.FirstName = item;
            invalidMinlengthFirstName.Add(user);
          }

          List<UserModel> invalidMaxlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.FirstName = item;
            invalidMaxlengthFirstName.Add(user);
          }

          UserModel invalidNullLastName = GetRandomUser(_tenantModelForUser);
          invalidNullLastName.LastName = "";

          List<UserModel> invalidMinlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMinLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.LastName = item;
            invalidMinlengthLastName.Add(user);
          }

          List<UserModel> invalidMaxlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.LastName = item;
            invalidMaxlengthLastName.Add(user);
          }

          UserModel invalidNullEmail = GetRandomUser(_tenantModelForUser);
          invalidNullEmail.Email = "";

          UserModel invalidIdNotFound = GetRandomUser(_tenantModelForUser);
          invalidIdNotFound.Id = 0;

          List<UserModel> invalidMaxlengthEmail = new List<UserModel>();
          foreach (var item in InvalidMaxlengthEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Email = item;
            invalidMaxlengthEmail.Add(user);
          }

          List<UserModel> invalidFormatIncorrecEmail = new List<UserModel>();
          foreach (var item in InvalidFormatIncorrecEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Email = item;
            invalidFormatIncorrecEmail.Add(user);
          }

          UserModel modelForCreate = GetRandomUser(_tenantModelForUser);
          var createModel = _userService.Create(modelForCreate, "123456");

          var invalidRoleIsAdminAndTenantNotNull = _userService.GetById(createModel.Data.Id).Data;
          invalidRoleIsAdminAndTenantNotNull.AppRole.IsAdministrator = true;
          invalidRoleIsAdminAndTenantNotNull.Tenant = _tenantModelForUser;
          var updateModel = _userService.Update(invalidRoleIsAdminAndTenantNotNull);

          param.CleanData.Add("CreateModel", updateModel.Data);

          List<InvalidModel<UserModel>> invalidModels = new List<InvalidModel<UserModel>>();
          invalidModels.Add(new InvalidModel<UserModel>(nameof(nullModel), nullModel));
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidIdNotFound), invalidIdNotFound));
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullFirstName), invalidNullFirstName));
          foreach (var item in invalidMinlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthFirstName), item));
          }

          foreach (var item in invalidMaxlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthFirstName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullLastName), invalidNullLastName));
          foreach (var item in invalidMinlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthLastName), item));
          }
          foreach (var item in invalidMaxlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthLastName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullEmail), invalidNullEmail));
          foreach (var item in invalidMaxlengthEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthEmail), item));
          }
          foreach (var item in invalidFormatIncorrecEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidFormatIncorrecEmail), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidRoleIsAdminAndTenantNotNull), invalidRoleIsAdminAndTenantNotNull));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _userService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            switch (invalidModel.Key)
            {
              case nameof(nullModel):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Error.Message);
                break;
              case nameof(invalidNullFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_FIRST_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMinlengthFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MIN_LENGTH_OF_FIRST_NAME_IS_3, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthFirstName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_FIRST_NAME_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_LAST_NAME_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidMinlengthLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MIN_LENGTH_OF_LAST_NAME_IS_3, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthLastName):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_LAST_NAME_IS_50, actualModel.Error.Message);
                break;
              case nameof(invalidNullEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_EMAIL_IS_REQUIRED, actualModel.Error.Message);
                break;
              case nameof(invalidIdNotFound):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.ID_DOES_NOT_MATCH, actualModel.Error.Message);
                break;
              case nameof(invalidMaxlengthEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, actualModel.Error.Message);
                break;
              case nameof(invalidFormatIncorrecEmail):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, actualModel.Error.Message);
                break;
              case nameof(invalidRoleIsAdminAndTenantNotNull):
                Assert.AreEqual(ErrorType.BAD_REQUEST, actualModel.Error.Type);
                Assert.AreEqual(LangKey.IF_THE_USER_IS_AN_ADMINISTRATOR_THE_TENANT_MUST_BE_EMPTY, actualModel.Error.Message);
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
          var createModel = param.CleanData.Get<UserModel>("CreateModel");
          if (createModel != null)
          {
            _userService.Delete(createModel.Id);
          }
        });
    }

    [TestMethod]
    public void UserRoleUpdateUser_Valid_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel actual = GetRandomUser(_tenantModelForUser);
          actual.Email = _userModel.Email;
          actual.Id = _userModel.Id;

          var actualModel = _userService.Update(actual);

          #endregion

          #region Get and check data

          Assert.IsNotNull(actualModel);
          Assert.IsNull(actualModel.Data);

          Assert.AreEqual(ErrorType.NO_ROLE, actualModel.Error.Type);
          Assert.AreEqual("No role", actualModel.Error.Message);
          #endregion
        })
        .ThenCleanDataTest(param =>
        {
        });
    }

    [TestMethod]
    public void UserRoleUpdateUser_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init Data

          UserModel nullModel = null;

          UserModel invalidIdNotFound = GetRandomUser(_tenantModelForUser);
          invalidIdNotFound.Id = 0;

          UserModel invalidNullFirstName = GetRandomUser(_tenantModelForUser);
          invalidNullFirstName.Id = _userModel.Id;
          invalidNullFirstName.FirstName = "";

          List<UserModel> invalidMinlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMinLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Id = _userModel.Id;
            user.FirstName = item;
            invalidMinlengthFirstName.Add(user);
          }

          List<UserModel> invalidMaxlengthFirstName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthFirstName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Id = _userModel.Id;
            user.FirstName = item;
            invalidMaxlengthFirstName.Add(user);
          }

          UserModel invalidNullLastName = GetRandomUser(_tenantModelForUser);
          invalidNullLastName.Id = _userModel.Id;
          invalidNullLastName.LastName = "";

          List<UserModel> invalidMinlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMinLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Id = _userModel.Id;
            user.LastName = item;
            invalidMinlengthLastName.Add(user);
          }

          List<UserModel> invalidMaxlengthLastName = new List<UserModel>();
          foreach (var item in InvalidMaxLengthLastName())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Id = _userModel.Id;
            user.LastName = item;
            invalidMaxlengthLastName.Add(user);
          }

          UserModel invalidNullEmail = GetRandomUser(_tenantModelForUser);
          invalidNullEmail.Id = _userModel.Id;
          invalidNullEmail.Email = "";

          List<UserModel> invalidMaxlengthEmail = new List<UserModel>();
          foreach (var item in InvalidMaxlengthEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Id = _userModel.Id;
            user.Email = item;
            invalidMaxlengthEmail.Add(user);
          }

          List<UserModel> invalidFormatIncorrecEmail = new List<UserModel>();
          foreach (var item in InvalidFormatIncorrecEmail())
          {
            var user = GetRandomUser(_tenantModelForUser);
            user.Id = _userModel.Id;
            user.Email = item;
            invalidFormatIncorrecEmail.Add(user);
          }

          UserModel invalidRoleIsAdminAndTenantNotNull = _userService.GetById(_userModel.Id).Data;
          invalidRoleIsAdminAndTenantNotNull.AppRole.IsAdministrator = true;
          invalidRoleIsAdminAndTenantNotNull.Tenant = _tenantModelForUser;

          List<InvalidModel<UserModel>> invalidModels = new List<InvalidModel<UserModel>>();
          invalidModels.Add(new InvalidModel<UserModel>(nameof(nullModel), nullModel));
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidIdNotFound), invalidIdNotFound));
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullFirstName), invalidNullFirstName));
          foreach (var item in invalidMinlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthFirstName), item));
          }

          foreach (var item in invalidMaxlengthFirstName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthFirstName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullLastName), invalidNullLastName));
          foreach (var item in invalidMinlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMinlengthLastName), item));
          }
          foreach (var item in invalidMaxlengthLastName)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthLastName), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidNullEmail), invalidNullEmail));
          foreach (var item in invalidMaxlengthEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidMaxlengthEmail), item));
          }
          foreach (var item in invalidFormatIncorrecEmail)
          {
            invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidFormatIncorrecEmail), item));
          }
          invalidModels.Add(new InvalidModel<UserModel>(nameof(invalidRoleIsAdminAndTenantNotNull), invalidRoleIsAdminAndTenantNotNull));

          #endregion

          #region Action

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _userService.Update(invalidModel.Value);

            Assert.IsNotNull(actualModel);
            Assert.IsNull(actualModel.Data);

            Assert.AreEqual(ErrorType.NO_ROLE, actualModel.Error.Type);
            Assert.AreEqual("No role", actualModel.Error.Message);

          }

          #endregion
        })
        .ThenCleanDataTest(param =>
        {

        });
    }

    #endregion

    #region DETELE_USER

    [TestMethod]
    public void SysAdminRoleDeteleUser_ValidUserId_Success()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel model = GetRandomUser(_tenantModelForUser);
          var createdModel = _userService.Create(model, "123456");
          var result = _userService.Delete(createdModel.Data.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.AreEqual(true, result.Data);

          var actualModel = _userService.GetById(createdModel.Data.Id);
          Assert.IsNull(actualModel.Data);
          Assert.IsNotNull(actualModel.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, actualModel.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod]
    public void SysAdminRoleDeteleUser_InValidUserId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _userService.Delete(0);

          UserModel model = GetRandomUser(_tenantModelForUser);
          var createdModel = _userService.Create(model, "123456");
          _userService.Delete(createdModel.Data.Id);
          var delete2 = _userService.Delete(createdModel.Data.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Error);
          Assert.IsNotNull(result.Data);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          Assert.IsNotNull(delete2.Error);
          Assert.IsNotNull(delete2.Data);
          Assert.AreEqual(ErrorType.NOT_EXIST, delete2.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, delete2.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod]
    public void AdminRoleDeteleUser_ValidUserId_Success()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          UserModel model = GetRandomUser(_tenantModelForUser);
          var createdModel = _userService.Create(model, "123456");
          var result = _userService.Delete(createdModel.Data.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Data);
          Assert.IsNull(result.Error);
          Assert.AreEqual(true, result.Data);

          var actualModel = _userService.GetById(createdModel.Data.Id);
          Assert.IsNull(actualModel.Data);
          Assert.IsNotNull(actualModel.Error);
          Assert.AreEqual(ErrorType.NOT_EXIST, actualModel.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, actualModel.Error.Message);

          #endregion

        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod]
    public void AdminRoleDeteleUser_InValidUserId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          var result = _userService.Delete(0);

          UserModel model = GetRandomUser(_tenantModelForUser);
          var createdModel = _userService.Create(model, "123456");
          _userService.Delete(createdModel.Data.Id);
          var delete2 = _userService.Delete(createdModel.Data.Id);

          #endregion

          #region Get and check data

          Assert.IsNotNull(result.Error);
          Assert.IsNotNull(result.Data);
          Assert.AreEqual(ErrorType.NOT_EXIST, result.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, result.Error.Message);

          Assert.IsNotNull(delete2.Error);
          Assert.IsNotNull(delete2.Data);
          Assert.AreEqual(ErrorType.NOT_EXIST, delete2.Error.Type);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID_, delete2.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param => { });
    }

    [TestMethod()]
    public void UserRoleDeleteUser_NoPermission_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {

          #region Init data

          UserModel model = GetRandomUser(_tenantModelForUser);
          var createdModel = _userService.Delete(model.Id);

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

    private List<string> InvalidMinLengthFirstName()
    {
      List<string> userNames = new List<string>()
      {
        "a",
        "ab",
        "cd",
        "1",
        "#$",
        "zs"
      };
      return userNames;
    }

    private List<string> InvalidMaxLengthFirstName()
    {
      List<string> firstName = new List<string>()
      {
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
        "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
        "ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc"
      };
      return firstName;
    }

    private List<string> InvalidMinLengthLastName()
    {
      List<string> userNames = new List<string>()
      {
        "a",
        "ab",
        "cd",
        "1",
        "#$",
        "zs"
      };
      return userNames;
    }

    private List<string> InvalidMaxLengthLastName()
    {
      List<string> lastName = new List<string>()
      {
        "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
        "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
        "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
        "11111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111",
        "ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc"
      };
      return lastName;
    }

    private List<string> InvalidMaxlengthEmail()
    {
      List<string> userNames = new List<string>()
      {
        "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh@gmail.com",
        "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzaa@gmail.com",
        "aaaaaaaaaaaa123qaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@gmail.com",
        "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbb42342bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbvvvvvvvvvvvv@gmail.com",
        "ccccccccccccccccccccccccccccccefwecccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc@gmail.com"
      };
      return userNames;
    }

    private List<string> InvalidFormatIncorrecEmail()
    {
      List<string> userNames = new List<string>()
      {
        "abcdef@gmail..com",
        "aav@dc@gmail.com",
        "aa*6vdz@gmail.com",
        "qqqq@$%^&*(@gmail.com",
        "!@$%$%^^&*@gmail.com",
        "!#$@#$%%^&"
      };
      return userNames;
    }
    #endregion
  }

  class InvalidModel<T> where T : class
  {
    public InvalidModel(string key, T value)
    {
      Key = key;
      Value = value;
    }
    public string Key { get; set; }
    public T Value { get; set; }
  }
}
