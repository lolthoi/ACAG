using System;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;
using Microsoft.Extensions.DependencyInjection;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface IAbacusSettingService
  {
    ResultModel<AbacusSettingModel> GetByTenant(int tenantId);
    ResultModel<AbacusSettingModel> GetById(int tenantId, int id);
    ResultModel<AbacusSettingModel> Create(AbacusSettingModel model);
    ResultModel<AbacusSettingModel> Update(AbacusSettingModel model);
    ResultModel<bool> Delete(int tenantId, int id);
    ResultModel<bool> CheckConnection(TenantModel tenantModel, AbacusSettingModel abacusSettingModel);
  }

  public class AbacusSettingService : IAbacusSettingService
  {
    private readonly IAuthenLogicService<AbacusSettingService> _logicService;
    private readonly IUnitOfWork<CalendarConnectorContext> _dbContext;
    private readonly ILogDiaryService _logDiaryService;
    private readonly WrapperAPI _wrapperAPI;

    private readonly IEntityRepository<User> _users;
    private readonly IEntityRepository<TenantUserRel> _tenantUserRel;
    private readonly IEntityRepository<Tenant> _tenants;
    private readonly IEntityRepository<AppRoleRel> _appRoleRels;
    private readonly IEntityRepository<AppRole> _appRoles;
    private readonly IEntityRepository<AbacusSetting> _abacusSetting;

    private const string TEST_SUCCESS = "Connection successful!";

    public AbacusSettingService(IAuthenLogicService<AbacusSettingService> logicService,
      ILogDiaryService logDiaryService,
      IServiceProvider provider)
    {
      _logicService = logicService;
      _dbContext = logicService.DbContext;
      _wrapperAPI = provider.GetRequiredService<WrapperAPI>();
      _logDiaryService = logDiaryService;

      _users = _dbContext.GetRepository<User>();
      _tenantUserRel = _dbContext.GetRepository<TenantUserRel>();
      _tenants = _dbContext.GetRepository<Tenant>();
      _appRoleRels = _dbContext.GetRepository<AppRoleRel>();
      _appRoles = _dbContext.GetRepository<AppRole>();
      _abacusSetting = _dbContext.GetRepository<AbacusSetting>();
    }

    public ResultModel<bool> CheckConnection(TenantModel tenantModel, AbacusSettingModel abacusSettingModel)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenAuthorizeTenant(tenantModel.Id)
        .ThenValidate(() =>
        {
          return Validate(abacusSettingModel);
        })
        .ThenImplement(currentUser =>
        {
          var abacusSettingApi = new Models.Abacus.V1_0.AbacusSettingModel(
            abacusSettingModel.Name,
            abacusSettingModel.Description = "default",
            abacusSettingModel.ServiceUrl,
            abacusSettingModel.ServicePort,
            abacusSettingModel.ServiceUseSsl,
            abacusSettingModel.ServiceUser,
            abacusSettingModel.ServiceUserPassword);

          var tenantSetting = new Models.Abacus.V1_0.TenantModel(
            tenantModel.Name,
            tenantModel.Description,
            tenantModel.Number,
            abacusSettingApi);

          System.Net.Http.HttpResponseMessage result;

          try
          {
             result = _wrapperAPI.GetRequest("/api/testconnection", tenantSetting);
          }
          catch (Exception ex)
          {
            CreateLogDiary(tenantModel.Id, "AbacusSetting Test Connection", ex.Message);
            return false;
          }

          var data = result.ToContentString();

          if (result.IsSuccessStatusCode 
            && data != null 
            && data.Contains(TEST_SUCCESS))
          {
            return true;
          }

          return false;
        });

      return result;
    }

    public ResultModel<AbacusSettingModel> Create(AbacusSettingModel abacusSettingModel)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenAuthorizeTenant(abacusSettingModel?.TenantId)
        .ThenValidate(() =>
        {
          return Validate(abacusSettingModel);
        })
        .ThenImplement(currentUser =>
        {
          var abacusSetting = new AbacusSetting()
          {
            Name = abacusSettingModel.Name,
            ServiceUrl = abacusSettingModel.ServiceUrl,
            ServicePort = abacusSettingModel.ServicePort,
            ServiceUseSSL = abacusSettingModel.ServiceUseSsl,
            ServiceUser = abacusSettingModel.ServiceUser,
            ServiceUserPassword = abacusSettingModel.ServiceUserPassword,
            HealthStatus = abacusSettingModel.HealthStatus,
            CreatedBy = currentUser.Id,
            CreatedDate = DateTime.UtcNow
          };

          _abacusSetting.Add(abacusSetting);

          var tenant = _tenants
                .Query(t => t.Id == abacusSettingModel.TenantId)
                .FirstOrDefault();

          if (tenant != null)
          {
            tenant.AbacusSetting = abacusSetting;
          }

          int result = _dbContext.Save();

          abacusSettingModel.Id = abacusSetting.Id;

          _logicService.Cache.Tenants.Clear();

          return abacusSettingModel;
        });

      return result;
    }

    public ResultModel<bool> Delete(int tenantId, int id)
    {
      AbacusSetting abacusSetting = null;

      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        //.ThenAuthorizeTenant(tenantId, TenantManager.ABACUS_SETTING, id)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(() =>
        {
          if (id <= 0)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          abacusSetting = _abacusSetting.Query(a => a.Id == id).FirstOrDefault();

          if (abacusSetting == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          var tenant = abacusSetting.Tenants.FirstOrDefault();
          if (tenant == null || tenant.Id != tenantId)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          return null;
        })
        .ThenImplement(() =>
        {
          _abacusSetting.Delete(abacusSetting);
          _dbContext.Save();
          _logicService.Cache.Tenants.Clear();

          return true;
        });

      return result;
    }

    public ResultModel<AbacusSettingModel> GetByTenant(int tenantId)
    {
      AbacusSettingMD abacusSettingMD = null;

      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(() =>
        {
          abacusSettingMD = _logicService.Cache.Tenants.GetAbacusSetting(tenantId);

          if (abacusSettingMD == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          return null;
        })
        .ThenImplement(() =>
        {
          var abacusSettingModel = new AbacusSettingModel()
          {
            Id = abacusSettingMD.Id,
            Name = abacusSettingMD.Name,
            ServiceUrl = abacusSettingMD.ServiceUrl,
            ServicePort = abacusSettingMD.ServicePort,
            ServiceUseSsl = abacusSettingMD.ServiceUseSSL,
            ServiceUser = abacusSettingMD.ServiceUser,
            ServiceUserPassword = abacusSettingMD.ServiceUserPassword,
            HealthStatus = abacusSettingMD.HealthStatus,
            TenantId = tenantId
          };

          return abacusSettingModel;
        });

      return result;
    }

    public ResultModel<AbacusSettingModel> GetById(int tenantId, int id)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenAuthorizeTenant(tenantId)
        //.ThenAuthorizeTenant(tenantId, TenantManager.ABACUS_SETTING, id)
        .ThenValidate(() =>
        {
          return null;
        })
        .ThenImplement(() =>
        {
          var abacusSettingModel = _abacusSetting.Query(ab => ab.Id == id)
          .Select(a => new AbacusSettingModel()
          {
            Id = a.Id,
            Name = a.Name,
            ServiceUrl = a.ServiceUrl,
            ServicePort = a.ServicePort,
            ServiceUseSsl = a.ServiceUseSSL,
            ServiceUser = a.ServiceUser,
            ServiceUserPassword = a.ServiceUserPassword,
            HealthStatus = a.HealthStatus,
          })
          .FirstOrDefault();

          if(abacusSettingModel != null)
          {
            var tenantModel = _tenants.Query(t => t.AbacusSettingId == abacusSettingModel.Id).FirstOrDefault();

            if (tenantModel != null)
              abacusSettingModel.TenantId = tenantModel.Id;
          }

          return abacusSettingModel;
        });

      return result;
    }

    public ResultModel<AbacusSettingModel> Update(AbacusSettingModel abacusSettingModel)
    {
      AbacusSetting abacusSetting = null;

      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenAuthorizeTenant(abacusSettingModel?.TenantId)
        .ThenValidate(() =>
        {
          if (abacusSettingModel == null)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
          }

          if (abacusSettingModel.Id <= 0)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          abacusSetting = _abacusSetting.Query(md => md.Id == abacusSettingModel.Id).FirstOrDefault();

          if (abacusSetting == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          return Validate(abacusSettingModel);
        })
        .ThenImplement(currentUser =>
        {
          abacusSetting.Name = abacusSettingModel.Name;
          abacusSetting.ServiceUrl = abacusSettingModel.ServiceUrl;
          abacusSetting.ServicePort = abacusSettingModel.ServicePort;
          abacusSetting.ServiceUseSSL = abacusSettingModel.ServiceUseSsl;
          abacusSetting.ServiceUser = abacusSettingModel.ServiceUser;
          abacusSetting.ServiceUserPassword = abacusSettingModel.ServiceUserPassword;
          abacusSetting.HealthStatus = abacusSettingModel.HealthStatus;
          abacusSetting.ModifiedBy = currentUser.Id;
          abacusSetting.ModifiedDate = DateTime.UtcNow;

          var tenant = _tenants
                .Query(t => t.Id == abacusSettingModel.TenantId)
                .FirstOrDefault();

          if (tenant != null)
          {
            tenant.AbacusSetting = abacusSetting;
          }

          int result = _dbContext.Save();

          abacusSettingModel.Id = abacusSetting.Id;

          _logicService.Cache.Tenants.Clear();

          return abacusSettingModel;
        });

      return result;
    }

    #region Private method

    private ErrorModel Validate(AbacusSettingModel model)
    {
      if (model == null)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
      }
      if (string.IsNullOrEmpty(model.Name))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NAME_IS_REQUIRED);
      }
      if (model.Name.Length > 255)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255);
      }
      if (string.IsNullOrEmpty(model.ServiceUrl))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_SERVICE_URL_IS_REQUIRED);
      }
      if (model.ServiceUrl.Length > 255)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255);
      }
      if(model.ServicePort < 1 || model.ServicePort > 99999)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999);
      }
      if (string.IsNullOrEmpty(model.ServiceUser))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_SERVICE_USER_IS_REQUIRED);
      }
      if (model.ServiceUser.Length > 255)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255);
      }
      if (string.IsNullOrEmpty(model.ServiceUserPassword))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED);
      }
      if (model.ServiceUserPassword.Length > 50)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50);
      }

      return null;
    }

    private void CreateLogDiary(int tenantId, string data, string error)
    {
      var logDiaryModel = new LogDiaryModel
      {
        DateTime = DateTime.UtcNow,
        Data = data,
        Error = error,
        TenantId = tenantId,
      };

      _logDiaryService.Create(logDiaryModel);
    }

    #endregion
  }
}