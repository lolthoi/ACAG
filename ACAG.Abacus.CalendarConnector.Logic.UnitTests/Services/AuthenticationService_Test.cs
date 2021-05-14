using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services
{
  [TestClass()]
  public class AuthenticationService_Test : TestBase
  {
    #region Variable and Constructor

    private ITenantService _tenantService;
    private IUserService _userService;
    private IAuthenticationService _authenticationService;

    private TenantModel _tenantModel;
    private TenantModelForUser _tenantModelForUser;
    private UserModel _userModel;
    private UserModel _userModel2;

    #endregion

    public AuthenticationService_Test()
    {
    }

    protected override void InitServices()
    {
      _userService = _scope.ServiceProvider.GetService<IUserService>();
      _tenantService = _scope.ServiceProvider.GetService<ITenantService>();
      _authenticationService = _scope.ServiceProvider.GetService<IAuthenticationService>();
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

          _userModel2 = GetRandomUser(_tenantModelForUser);
          _userModel2.IsEnabled = false;
          _userService.Create(_userModel2, "123456");
        });
    }

    protected override void CleanEnvirontment()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          _userService.Delete(_userModel.Id);
          _userService.Delete(_userModel2.Id);
          _tenantService.Delete(_tenantModel.Id);
        });
    }

    #region GET BY EMAIL

    [TestMethod]
    public void GetByEmail_Success()
    {
      try
      {
        #region Init data

        var createModel = _authenticationService.GetByEmail(_userModel.Email);

        #endregion

        #region Get and check data

        Assert.IsNotNull(createModel.Data);
        Assert.IsNull(createModel.Error);
        Assert.AreEqual(_userModel.Email, createModel.Data.Email);
        Assert.AreEqual(_userModel.FirstName, createModel.Data.FirstName);
        Assert.AreEqual(_userModel.LastName, createModel.Data.LastName);
        Assert.AreEqual(_userModel.Language.Id, createModel.Data.CultureId);

        #endregion
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    #endregion

    #region LOGIN

    [TestMethod]
    public void Login_Success()
    {
      try
      {

        #region Init data

        LoginModel loginModel = new LoginModel()
        {
          Email = _userModel.Email,
          Password = "123456",
        };
        var actualModel = _authenticationService.Login(loginModel.Email, loginModel.Password);

        #endregion

        #region Get and check data

        Assert.IsNotNull(actualModel.Data);
        Assert.IsNull(actualModel.Error);
        Assert.AreEqual(_userModel.Email, actualModel.Data.Email);
        Assert.AreEqual(_userModel.FirstName, actualModel.Data.FirstName);
        Assert.AreEqual(_userModel.LastName, actualModel.Data.LastName);
        Assert.AreEqual(_userModel.Language.Id, actualModel.Data.CultureId);

        #endregion
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    #endregion

    #region Forgot Password

    [TestMethod]
    public void ForgotPassword_Success()
    {
      try
      {

        #region Init data

        ForgotPasswordModel forgotPassword = new ForgotPasswordModel()
        {
          Email = _userModel.Email
        };
        var actualModel = _authenticationService.ForgotPassword(forgotPassword);

        #endregion

        #region Get and check data

        Assert.IsNotNull(actualModel.Data);
        Assert.IsNull(actualModel.Error);
        Assert.AreEqual(_userModel.Email, actualModel.Data.Email);
        Assert.AreEqual(_userModel.Language.Code, actualModel.Data.LanguageCode);

        #endregion

        #region Clean data
        if (actualModel != null && actualModel.Data != null)
        {
          _authenticationService.ResetPassword(new ResetPasswordModel()
          {
            Code = actualModel.Data.Code,
            Password = "123456",
            ConfirmPassword = "123456",
            Status = true
          });
        }
        #endregion
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }
    #endregion

    #region ResetPassword

    [TestMethod]
    public void ResetPassword_Success()
    {
      try
      {

        #region Init data

        ForgotPasswordModel forgotPassword = new ForgotPasswordModel()
        {
          Email = _userModel.Email
        };
        var createModel = _authenticationService.ForgotPassword(forgotPassword);
        var resetPasswordModel = new ResetPasswordModel()
        {
          Code = createModel.Data.Code,
          Password = "123456",
          ConfirmPassword = "123456",
          Status = true
        };
        var actualModel = _authenticationService.ResetPassword(resetPasswordModel);

        #endregion

        #region Get and check data

        Assert.IsNotNull(actualModel.Data);
        Assert.IsNull(actualModel.Error);
        Assert.IsTrue(actualModel.Data.Status);

        #endregion
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    #endregion

    #region Check Code For Reset Password

    [TestMethod]
    public void CheckCodeForResetPassword_Success()
    {
      try
      {

        #region Init data

        ForgotPasswordModel forgotPassword = new ForgotPasswordModel()
        {
          Email = _userModel.Email
        };
        var createModel = _authenticationService.ForgotPassword(forgotPassword);
        var code = createModel.Data.Code;

        var actualModel = _authenticationService.CheckCodeForResetPassword(code);

        #endregion

        #region Get and check data

        Assert.IsNotNull(actualModel.Data);
        Assert.IsNull(actualModel.Error);
        Assert.IsTrue(actualModel.Data);

        #endregion

        #region Clean data

        if (createModel != null && createModel.Data != null)
        {
          _authenticationService.ResetPassword(new ResetPasswordModel()
          {
            Code = createModel.Data.Code,
            Password = "123456",
            ConfirmPassword = "123456",
            Status = true
          });
        }
        #endregion
      }
      catch (Exception ex)
      {
        Assert.Fail(ex.Message);
      }
    }

    #endregion

    #region Private

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
}
