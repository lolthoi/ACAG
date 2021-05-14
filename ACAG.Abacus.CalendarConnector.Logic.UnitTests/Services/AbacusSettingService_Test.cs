using System;
using System.Collections.Generic;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services
{
  [TestClass()]
  public class AbacusSettingService_Test : TestBase
  {
    private ITenantService _tenantService;
    private IUserService _userService;
    private IAbacusSettingService _abacusSettingService;

    private TenantModel _tenantModel;
    private TenantModelForUser _tenantModelForUser;
    private UserModel _userModel;

    private WrapperAPI _wrapperAPI;

    protected override void InitServices()
    {
      _wrapperAPI = _scope.ServiceProvider.GetService<WrapperAPI>();
      _tenantService = _scope.ServiceProvider.GetService<ITenantService>();
      _userService = _scope.ServiceProvider.GetService<IUserService>();
      _abacusSettingService = _scope.ServiceProvider.GetService<IAbacusSettingService>();
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

    #region GetByTenant(int tenantId);

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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualResult = _abacusSettingService.GetByTenant(createdTenantModel.Data.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.AreEqual(createdAbacusSettingModel.Data.Id, actualResult.Data.Id);
          Assert.AreEqual(createdAbacusSettingModel.Data.Name, actualResult.Data.Name);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServicePort, actualResult.Data.ServicePort);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUrl, actualResult.Data.ServiceUrl);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUser, actualResult.Data.ServiceUser);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUserPassword, actualResult.Data.ServiceUserPassword);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUseSsl, actualResult.Data.ServiceUseSsl);
          Assert.AreEqual(createdAbacusSettingModel.Data.HealthStatus, actualResult.Data.HealthStatus);
          Assert.AreEqual(createdAbacusSettingModel.Data.TenantId, actualResult.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualResult = _abacusSettingService.GetByTenant(createdTenantModel.Data.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.AreEqual(createdAbacusSettingModel.Data.Id, actualResult.Data.Id);
          Assert.AreEqual(createdAbacusSettingModel.Data.Name, actualResult.Data.Name);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServicePort, actualResult.Data.ServicePort);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUrl, actualResult.Data.ServiceUrl);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUser, actualResult.Data.ServiceUser);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUserPassword, actualResult.Data.ServiceUserPassword);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUseSsl, actualResult.Data.ServiceUseSsl);
          Assert.AreEqual(createdAbacusSettingModel.Data.HealthStatus, actualResult.Data.HealthStatus);
          Assert.AreEqual(createdAbacusSettingModel.Data.TenantId, actualResult.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
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

          var abacusSettingModel = MakeAbacusSettingModel(_tenantModel.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualResult = _abacusSettingService.GetByTenant(_tenantModel.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.AreEqual(createdAbacusSettingModel.Data.Id, actualResult.Data.Id);
          Assert.AreEqual(createdAbacusSettingModel.Data.Name, actualResult.Data.Name);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServicePort, actualResult.Data.ServicePort);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUrl, actualResult.Data.ServiceUrl);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUser, actualResult.Data.ServiceUser);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUserPassword, actualResult.Data.ServiceUserPassword);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUseSsl, actualResult.Data.ServiceUseSsl);
          Assert.AreEqual(createdAbacusSettingModel.Data.HealthStatus, actualResult.Data.HealthStatus);
          Assert.AreEqual(createdAbacusSettingModel.Data.TenantId, actualResult.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetByTenant_WithInvalidTenantId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(currentUser =>
        {
          var actualResult = _abacusSettingService.GetByTenant(0);

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
          var actualResult = _abacusSettingService.GetByTenant(0);

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
          var testingResult = _abacusSettingService.GetByTenant(-1);

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

          var actualResult = _abacusSettingService.GetByTenant(createdTenantModel.Data.Id);

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

          var actualResult = _abacusSettingService.GetByTenant(createdTenantModel.Data.Id);

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
          var actualResult = _abacusSettingService.GetByTenant(Int32.MaxValue);

          Assert.IsNull(actualResult.Data);
          Assert.IsNotNull(actualResult.Error);
          Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualResult.Error.Message);
        });
    }

    #endregion

    #region GetById(int id)

    [TestMethod()]
    public void SysAdminRoleGetById_WithExistingAbacusSetting_Success()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualResult = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.AreEqual(createdAbacusSettingModel.Data.Id, actualResult.Data.Id);
          Assert.AreEqual(createdAbacusSettingModel.Data.Name, actualResult.Data.Name);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServicePort, actualResult.Data.ServicePort);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUrl, actualResult.Data.ServiceUrl);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUser, actualResult.Data.ServiceUser);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUserPassword, actualResult.Data.ServiceUserPassword);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUseSsl, actualResult.Data.ServiceUseSsl);
          Assert.AreEqual(createdAbacusSettingModel.Data.HealthStatus, actualResult.Data.HealthStatus);
          Assert.AreEqual(createdAbacusSettingModel.Data.TenantId, actualResult.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetById_WithExistingAbacusSetting_Success()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualResult = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.AreEqual(createdAbacusSettingModel.Data.Id, actualResult.Data.Id);
          Assert.AreEqual(createdAbacusSettingModel.Data.Name, actualResult.Data.Name);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServicePort, actualResult.Data.ServicePort);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUrl, actualResult.Data.ServiceUrl);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUser, actualResult.Data.ServiceUser);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUserPassword, actualResult.Data.ServiceUserPassword);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUseSsl, actualResult.Data.ServiceUseSsl);
          Assert.AreEqual(createdAbacusSettingModel.Data.HealthStatus, actualResult.Data.HealthStatus);
          Assert.AreEqual(createdAbacusSettingModel.Data.TenantId, actualResult.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetById_WithExistingAbacusSetting_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var abacusSettingModel = MakeAbacusSettingModel(_tenantModel.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualResult = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNotNull(actualResult.Data);
          Assert.IsNull(actualResult.Error);
          Assert.AreEqual(createdAbacusSettingModel.Data.Id, actualResult.Data.Id);
          Assert.AreEqual(createdAbacusSettingModel.Data.Name, actualResult.Data.Name);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServicePort, actualResult.Data.ServicePort);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUrl, actualResult.Data.ServiceUrl);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUser, actualResult.Data.ServiceUser);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUserPassword, actualResult.Data.ServiceUserPassword);
          Assert.AreEqual(createdAbacusSettingModel.Data.ServiceUseSsl, actualResult.Data.ServiceUseSsl);
          Assert.AreEqual(createdAbacusSettingModel.Data.HealthStatus, actualResult.Data.HealthStatus);
          Assert.AreEqual(createdAbacusSettingModel.Data.TenantId, actualResult.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetById_WithNonExistingAbacusSetting_Fail()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          var testingResult = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNull(testingResult.Data);
          Assert.IsNull(testingResult.Error);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleGetById_WithNonExistingAbacusSetting_Fail()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          var testingResult = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNull(testingResult.Data);
          Assert.IsNull(testingResult.Error);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleGetById_WithNonExistingAbacusSetting_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var abacusSettingModel = MakeAbacusSettingModel(_tenantModel.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          var testingResult = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNull(testingResult.Data);
          Assert.IsNull(testingResult.Error);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleGetById_WithInvalidAbacusSettingId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(currentUser =>
        {
          var testingResult = _abacusSettingService.GetById(_tenantModel.Id, -1);

          Assert.IsNull(testingResult.Data);
          Assert.IsNotNull(testingResult.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, testingResult.Error.Message);
        });
    }

    [TestMethod()]
    public void AdminRoleGetById_WithInvalidAbacusSettingId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(currentUser =>
        {
          var testingResult = _abacusSettingService.GetById(_tenantModel.Id, -1);

          Assert.IsNull(testingResult.Data);
          Assert.IsNotNull(testingResult.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, testingResult.Error.Message);
        });
    }

    [TestMethod()]
    public void UserRoleGetById_WithInvalidAbacusSettingId_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(currentUser =>
        {
          var testingResult = _abacusSettingService.GetById(_tenantModel.Id, 0);

          Assert.IsNull(testingResult.Data);
          Assert.IsNotNull(testingResult.Error);
          Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, testingResult.Error.Message);
        });
    }

    #endregion

    #region Create(AbacusSettingModel model)

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

          var expectedModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);

          #endregion

          #region Arrange

          var actualModel = _abacusSettingService.Create(expectedModel);

          param.CleanData.Add("actualModel", actualModel.Data);

          Assert.AreEqual(expectedModel.Name, actualModel.Data.Name);
          Assert.AreEqual(expectedModel.ServiceUrl, actualModel.Data.ServiceUrl);
          Assert.AreEqual(expectedModel.ServicePort, actualModel.Data.ServicePort);
          Assert.AreEqual(expectedModel.ServiceUseSsl, actualModel.Data.ServiceUseSsl);
          Assert.AreEqual(expectedModel.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(expectedModel.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(expectedModel.HealthStatus, actualModel.Data.HealthStatus);
          Assert.AreEqual(expectedModel.TenantId, actualModel.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var actualModel = param.CleanData.Get<AbacusSettingModel>("actualModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (actualModel != null)
          {
            _abacusSettingService.Delete(actualModel.TenantId, actualModel.Id);
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

          var expectedModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);

          #endregion

          #region Arrange

          var actualModel = _abacusSettingService.Create(expectedModel);

          param.CleanData.Add("actualModel", actualModel.Data);

          Assert.AreEqual(expectedModel.Name, actualModel.Data.Name);
          Assert.AreEqual(expectedModel.ServiceUrl, actualModel.Data.ServiceUrl);
          Assert.AreEqual(expectedModel.ServicePort, actualModel.Data.ServicePort);
          Assert.AreEqual(expectedModel.ServiceUseSsl, actualModel.Data.ServiceUseSsl);
          Assert.AreEqual(expectedModel.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(expectedModel.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(expectedModel.HealthStatus, actualModel.Data.HealthStatus);
          Assert.AreEqual(expectedModel.TenantId, actualModel.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var actualModel = param.CleanData.Get<AbacusSettingModel>("actualModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (actualModel != null)
          {
            _abacusSettingService.Delete(actualModel.TenantId, actualModel.Id);
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

          var expectedModel = MakeAbacusSettingModel(_tenantModel.Id);

          #endregion

          #region Arrange

          var actualModel = _abacusSettingService.Create(expectedModel);

          param.CleanData.Add("actualModel", actualModel.Data);

          Assert.AreEqual(expectedModel.Name, actualModel.Data.Name);
          Assert.AreEqual(expectedModel.ServiceUrl, actualModel.Data.ServiceUrl);
          Assert.AreEqual(expectedModel.ServicePort, actualModel.Data.ServicePort);
          Assert.AreEqual(expectedModel.ServiceUseSsl, actualModel.Data.ServiceUseSsl);
          Assert.AreEqual(expectedModel.ServiceUser, actualModel.Data.ServiceUser);
          Assert.AreEqual(expectedModel.ServiceUserPassword, actualModel.Data.ServiceUserPassword);
          Assert.AreEqual(expectedModel.HealthStatus, actualModel.Data.HealthStatus);
          Assert.AreEqual(expectedModel.TenantId, actualModel.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var actualModel = param.CleanData.Get<AbacusSettingModel>("actualModel");

          if (actualModel != null)
          {
            _abacusSettingService.Delete(actualModel.TenantId, actualModel.Id);
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

          AbacusSettingModel nullModel = null;

          var nullNameModel = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidName: true,
            isNull: true);

          var nameGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidName: true,
            isInvalidCharacterNumber: true);

          var nullServiceUrlModel = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidServiceUrl: true,
            isNull: true);

          var portSmallerThan1Model = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidServicePort: true,
            isSmallerThan1: true);

          var portGreaterThan99999Model = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidServicePort: true,
            isSmallerThan1: false);

          var urlGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidServiceUrl: true,
            isInvalidCharacterNumber: true);

          var nullServiceUserModel = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidServiceUser: true,
            isNull: true);

          var serviceUserGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidServiceUser: true,
            isInvalidCharacterNumber: true);

          var nullServiceUserPasswordModel = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidServiceUserPassword: true,
            isNull: true);

          var serviceUserPasswordGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
            tenantId: createdTenantModel.Data.Id,
            invalidServiceUserPassword: true,
            isInvalidCharacterNumber: true);

          #endregion

          Dictionary<string, AbacusSettingModel> invalidModels = new Dictionary<string, AbacusSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(nullNameModel), nullNameModel);
          invalidModels.Add(nameof(nameGreaterThen255CharactersModel), nameGreaterThen255CharactersModel);
          invalidModels.Add(nameof(nullServiceUrlModel), nullServiceUrlModel);
          invalidModels.Add(nameof(portSmallerThan1Model), portSmallerThan1Model);
          invalidModels.Add(nameof(portGreaterThan99999Model), portGreaterThan99999Model);
          invalidModels.Add(nameof(urlGreaterThen255CharactersModel), urlGreaterThen255CharactersModel);
          invalidModels.Add(nameof(nullServiceUserModel), nullServiceUserModel);
          invalidModels.Add(nameof(serviceUserGreaterThan255CharactersModel), serviceUserGreaterThan255CharactersModel);
          invalidModels.Add(nameof(nullServiceUserPasswordModel), nullServiceUserPasswordModel);
          invalidModels.Add(nameof(serviceUserPasswordGreaterThan255CharactersModel), serviceUserPasswordGreaterThan255CharactersModel);

          #endregion

          #region Arrange

          Dictionary<string, ResultModel<AbacusSettingModel>> actualModels = new Dictionary<string, ResultModel<AbacusSettingModel>>();

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _abacusSettingService.Create(invalidModel.Value);
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
              case LangKey.MSG_NAME_IS_REQUIRED when actualModel.Key == nameof(nullNameModel):
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255 when actualModel.Key == nameof(nameGreaterThen255CharactersModel):
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_URL_IS_REQUIRED when actualModel.Key == nameof(nullServiceUrlModel):
                Assert.AreEqual(LangKey.MSG_SERVICE_URL_IS_REQUIRED, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portSmallerThan1Model):
                Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portGreaterThan99999Model):
                Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255 when actualModel.Key == nameof(urlGreaterThen255CharactersModel):
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_USER_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserModel):
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255 when actualModel.Key == nameof(serviceUserGreaterThan255CharactersModel):
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserPasswordModel):
                Assert.AreEqual(LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50 when actualModel.Key == nameof(serviceUserPasswordGreaterThan255CharactersModel):
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50, actualModel.Value.Error.Message);
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

            AbacusSettingModel nullModel = null;

            var nullNameModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidName: true,
              isNull: true);

            var nameGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidName: true,
              isInvalidCharacterNumber: true);

            var nullServiceUrlModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidServiceUrl: true,
              isNull: true);

            var portSmallerThan1Model = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidServicePort: true,
              isSmallerThan1: true);

            var portGreaterThan99999Model = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidServicePort: true,
              isSmallerThan1: false);

            var urlGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidServiceUrl: true,
              isInvalidCharacterNumber: true);

            var nullServiceUserModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidServiceUser: true,
              isNull: true);

            var serviceUserGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidServiceUser: true,
              isInvalidCharacterNumber: true);

            var nullServiceUserPasswordModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidServiceUserPassword: true,
              isNull: true);

            var serviceUserPasswordGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              invalidServiceUserPassword: true,
              isInvalidCharacterNumber: true);

            #endregion

            Dictionary<string, AbacusSettingModel> invalidModels = new Dictionary<string, AbacusSettingModel>();
            invalidModels.Add(nameof(nullModel), nullModel);
            invalidModels.Add(nameof(nullNameModel), nullNameModel);
            invalidModels.Add(nameof(nameGreaterThen255CharactersModel), nameGreaterThen255CharactersModel);
            invalidModels.Add(nameof(nullServiceUrlModel), nullServiceUrlModel);
            invalidModels.Add(nameof(portSmallerThan1Model), portSmallerThan1Model);
            invalidModels.Add(nameof(portGreaterThan99999Model), portGreaterThan99999Model);
            invalidModels.Add(nameof(urlGreaterThen255CharactersModel), urlGreaterThen255CharactersModel);
            invalidModels.Add(nameof(nullServiceUserModel), nullServiceUserModel);
            invalidModels.Add(nameof(serviceUserGreaterThan255CharactersModel), serviceUserGreaterThan255CharactersModel);
            invalidModels.Add(nameof(nullServiceUserPasswordModel), nullServiceUserPasswordModel);
            invalidModels.Add(nameof(serviceUserPasswordGreaterThan255CharactersModel), serviceUserPasswordGreaterThan255CharactersModel);

            #endregion

            #region Arrange

            Dictionary<string, ResultModel<AbacusSettingModel>> actualModels = new Dictionary<string, ResultModel<AbacusSettingModel>>();

            foreach (var invalidModel in invalidModels)
            {
              var actualModel = _abacusSettingService.Create(invalidModel.Value);
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
                case LangKey.MSG_NAME_IS_REQUIRED when actualModel.Key == nameof(nullNameModel):
                  Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255 when actualModel.Key == nameof(nameGreaterThen255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_URL_IS_REQUIRED when actualModel.Key == nameof(nullServiceUrlModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_URL_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portSmallerThan1Model):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portGreaterThan99999Model):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255 when actualModel.Key == nameof(urlGreaterThen255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_USER_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255 when actualModel.Key == nameof(serviceUserGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserPasswordModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50 when actualModel.Key == nameof(serviceUserPasswordGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50, actualModel.Value.Error.Message);
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

          AbacusSettingModel nullModel = null;

          var nullNameModel = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidName: true,
            isNull: true);

          var nameGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidName: true,
            isInvalidCharacterNumber: true);

          var nullServiceUrlModel = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidServiceUrl: true,
            isNull: true);

          var portSmallerThan1Model = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidServicePort: true,
            isSmallerThan1: true);

          var portGreaterThan99999Model = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidServicePort: true,
            isSmallerThan1: false);

          var urlGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidServiceUrl: true,
            isInvalidCharacterNumber: true);

          var nullServiceUserModel = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidServiceUser: true,
            isNull: true);

          var serviceUserGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidServiceUser: true,
            isInvalidCharacterNumber: true);

          var nullServiceUserPasswordModel = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidServiceUserPassword: true,
            isNull: true);

          var serviceUserPasswordGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
            tenantId: _tenantModel.Id,
            invalidServiceUserPassword: true,
            isInvalidCharacterNumber: true);

          #endregion

          Dictionary<string, AbacusSettingModel> invalidModels = new Dictionary<string, AbacusSettingModel>();
          invalidModels.Add(nameof(nullModel), nullModel);
          invalidModels.Add(nameof(nullNameModel), nullNameModel);
          invalidModels.Add(nameof(nameGreaterThen255CharactersModel), nameGreaterThen255CharactersModel);
          invalidModels.Add(nameof(nullServiceUrlModel), nullServiceUrlModel);
          invalidModels.Add(nameof(portSmallerThan1Model), portSmallerThan1Model);
          invalidModels.Add(nameof(portGreaterThan99999Model), portGreaterThan99999Model);
          invalidModels.Add(nameof(urlGreaterThen255CharactersModel), urlGreaterThen255CharactersModel);
          invalidModels.Add(nameof(nullServiceUserModel), nullServiceUserModel);
          invalidModels.Add(nameof(serviceUserGreaterThan255CharactersModel), serviceUserGreaterThan255CharactersModel);
          invalidModels.Add(nameof(nullServiceUserPasswordModel), nullServiceUserPasswordModel);
          invalidModels.Add(nameof(serviceUserPasswordGreaterThan255CharactersModel), serviceUserPasswordGreaterThan255CharactersModel);

          #endregion

          #region Arrange

          Dictionary<string, ResultModel<AbacusSettingModel>> actualModels = new Dictionary<string, ResultModel<AbacusSettingModel>>();

          foreach (var invalidModel in invalidModels)
          {
            var actualModel = _abacusSettingService.Create(invalidModel.Value);
            actualModels.Add(invalidModel.Key, actualModel);
          }

          foreach (var actualModel in actualModels)
          {
            Assert.IsNull(actualModel.Value.Data);
            Assert.IsNotNull(actualModel.Value.Error);

            switch (actualModel.Value.Error.Message)
            {
              case LangKey.DATA_DOES_NOT_EXIST when actualModel.Key == nameof(nullModel):
                Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_NAME_IS_REQUIRED when actualModel.Key == nameof(nullNameModel):
                Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255 when actualModel.Key == nameof(nameGreaterThen255CharactersModel):
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_URL_IS_REQUIRED when actualModel.Key == nameof(nullServiceUrlModel):
                Assert.AreEqual(LangKey.MSG_SERVICE_URL_IS_REQUIRED, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portSmallerThan1Model):
                Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portGreaterThan99999Model):
                Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255 when actualModel.Key == nameof(urlGreaterThen255CharactersModel):
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_USER_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserModel):
                Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255 when actualModel.Key == nameof(serviceUserGreaterThan255CharactersModel):
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserPasswordModel):
                Assert.AreEqual(LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED, actualModel.Value.Error.Message);
                break;
              case LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50 when actualModel.Key == nameof(serviceUserPasswordGreaterThan255CharactersModel):
                Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50, actualModel.Value.Error.Message);
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

    #region Update(AbacusSettingModel model)

    [TestMethod()]
    public void SysAdminRoleUpdate_CorrectModel_Success()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          var expectedAbacusSetting = MakeAbacusSettingModel(createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualAbacusSetting = _abacusSettingService.Update(expectedAbacusSetting);

          Assert.AreEqual(expectedAbacusSetting.Id, actualAbacusSetting.Data.Id);
          Assert.AreEqual(expectedAbacusSetting.Name, actualAbacusSetting.Data.Name);
          Assert.AreEqual(expectedAbacusSetting.ServiceUrl, actualAbacusSetting.Data.ServiceUrl);
          Assert.AreEqual(expectedAbacusSetting.ServicePort, actualAbacusSetting.Data.ServicePort);
          Assert.AreEqual(expectedAbacusSetting.ServiceUseSsl, actualAbacusSetting.Data.ServiceUseSsl);
          Assert.AreEqual(expectedAbacusSetting.ServiceUser, actualAbacusSetting.Data.ServiceUser);
          Assert.AreEqual(expectedAbacusSetting.ServiceUserPassword, actualAbacusSetting.Data.ServiceUserPassword);
          Assert.AreEqual(expectedAbacusSetting.HealthStatus, actualAbacusSetting.Data.HealthStatus);
          Assert.AreEqual(expectedAbacusSetting.TenantId, actualAbacusSetting.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleUpdate_CorrectModel_Success()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          var expectedAbacusSetting = MakeAbacusSettingModel(createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualAbacusSetting = _abacusSettingService.Update(expectedAbacusSetting);

          Assert.AreEqual(expectedAbacusSetting.Id, actualAbacusSetting.Data.Id);
          Assert.AreEqual(expectedAbacusSetting.Name, actualAbacusSetting.Data.Name);
          Assert.AreEqual(expectedAbacusSetting.ServiceUrl, actualAbacusSetting.Data.ServiceUrl);
          Assert.AreEqual(expectedAbacusSetting.ServicePort, actualAbacusSetting.Data.ServicePort);
          Assert.AreEqual(expectedAbacusSetting.ServiceUseSsl, actualAbacusSetting.Data.ServiceUseSsl);
          Assert.AreEqual(expectedAbacusSetting.ServiceUser, actualAbacusSetting.Data.ServiceUser);
          Assert.AreEqual(expectedAbacusSetting.ServiceUserPassword, actualAbacusSetting.Data.ServiceUserPassword);
          Assert.AreEqual(expectedAbacusSetting.HealthStatus, actualAbacusSetting.Data.HealthStatus);
          Assert.AreEqual(expectedAbacusSetting.TenantId, actualAbacusSetting.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleUpdate_CorrectModel_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var abacusSettingModel = MakeAbacusSettingModel(_tenantModel.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          var expectedAbacusSetting = MakeAbacusSettingModel(createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var actualAbacusSetting = _abacusSettingService.Update(expectedAbacusSetting);

          Assert.AreEqual(expectedAbacusSetting.Id, actualAbacusSetting.Data.Id);
          Assert.AreEqual(expectedAbacusSetting.Name, actualAbacusSetting.Data.Name);
          Assert.AreEqual(expectedAbacusSetting.ServiceUrl, actualAbacusSetting.Data.ServiceUrl);
          Assert.AreEqual(expectedAbacusSetting.ServicePort, actualAbacusSetting.Data.ServicePort);
          Assert.AreEqual(expectedAbacusSetting.ServiceUseSsl, actualAbacusSetting.Data.ServiceUseSsl);
          Assert.AreEqual(expectedAbacusSetting.ServiceUser, actualAbacusSetting.Data.ServiceUser);
          Assert.AreEqual(expectedAbacusSetting.ServiceUserPassword, actualAbacusSetting.Data.ServiceUserPassword);
          Assert.AreEqual(expectedAbacusSetting.HealthStatus, actualAbacusSetting.Data.HealthStatus);
          Assert.AreEqual(expectedAbacusSetting.TenantId, actualAbacusSetting.Data.TenantId);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod]
    public void SysAdminRoleUpdate_InvalidModels_Fail()
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

            var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
            var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
            if (createdAbacusSettingModel.Error != null)
              Assert.Fail();

            param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

            #region Prepare invalid models

            var invalidIdModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: -1,
              isInvalidId: true);

            AbacusSettingModel nullModel = null;

            var nullNameModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidName: true,
              isNull: true);

            var nameGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidName: true,
              isInvalidCharacterNumber: true);

            var nullServiceUrlModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUrl: true,
              isNull: true);

            var portSmallerThan1Model = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServicePort: true,
              isSmallerThan1: true);

            var portGreaterThan99999Model = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServicePort: true,
              isSmallerThan1: false);

            var urlGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUrl: true,
              isInvalidCharacterNumber: true);

            var nullServiceUserModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUser: true,
              isNull: true);

            var serviceUserGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUser: true,
              isInvalidCharacterNumber: true);

            var nullServiceUserPasswordModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUserPassword: true,
              isNull: true);

            var serviceUserPasswordGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUserPassword: true,
              isInvalidCharacterNumber: true);

            #endregion

            Dictionary<string, AbacusSettingModel> invalidModels = new Dictionary<string, AbacusSettingModel>();
            invalidModels.Add(nameof(invalidIdModel), invalidIdModel);
            invalidModels.Add(nameof(nullModel), nullModel);
            invalidModels.Add(nameof(nullNameModel), nullNameModel);
            invalidModels.Add(nameof(nameGreaterThen255CharactersModel), nameGreaterThen255CharactersModel);
            invalidModels.Add(nameof(nullServiceUrlModel), nullServiceUrlModel);
            invalidModels.Add(nameof(portSmallerThan1Model), portSmallerThan1Model);
            invalidModels.Add(nameof(portGreaterThan99999Model), portGreaterThan99999Model);
            invalidModels.Add(nameof(urlGreaterThan255CharactersModel), urlGreaterThan255CharactersModel);
            invalidModels.Add(nameof(nullServiceUserModel), nullServiceUserModel);
            invalidModels.Add(nameof(serviceUserGreaterThan255CharactersModel), serviceUserGreaterThan255CharactersModel);
            invalidModels.Add(nameof(nullServiceUserPasswordModel), nullServiceUserPasswordModel);
            invalidModels.Add(nameof(serviceUserPasswordGreaterThan255CharactersModel), serviceUserPasswordGreaterThan255CharactersModel);

            #endregion

            #region Arrange

            Dictionary<string, ResultModel<AbacusSettingModel>> actualModels = new Dictionary<string, ResultModel<AbacusSettingModel>>();

            foreach (var invalidModel in invalidModels)
            {
              var actualModel = _abacusSettingService.Update(invalidModel.Value);
              actualModels.Add(invalidModel.Key, actualModel);
            }

            foreach (var actualModel in actualModels)
            {
              Assert.IsNull(actualModel.Value.Data);
              Assert.IsNotNull(actualModel.Value.Error);

              switch (actualModel.Value.Error.Message)
              {
                case LangKey.MSG_NO_RECORD_FOUND_WITH_ID when actualModel.Key == nameof(invalidIdModel):
                  Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullModel):
                  Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_NAME_IS_REQUIRED when actualModel.Key == nameof(nullNameModel):
                  Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255 when actualModel.Key == nameof(nameGreaterThen255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_URL_IS_REQUIRED when actualModel.Key == nameof(nullServiceUrlModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_URL_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portSmallerThan1Model):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portGreaterThan99999Model):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255 when actualModel.Key == nameof(urlGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_USER_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255 when actualModel.Key == nameof(serviceUserGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserPasswordModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50 when actualModel.Key == nameof(serviceUserPasswordGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50, actualModel.Value.Error.Message);
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
            var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

            if (createdTenantModel != null)
            {
              _tenantService.Delete(createdTenantModel.Id);
            }

            if (createdAbacusSettingModel != null)
            {
              _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
            }
          });
    }

    [TestMethod]
    public void AdminRoleUpdate_InvalidModels_Fail()
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

            var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
            var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
            if (createdAbacusSettingModel.Error != null)
              Assert.Fail();

            param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

            #region Prepare invalid models

            var invalidIdModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: -1,
              isInvalidId: true);

            AbacusSettingModel nullModel = null;

            var nullNameModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidName: true,
              isNull: true);

            var nameGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidName: true,
              isInvalidCharacterNumber: true);

            var nullServiceUrlModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUrl: true,
              isNull: true);

            var portSmallerThan1Model = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServicePort: true,
              isSmallerThan1: true);

            var portGreaterThan99999Model = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServicePort: true,
              isSmallerThan1: false);

            var urlGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUrl: true,
              isInvalidCharacterNumber: true);

            var nullServiceUserModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUser: true,
              isNull: true);

            var serviceUserGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUser: true,
              isInvalidCharacterNumber: true);

            var nullServiceUserPasswordModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUserPassword: true,
              isNull: true);

            var serviceUserPasswordGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: createdTenantModel.Data.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUserPassword: true,
              isInvalidCharacterNumber: true);

            #endregion

            Dictionary<string, AbacusSettingModel> invalidModels = new Dictionary<string, AbacusSettingModel>();
            invalidModels.Add(nameof(invalidIdModel), invalidIdModel);
            invalidModels.Add(nameof(nullModel), nullModel);
            invalidModels.Add(nameof(nullNameModel), nullNameModel);
            invalidModels.Add(nameof(nameGreaterThen255CharactersModel), nameGreaterThen255CharactersModel);
            invalidModels.Add(nameof(nullServiceUrlModel), nullServiceUrlModel);
            invalidModels.Add(nameof(portSmallerThan1Model), portSmallerThan1Model);
            invalidModels.Add(nameof(portGreaterThan99999Model), portGreaterThan99999Model);
            invalidModels.Add(nameof(urlGreaterThan255CharactersModel), urlGreaterThan255CharactersModel);
            invalidModels.Add(nameof(nullServiceUserModel), nullServiceUserModel);
            invalidModels.Add(nameof(serviceUserGreaterThan255CharactersModel), serviceUserGreaterThan255CharactersModel);
            invalidModels.Add(nameof(nullServiceUserPasswordModel), nullServiceUserPasswordModel);
            invalidModels.Add(nameof(serviceUserPasswordGreaterThan255CharactersModel), serviceUserPasswordGreaterThan255CharactersModel);

            #endregion

            #region Arrange

            Dictionary<string, ResultModel<AbacusSettingModel>> actualModels = new Dictionary<string, ResultModel<AbacusSettingModel>>();

            foreach (var invalidModel in invalidModels)
            {
              var actualModel = _abacusSettingService.Update(invalidModel.Value);
              actualModels.Add(invalidModel.Key, actualModel);
            }

            foreach (var actualModel in actualModels)
            {
              Assert.IsNull(actualModel.Value.Data);
              Assert.IsNotNull(actualModel.Value.Error);

              switch (actualModel.Value.Error.Message)
              {
                case LangKey.MSG_NO_RECORD_FOUND_WITH_ID when actualModel.Key == nameof(invalidIdModel):
                  Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS when actualModel.Key == nameof(nullModel):
                  Assert.AreEqual(LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_NAME_IS_REQUIRED when actualModel.Key == nameof(nullNameModel):
                  Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255 when actualModel.Key == nameof(nameGreaterThen255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_URL_IS_REQUIRED when actualModel.Key == nameof(nullServiceUrlModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_URL_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portSmallerThan1Model):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portGreaterThan99999Model):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255 when actualModel.Key == nameof(urlGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_USER_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255 when actualModel.Key == nameof(serviceUserGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserPasswordModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50 when actualModel.Key == nameof(serviceUserPasswordGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50, actualModel.Value.Error.Message);
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
            var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

            if (createdTenantModel != null)
            {
              _tenantService.Delete(createdTenantModel.Id);
            }

            if (createdAbacusSettingModel != null)
            {
              _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
            }
          });
    }

    [TestMethod]
    public void UserRoleUpdate_InvalidModels_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
          .ThenImplementTest(param =>
          {
            #region Init data

            var abacusSettingModel = MakeAbacusSettingModel(_tenantModel.Id);
            var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
            if (createdAbacusSettingModel.Error != null)
              Assert.Fail();

            param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

            #region Prepare invalid models

            var invalidIdModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: -1,
              isInvalidId: true);

            AbacusSettingModel nullModel = null;

            var nullNameModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidName: true,
              isNull: true);

            var nameGreaterThen255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidName: true,
              isInvalidCharacterNumber: true);

            var nullServiceUrlModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUrl: true,
              isNull: true);

            var portSmallerThan1Model = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServicePort: true,
              isSmallerThan1: true);

            var portGreaterThan99999Model = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServicePort: true,
              isSmallerThan1: false);

            var urlGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUrl: true,
              isInvalidCharacterNumber: true);

            var nullServiceUserModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUser: true,
              isNull: true);

            var serviceUserGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUser: true,
              isInvalidCharacterNumber: true);

            var nullServiceUserPasswordModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUserPassword: true,
              isNull: true);

            var serviceUserPasswordGreaterThan255CharactersModel = MakeInvalidAbacusSettingModel(
              tenantId: _tenantModel.Id,
              abacusSettingId: createdAbacusSettingModel.Data.Id,
              invalidServiceUserPassword: true,
              isInvalidCharacterNumber: true);

            #endregion

            Dictionary<string, AbacusSettingModel> invalidModels = new Dictionary<string, AbacusSettingModel>();
            invalidModels.Add(nameof(invalidIdModel), invalidIdModel);
            invalidModels.Add(nameof(nullModel), nullModel);
            invalidModels.Add(nameof(nullNameModel), nullNameModel);
            invalidModels.Add(nameof(nameGreaterThen255CharactersModel), nameGreaterThen255CharactersModel);
            invalidModels.Add(nameof(nullServiceUrlModel), nullServiceUrlModel);
            invalidModels.Add(nameof(portSmallerThan1Model), portSmallerThan1Model);
            invalidModels.Add(nameof(portGreaterThan99999Model), portGreaterThan99999Model);
            invalidModels.Add(nameof(urlGreaterThan255CharactersModel), urlGreaterThan255CharactersModel);
            invalidModels.Add(nameof(nullServiceUserModel), nullServiceUserModel);
            invalidModels.Add(nameof(serviceUserGreaterThan255CharactersModel), serviceUserGreaterThan255CharactersModel);
            invalidModels.Add(nameof(nullServiceUserPasswordModel), nullServiceUserPasswordModel);
            invalidModels.Add(nameof(serviceUserPasswordGreaterThan255CharactersModel), serviceUserPasswordGreaterThan255CharactersModel);

            #endregion

            #region Arrange

            Dictionary<string, ResultModel<AbacusSettingModel>> actualModels = new Dictionary<string, ResultModel<AbacusSettingModel>>();

            foreach (var invalidModel in invalidModels)
            {
              var actualModel = _abacusSettingService.Update(invalidModel.Value);
              actualModels.Add(invalidModel.Key, actualModel);
            }

            foreach (var actualModel in actualModels)
            {
              Assert.IsNull(actualModel.Value.Data);
              Assert.IsNotNull(actualModel.Value.Error);

              switch (actualModel.Value.Error.Message)
              {
                case LangKey.MSG_NO_RECORD_FOUND_WITH_ID when actualModel.Key == nameof(invalidIdModel):
                  Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, actualModel.Value.Error.Message);
                  break;
                case LangKey.DATA_DOES_NOT_EXIST when actualModel.Key == nameof(nullModel):
                  Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_NAME_IS_REQUIRED when actualModel.Key == nameof(nullNameModel):
                  Assert.AreEqual(LangKey.MSG_NAME_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255 when actualModel.Key == nameof(nameGreaterThen255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_URL_IS_REQUIRED when actualModel.Key == nameof(nullServiceUrlModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_URL_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portSmallerThan1Model):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999 when actualModel.Key == nameof(portGreaterThan99999Model):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255 when actualModel.Key == nameof(urlGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_USER_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_USER_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255 when actualModel.Key == nameof(serviceUserGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED when actualModel.Key == nameof(nullServiceUserPasswordModel):
                  Assert.AreEqual(LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED, actualModel.Value.Error.Message);
                  break;
                case LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50 when actualModel.Key == nameof(serviceUserPasswordGreaterThan255CharactersModel):
                  Assert.AreEqual(LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50, actualModel.Value.Error.Message);
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
            var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

            if (createdTenantModel != null)
            {
              _tenantService.Delete(createdTenantModel.Id);
            }

            if (createdAbacusSettingModel != null)
            {
              _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
            }
          });
    }


    #endregion

    #region Delete(int id);

    [TestMethod()]
    public void SysAdminRoleDelete_WithExistingAbacusSettingId_Success()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var beforDeletedModel = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var isDeleted = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var afterDeletedModel = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNotNull(beforDeletedModel.Data);
          Assert.IsTrue(isDeleted.Data);
          Assert.IsNull(afterDeletedModel.Data);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleDelete_WithExistingAbacusSettingId_Success()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var beforDeletedModel = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var isDeleted = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var afterDeletedModel = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNotNull(beforDeletedModel.Data);
          Assert.IsTrue(isDeleted.Data);
          Assert.IsNull(afterDeletedModel.Data);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleDelete_WithExistingAbacusSettingId_Success()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var abacusSettingModel = MakeAbacusSettingModel(_tenantModel.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var beforDeletedModel = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var isDeleted = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var afterDeletedModel = _abacusSettingService.GetById(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsNotNull(beforDeletedModel.Data);
          Assert.IsTrue(isDeleted.Data);
          Assert.IsNull(afterDeletedModel.Data);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleDelete_WithNonExistingAbacusSetting_Fail()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var stDelete = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var ndDelete = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsFalse(ndDelete.Data);
          Assert.IsNotNull(ndDelete.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, ndDelete.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void AdminRoleDelete_WithNonExistingAbacusSetting_Fail()
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

          var abacusSettingModel = MakeAbacusSettingModel(createdTenantModel.Data.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var stDelete = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var ndDelete = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsFalse(ndDelete.Data);
          Assert.IsNotNull(ndDelete.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, ndDelete.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdTenantModel = param.CleanData.Get<TenantModel>("createdTenantModel");
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdTenantModel != null)
          {
            _tenantService.Delete(createdTenantModel.Id);
          }

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void UserRoleDelete_WithNonExistingAbacusSetting_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          var abacusSettingModel = MakeAbacusSettingModel(_tenantModel.Id);
          var createdAbacusSettingModel = _abacusSettingService.Create(abacusSettingModel);
          if (createdAbacusSettingModel.Error != null)
            Assert.Fail();

          param.CleanData.Add("createdAbacusSettingModel", createdAbacusSettingModel.Data);

          #endregion

          #region Arrange

          var stDelete = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);
          var ndDelete = _abacusSettingService.Delete(createdAbacusSettingModel.Data.TenantId, createdAbacusSettingModel.Data.Id);

          Assert.IsFalse(ndDelete.Data);
          Assert.IsNotNull(ndDelete.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, ndDelete.Error.Message);

          #endregion
        })
        .ThenCleanDataTest(param =>
        {
          var createdAbacusSettingModel = param.CleanData.Get<AbacusSettingModel>("createdAbacusSettingModel");

          if (createdAbacusSettingModel != null)
          {
            _abacusSettingService.Delete(createdAbacusSettingModel.TenantId, createdAbacusSettingModel.Id);
          }
        });
    }

    [TestMethod()]
    public void SysAdminRoleDelete_WithInvalidAbacusSettingId_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(currentUser =>
        {
          var isDeleted = _abacusSettingService.Delete(_tenantModel.Id, 0);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, isDeleted.Error.Message);
        });
    }

    [TestMethod()]
    public void AdminRoleDelete_WithInvalidAbacusSettingId_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(currentUser =>
        {
          var isDeleted = _abacusSettingService.Delete(_tenantModel.Id, 0);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.MSG_NO_RECORD_FOUND_WITH_ID, isDeleted.Error.Message);
        });
    }

    [TestMethod()]
    public void UserRoleDelete_WithInvalidAbacusSettingId_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(currentUser =>
        {
          var isDeleted = _abacusSettingService.Delete(_tenantModel.Id, 0);

          Assert.IsFalse(isDeleted.Data);
          Assert.IsNotNull(isDeleted.Error);
          Assert.AreEqual(LangKey.DATA_DOES_NOT_EXIST, isDeleted.Error.Message);
        });
    }

    #endregion

    #region CheckConnection(TenantModel tenantModel, AbacusSettingModel abacusSettingModel)

    #region Success

    #endregion

    #region Fail

    #region Incorrect Tenant


    #endregion

    #region Incorrect Abacus setting

    [TestMethod]
    public void SysAdminRoleTestConnection_InCorrectAbacusSettingModels_Fail()
    {
      _testService.StartLoginWithSysAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          #region prepare tenants

          var correctTenantModel = MakeTenantModelForTestConnection(_tenantModel.Id);

          #endregion

          #region prepare incorrect abacus settings

          #region Name

          var nullNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectName: true,
            isNull: true);

          var emptyNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectName: true,
            isNull: false);

          #endregion

          #region Description

          var nullDescriptionAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectDescription: true,
            isNull: true);

          var emptyDescriptionAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectDescription: true,
            isNull: false);

          #endregion

          #region ServiceServerName

          var nullServerNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerUser: true,
            isNull: true);

          var emptyServerNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerUser: true,
            isNull: false);

          #endregion

          #region ServicePort

          var incorrectServicePortAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServicePort: true);

          #endregion

          #region ServiceSecurity

          var incorrectServiceSecurityAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceSecurity: true);

          #endregion

          #region ServiceServerPassword

          var nullServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNull: true);

          var emptyServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNull: false);

          var notExistServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNotExist: true);

          #endregion

          #endregion

          #endregion

          #region Arrange

          var actualNullNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullNameAbacusSettingModel);

          var actualEmptyNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyNameAbacusSettingModel);

          var actualNullDescriptionAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullDescriptionAbacusSettingModel);

          var actualEmptyDescriptionAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyDescriptionAbacusSettingModel);

          var actualNullServerNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullServerNameAbacusSettingModel);

          var actualEmptyServerNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyServerNameAbacusSettingModel);

          var actualIncorrectServicePortAbacusSettingAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            incorrectServicePortAbacusSettingModel);

          var actualIncorrectServiceSecurityAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            incorrectServiceSecurityAbacusSettingModel);

          var actualNullServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullServiceServerPasswordAbacusSettingModel);

          var actualEmptyServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyServiceServerPasswordAbacusSettingModel);

          var actualNotExistServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            notExistServiceServerPasswordAbacusSettingModel);

          Assert.IsFalse(actualNullNameAbacusSetting.Data);
          Assert.IsFalse(actualEmptyNameAbacusSetting.Data);
          Assert.IsFalse(actualNullServerNameAbacusSetting.Data);
          Assert.IsFalse(actualEmptyServerNameAbacusSetting.Data);
          Assert.IsFalse(actualIncorrectServiceSecurityAbacusSettingModel.Data);
          Assert.IsFalse(actualNullServiceServerPasswordAbacusSettingModel.Data);
          Assert.IsFalse(actualEmptyServiceServerPasswordAbacusSettingModel.Data);
          Assert.IsFalse(actualNotExistServiceServerPasswordAbacusSettingModel.Data);
          Assert.IsFalse(actualIncorrectServicePortAbacusSettingAbacusSetting.Data);

          Assert.IsTrue(actualNullDescriptionAbacusSetting.Data);
          Assert.IsTrue(actualEmptyDescriptionAbacusSetting.Data);

          #endregion
        });
    }

    [TestMethod]
    public void AdminRoleTestConnection_InCorrectAbacusSettingModels_Fail()
    {
      _testService.StartLoginWithAdmin()
        .ThenImplementTest(param =>
        {
          #region Init data

          #region prepare tenants

          var correctTenantModel = MakeTenantModelForTestConnection(_tenantModel.Id);

          #endregion

          #region prepare incorrect abacus settings

          #region Name

          var nullNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectName: true,
            isNull: true);

          var emptyNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectName: true,
            isNull: false);

          #endregion

          #region Description

          var nullDescriptionAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectDescription: true,
            isNull: true);

          var emptyDescriptionAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectDescription: true,
            isNull: false);

          #endregion

          #region ServiceServerName

          var nullServerNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerUser: true,
            isNull: true);

          var emptyServerNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerUser: true,
            isNull: false);

          #endregion

          #region ServicePort

          var incorrectServicePortAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServicePort: true);

          #endregion

          #region ServiceSecurity

          var incorrectServiceSecurityAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceSecurity: true);

          #endregion

          #region ServiceServerPassword

          var nullServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNull: true);

          var emptyServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNull: false);

          var notExistServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNotExist: true);

          #endregion

          #endregion

          #endregion

          #region Arrange

          var actualNullNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullNameAbacusSettingModel);

          var actualEmptyNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyNameAbacusSettingModel);

          var actualNullDescriptionAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullDescriptionAbacusSettingModel);

          var actualEmptyDescriptionAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyDescriptionAbacusSettingModel);

          var actualNullServerNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullServerNameAbacusSettingModel);

          var actualEmptyServerNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyServerNameAbacusSettingModel);

          var actualIncorrectServicePortAbacusSettingAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            incorrectServicePortAbacusSettingModel);

          var actualIncorrectServiceSecurityAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            incorrectServiceSecurityAbacusSettingModel);

          var actualNullServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullServiceServerPasswordAbacusSettingModel);

          var actualEmptyServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyServiceServerPasswordAbacusSettingModel);

          var actualNotExistServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            notExistServiceServerPasswordAbacusSettingModel);

          Assert.IsFalse(actualNullNameAbacusSetting.Data);
          Assert.IsFalse(actualEmptyNameAbacusSetting.Data);
          Assert.IsFalse(actualNullServerNameAbacusSetting.Data);
          Assert.IsFalse(actualEmptyServerNameAbacusSetting.Data);
          Assert.IsFalse(actualIncorrectServiceSecurityAbacusSettingModel.Data);
          Assert.IsFalse(actualNullServiceServerPasswordAbacusSettingModel.Data);
          Assert.IsFalse(actualEmptyServiceServerPasswordAbacusSettingModel.Data);
          Assert.IsFalse(actualNotExistServiceServerPasswordAbacusSettingModel.Data);
          Assert.IsFalse(actualIncorrectServicePortAbacusSettingAbacusSetting.Data);

          Assert.IsTrue(actualNullDescriptionAbacusSetting.Data);
          Assert.IsTrue(actualEmptyDescriptionAbacusSetting.Data);
          
          #endregion
        });
    }

    [TestMethod]
    public void UserRoleTestConnection_InCorrectAbacusSettingModels_Fail()
    {
      _testService.StartLoginWithUser(_userModel.Email, "123456")
        .ThenImplementTest(param =>
        {
          #region Init data

          #region prepare tenants

          var correctTenantModel = MakeTenantModelForTestConnection(_tenantModel.Id);

          #endregion

          #region prepare incorrect abacus settings

          #region Name

          var nullNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectName: true,
            isNull: true);

          var emptyNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectName: true,
            isNull: false);

          #endregion

          #region Description

          var nullDescriptionAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectDescription: true,
            isNull: true);

          var emptyDescriptionAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectDescription: true,
            isNull: false);

          #endregion

          #region ServiceServerName

          var nullServerNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerUser: true,
            isNull: true);

          var emptyServerNameAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerUser: true,
            isNull: false);

          #endregion

          #region ServicePort

          var incorrectServicePortAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServicePort: true);

          #endregion

          #region ServiceSecurity

          var incorrectServiceSecurityAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceSecurity: true);

          #endregion

          #region ServiceServerPassword

          var nullServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNull: true);

          var emptyServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNull: false);

          var notExistServiceServerPasswordAbacusSettingModel = MakeAbacusSettingModelForTestConnection(
            tenantId: correctTenantModel.Id,
            isIncorrectServiceServerPassword: true,
            isNotExist: true);

          #endregion

          #endregion

          #endregion

          #region Arrange

          var actualNullNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullNameAbacusSettingModel);

          var actualEmptyNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyNameAbacusSettingModel);

          var actualNullDescriptionAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullDescriptionAbacusSettingModel);

          var actualEmptyDescriptionAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyDescriptionAbacusSettingModel);

          var actualNullServerNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullServerNameAbacusSettingModel);

          var actualEmptyServerNameAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyServerNameAbacusSettingModel);

          var actualIncorrectServicePortAbacusSettingAbacusSetting = _abacusSettingService.CheckConnection(
            correctTenantModel,
            incorrectServicePortAbacusSettingModel);

          var actualIncorrectServiceSecurityAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            incorrectServiceSecurityAbacusSettingModel);

          var actualNullServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            nullServiceServerPasswordAbacusSettingModel);

          var actualEmptyServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            emptyServiceServerPasswordAbacusSettingModel);

          var actualNotExistServiceServerPasswordAbacusSettingModel = _abacusSettingService.CheckConnection(
            correctTenantModel,
            notExistServiceServerPasswordAbacusSettingModel);

          Assert.IsTrue(actualNullDescriptionAbacusSetting.Data);
          Assert.IsTrue(actualEmptyDescriptionAbacusSetting.Data);

          Assert.IsFalse(actualNullNameAbacusSetting.Data);
          Assert.IsFalse(actualEmptyNameAbacusSetting.Data);
          Assert.IsFalse(actualNullServerNameAbacusSetting.Data);
          Assert.IsFalse(actualEmptyServerNameAbacusSetting.Data);
          Assert.IsFalse(actualIncorrectServicePortAbacusSettingAbacusSetting.Data);
          Assert.IsFalse(actualIncorrectServiceSecurityAbacusSettingModel.Data);
          Assert.IsFalse(actualNullServiceServerPasswordAbacusSettingModel.Data);
          Assert.IsFalse(actualEmptyServiceServerPasswordAbacusSettingModel.Data);
          Assert.IsFalse(actualNotExistServiceServerPasswordAbacusSettingModel.Data);

          #endregion
        });
    }

    #endregion

    #endregion

    #endregion

    #region Private methods

    private TenantModel MakeTenantModelForTestConnection(
      int? tenantId = 0,
      bool isInvalidNumber = false,
      bool isIncorrectName = false,
      bool isIncorrectDescription = false,
      bool isNull = false)
    {
      return new TenantModel()
      {
        Id = tenantId.Value,
        Name = isIncorrectName ? isNull ? null : String.Empty : "Tenant Unit Test",
        Description = isIncorrectDescription ? isNull ? null : String.Empty : "Tenant Unit Test Description",
        Number = isInvalidNumber ? 69 : 200,
        ScheduleTimer = new Random().Next(1, 180)
      };
    }

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

    private AbacusSettingModel MakeAbacusSettingModelForTestConnection(
      int tenantId,
      bool isIncorrectName = false,
      bool isIncorrectDescription = false,
      bool isIncorrectServiceServerName = false,
      bool isIncorrectServicePort = false,
      bool isIncorrectServiceSecurity = false,
      bool isIncorrectServiceServerUser = false,
      bool isIncorrectServiceServerPassword = false,
      bool isNull = false,
      bool isNotExist = false)
    {
        return new AbacusSettingModel()
        {
          Name = isIncorrectName ? isNull ? null : String.Empty : "AbacusSetting Unit Test",
          Description = isIncorrectDescription ? isNull ? null : String.Empty : "test",
          ServiceUrl = isIncorrectServiceServerName ? isNull ? null : String.Empty : "allcus.all-consulting.ch",
          ServicePort = isIncorrectServicePort ? 69 : 443,
          ServiceUseSsl = isIncorrectServiceSecurity ? false : true,
          ServiceUser = isIncorrectServiceServerUser ? isNull ? null : String.Empty : "acag",
          ServiceUserPassword = isIncorrectServiceServerPassword ? isNull ? null : isNotExist ? "no exist password" : String.Empty : "RFY2TYecnOiRQX4F52n30g==",
          TenantId = tenantId
        };
    }

    private AbacusSettingModel MakeAbacusSettingModel(int tenantId)
    {
      return new AbacusSettingModel()
      {
        Name = "NameTest_" + Guid.NewGuid().ToString(),
        ServiceUrl = "ServiceUrlTest_" + Guid.NewGuid().ToString(),
        ServicePort = new Random().Next(1, 99999),
        ServiceUseSsl = true,
        ServiceUser = "ServiceUserTest_" + Guid.NewGuid().ToString(),
        ServiceUserPassword = "ServiceUserPasswordTest",
        HealthStatus = true,
        TenantId = tenantId
      };
    }

    private AbacusSettingModel MakeAbacusSettingModel(AbacusSettingModel abacusSettingModel)
    {
      return new AbacusSettingModel()
      {
        Id = abacusSettingModel.Id,
        Name = "NameTest_" + Guid.NewGuid().ToString(),
        ServiceUrl = "ServiceUrlTest_" + Guid.NewGuid().ToString(),
        ServicePort = new Random().Next(1, 99999),
        ServiceUseSsl = abacusSettingModel.ServiceUseSsl,
        ServiceUser = "ServiceUserTest_" + Guid.NewGuid().ToString(),
        ServiceUserPassword = "ServiceUserPasswordTest",
        HealthStatus = abacusSettingModel.HealthStatus,
        TenantId = abacusSettingModel.TenantId
      };
    }

    private AbacusSettingModel MakeInvalidAbacusSettingModel(
      int tenantId,
      int abacusSettingId = 1,
      bool invalidName = false,
      bool invalidServiceUrl = false,
      bool invalidServicePort = false,
      bool invalidServiceUser = false,
      bool invalidServiceUserPassword = false,
      bool isNull = false,
      bool isInvalidId = false,
      bool isInvalidCharacterNumber = false,
      bool isSmallerThan1 = false)
    {
      AbacusSettingModel result = null;

      if (isNull)
      {
        result = new AbacusSettingModel()
        {
          Id = abacusSettingId,
          Name = invalidName ? null : "ValidName_" + Guid.NewGuid().ToString(),
          ServiceUrl = invalidServiceUrl ? null : "ValidUrl_" + Guid.NewGuid().ToString(),
          ServicePort = new Random().Next(1, 99999),
          ServiceUseSsl = true,
          ServiceUser = invalidServiceUser ? null : "ValidUser_" + Guid.NewGuid().ToString(),
          ServiceUserPassword = invalidServiceUserPassword ? null : "ValidPassword",
          HealthStatus = true,
          TenantId = tenantId
        };
      }
      else if (isInvalidCharacterNumber)
      {
        result = new AbacusSettingModel()
        {
          Id = abacusSettingId,
          Name = invalidName ? Get300Characters() : "ValidName_" + Guid.NewGuid().ToString(),
          ServiceUrl = invalidServiceUrl ? Get300Characters() : "ValidUrl_" + Guid.NewGuid().ToString(),
          ServicePort = new Random().Next(1, 99999),
          ServiceUseSsl = true,
          ServiceUser = invalidServiceUser ? Get300Characters() : "ValidUser_" + Guid.NewGuid().ToString(),
          ServiceUserPassword = invalidServiceUserPassword ? Get300Characters() : "ValidPassword",
          HealthStatus = true,
          TenantId = tenantId
        };
      }
      else if (isInvalidId)
      {
        result = new AbacusSettingModel()
        {
          Id = abacusSettingId,
          Name = "ValidName_" + Guid.NewGuid().ToString(),
          ServiceUrl = "ValidUrl_" + Guid.NewGuid().ToString(),
          ServicePort = new Random().Next(1, 99999),
          ServiceUseSsl = true,
          ServiceUser = "ValidUser_" + Guid.NewGuid().ToString(),
          ServiceUserPassword = "ValidPassword",
          HealthStatus = true,
          TenantId = tenantId
        };
      }
      else if (invalidServicePort)
      {
        result = new AbacusSettingModel()
        {
          Id = abacusSettingId,
          Name = "ValidName_" + Guid.NewGuid().ToString(),
          ServiceUrl = "ValidUrl_" + Guid.NewGuid().ToString(),
          ServicePort = isSmallerThan1 ? 0 : 999999999,
          ServiceUseSsl = true,
          ServiceUser = "ValidUser_" + Guid.NewGuid().ToString(),
          ServiceUserPassword = "ValidPassword",
          HealthStatus = true,
          TenantId = tenantId
        };
      }

      return result;
    }

    private string Get300Characters()
    {
      return "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante. Etiam sit amet orci eget eros faucibus tincidunt. Duis leo. Sed fringilla mauris sit amet nibh. Donec sodales sagittis magna. Sed consequat, leo eget bibendum sodales, augue velit cursus nunc, quis gravida magna mi a libero. Fusce vulputate eleifend sapien. Vestibulum purus quam, scelerisque ut, mollis sed, nonummy id, metus. Nullam accumsan lorem in dui. Cras ultricies mi eu turpis hendrerit fringilla. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; In ac dui quis mi consectetuer lacinia. Nam pretium turpis et arcu. Duis arcu tortor, suscipit eget, imperdiet nec, imperdiet iaculis, ipsum. Sed aliquam ultrices mauris. Integer ante arcu, accumsan a, consectetuer eget, posuere ut, mauris. Praesent adipiscing. Phasellus ullamcorper ipsum rutrum nunc. Nunc nonummy metus. Vestibulum volutpat pretium libero. Cras id dui. Aenean ut";
    }

    #endregion
  }
}