using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests
{
  public class AuthenTestLogin : IIdentityService
  {
    private readonly ITestService _testLogic;

    public AuthenTestLogin(
      ITestService testLogic)
    {
      _testLogic = testLogic;
    }

    public LoginUserModel GetCurrentUser()
    {
      return _testLogic.GetLoginUser();
    }
  }

  public interface ITestService
  {
    LoginUserModel GetLoginUser();

    TestResult StartLoginWithAdmin();

    TestResult StartLoginWithUser(string username, string password);
    TestResult StartLoginWithSysAdmin();
  }

  public class TestService : ITestService
  {
    private readonly ConcurrentDictionary<string, TestResult> _testResults = new ConcurrentDictionary<string, TestResult>();
    private TestResult _lastTestResult;
    private readonly AccountSetting _accountSetting;
    private readonly IAuthenticationService _authenService;

    public TestService(IOptions<AccountSetting> options)
    {
      _accountSetting = options.Value;
      _authenService = Startup.Instance.ServiceProvider.GetService<IAuthenticationService>();
    }

    public LoginUserModel GetLoginUser()
    {
      return _lastTestResult.DataParam.CurrentUser;
    }

    public TestResult StartLoginWithAdmin()
    {
      return Login(_accountSetting.Admin.UserName, _accountSetting.Admin.Password);
    }

    public TestResult StartLoginWithUser(string username, string password)
    {
      return Login(username, password);
    }


    public TestResult StartLoginWithSysAdmin()
    {
      return Login(_accountSetting.SysAdmin.UserName, _accountSetting.SysAdmin.Password);
    }

    private TestResult Login(string username, string password)
    {
      if (_testResults.TryGetValue(username, out TestResult testResult))
      {
        _lastTestResult = testResult;
        return testResult;
      }

      var user = _authenService.Login(username, password);
      if (user.Error != null)
      {
        throw new System.Exception(user.Error.Message);
      }

      var loginUser = new LoginUserModel
      {
        Key = "test",
        User = new LoginUserModel.LoginUser
        {
          Id = user.Data.Id,
          Email = user.Data.Email,
          FirstName = user.Data.FirstName,
          LastName = user.Data.LastName,
          TenantId = user.Data.TenantId,
          Role = ConvertToRole(user.Data.Role)
        }
      };

      var result = new TestResult(this, loginUser);
      _lastTestResult = result;
      _testResults.TryAdd(username, result);
      return result;
    }

    private static Roles ConvertToRole(string role)
    {
      switch (role)
      {
        case "Administrator":
          return Roles.ADMINISTRATOR;
        case "SysAdmin":
          return Roles.SYSADMIN;
        default:
        case "User":
          return Roles.USER;
      }
    }
  }

  public class TestResult
  {

    private readonly ITestService _testService;
    private bool isSuccessTest = true;
    public readonly DataParam DataParam;

    public TestResult(ITestService testService, LoginUserModel currentUser)
    {
      _testService = testService;
      DataParam = new DataParam(currentUser);
    }


    public TestResult ThenImplementTest(Action<DataParam> action)
    {
      try
      {
        action.Invoke(DataParam);
      }
      catch
      {
        isSuccessTest = false;
        return this;
      }

      return this;
    }
    public ITestService ThenCleanDataTest(Action<DataParam> cleanAction)
    {
      try
      {
        if (DataParam.CleanData.HasValue)
        {
          cleanAction.Invoke(DataParam);
        }
      }
      catch
      {
        isSuccessTest = false;
      }

      if (!isSuccessTest)
      {
        Assert.Fail();
      }
      return _testService;
    }
  }

  public interface IClean
  {
    void Add(string key, object data);

    T Get<T>(string key);

    bool HasValue { get; }
  }

  public class DataParam
  {
    public LoginUserModel CurrentUser { get; private set; }

    public IClean CleanData { get; private set; }

    public DataParam(LoginUserModel currentData)
    {
      CurrentUser = currentData;
      CleanData = new Clean();
    }

    protected class Clean : IClean
    {
      private Dictionary<string, object> _dics = new Dictionary<string, object>();

      public void Add(string key, object data)
      {
        _dics[key] = data;
      }

      public T Get<T>(string key)
      {
        if (_dics.TryGetValue(key, out object data))
        {
          return (T)data;
        }
        return default(T);
      }

      public bool HasValue { get { return _dics.Count > 0; } }
    }
  }
}
