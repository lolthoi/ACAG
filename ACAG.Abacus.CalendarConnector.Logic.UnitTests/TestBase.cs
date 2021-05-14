using System;
using System.Collections.Generic;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests
{

  [TestClass()]
  public abstract class TestBase
  {
    #region Variables and Constructors

    protected IServiceScope _scope;
    protected AccountSetting _accountSetting;
    protected ITestService _testService;

    private List<LanguageModel> _cultures;
    private AppRoleViewModel _appRole;

    public TestBase()
    {
      var startup = Startup.Instance;
      _accountSetting = startup.Options.Value;
      _scope = startup.ServiceProvider.CreateScope();
      _testService = _scope.ServiceProvider.GetService<ITestService>();
      
    }

    #endregion

    #region Init and Clean services

    protected abstract void InitServices();

    protected abstract void InitEnvirontment();

    protected abstract void CleanEnvirontment();

    [TestInitialize]
    public void TestInitialize()
    {
      InitServices();
      InitEnvirontment();
    }

    [TestCleanup]
    public void TestCleanUp()
    {
      CleanEnvirontment();
    }

    #endregion

    protected LanguageModel GetRandomCulture()
    {
      if (_cultures == null)
      {
        var cultureService = _scope.ServiceProvider.GetService<ICultureService>();
        _cultures = cultureService.GetAll().Data;
      }

      Random random = new Random();
      var index = random.Next(_cultures.Count);
      var data = _cultures[index];
      return data;
    }

    protected AppRoleModel GetRandomAppRole()
    {
      if (_appRole == null)
      {
        var roleService = _scope.ServiceProvider.GetService<IRoleService>();
        _appRole = roleService.GetAll().Data;
      }

      Random random = new Random();
      var index = random.Next(_appRole.Roles.Count);
      var data = _appRole.Roles[index];
      return data;
    }

    protected AppRoleModel GetUserAppRole(Roles role)
    {
      if (_appRole == null)
      {
        var roleService = _scope.ServiceProvider.GetService<IRoleService>();
        _appRole = roleService.GetAll().Data;
      }

      switch (role)
      {
        case Roles.ADMINISTRATOR:
          return _appRole.Roles.Find(t => t.IsAdministrator);
        case Roles.USER:
        default:
          return _appRole.Roles.Find(t => !t.IsAdministrator);
      }  
    }

    protected UserModel GetRandomUser(TenantModelForUser tenant)
    {
      var firstName = "user" + Guid.NewGuid().ToString().Replace("-", "");
      var lastName = "user" + Guid.NewGuid().ToString().Replace("-", "");
      var email = lastName + "@kloon.vn";
      var culture = GetRandomCulture();
      var appRole = GetUserAppRole(Roles.USER);

      var user = new UserModel()
      {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        Comment = "abc test",
        IsEnabled = true,
        Language = new LanguageModel()
        {
          Id = culture.Id,
          DisplayName = culture.DisplayName,
          Code = culture.Code
        },
        AppRole = appRole,
        Tenant = new TenantModelForUser()
        {
          Id = tenant.Id,
          Name = tenant.Name
        },
      };

      return user;
    }

    protected UserModel GetRandomAdminUser()
    {
      var firstName = "user" + Guid.NewGuid().ToString().Replace("-", "");
      var lastName = "user" + Guid.NewGuid().ToString().Replace("-", "");
      var email = firstName + lastName + "@kloon.vn";
      var culture = GetRandomCulture();
      var appRole = GetUserAppRole(Roles.ADMINISTRATOR);

      var user = new UserModel()
      {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        Comment = "abc test",
        IsEnabled = true,
        Language = new LanguageModel()
        {
          Id = culture.Id,
          DisplayName = culture.DisplayName
        },
        AppRole = appRole
      };

      return user;
    }
  }
}
