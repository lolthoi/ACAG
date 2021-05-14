using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface IExchangeSettingService
  {
    ResultModel<ExchangeSettingViewModel> GetAll(int tenantId, string searchText);
    ResultModel<ExchangeSettingModel> GetById(int tenantId, int id);
    ResultModel<ExchangeSettingModel> Create(ExchangeSettingModel model);
    ResultModel<ExchangeSettingModel> Update(ExchangeSettingModel model);
    ResultModel<bool> Delete(int tenantId, int id);
    ResultModel<string> CheckConnection(ExchangeSettingModel model);
    List<ExchangeVersionModel> GetAllExchangeVersions();
    List<ExchangeLoginTypeModel> GetExchangeLoginTypes();
  }
  public class ExchangeSettingService : IExchangeSettingService
  {
    private readonly IAuthenLogicService<ExchangeSettingService> _logicService;
    protected readonly IUnitOfWork<CalendarConnectorContext> _dbContext;

    private readonly IEntityRepository<ExchangeSetting> _exchangeSettings;

    public ExchangeSettingService(IAuthenLogicService<ExchangeSettingService> logicService)
    {
      _logicService = logicService;
      _dbContext = _logicService.DbContext;
      _exchangeSettings = _dbContext.GetRepository<ExchangeSetting>();
    }

    public List<ExchangeVersionModel> GetAllExchangeVersions()
    {
      var result = GlobalSetting.Exchange.Versions
        .OrderBy(t => t.Value)
        .Select(t => new ExchangeVersionModel()
        {
          Id = t.Key,
          Name = t.Value
        })
        .ToList();

      return result;
    }

    public List<ExchangeLoginTypeModel> GetExchangeLoginTypes()
    {
      return GlobalSetting.Exchange.LoginTypes
        .OrderBy(t => t.Value)
        .Select(e => new ExchangeLoginTypeModel()
        {
          Id = e.Key,
          Name = e.Value,
          Type = (ExchangeLoginEnumType) e.Key
        })
        .ToList();
    }

    public ResultModel<string> CheckConnection(ExchangeSettingModel model)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        .ThenAuthorizeTenant(model?.TenantId)
        .ThenValidate(currentUser => 
        {
          var error = Validation(model);
          if (error != null)
          {
            return error;
          }
          var tenant = _logicService.Cache.Tenants.Get(model.TenantId);
          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
          }
          return null;
        })
        .ThenImplement(currentUser =>
        {
          model.LoginType = model.ExchangeLoginTypeModel.Id;
          var ewsService = new ACAG.Abacus.CalendarConnector.Exchange.EWSService(model);
          var status = ewsService.ExchangeServerIsValidDetail();
          return status;
        });
      return result;
    }

    public ResultModel<ExchangeSettingModel> Create(ExchangeSettingModel model)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        .ThenAuthorizeTenant(model?.TenantId)
        .ThenValidate(currentUser =>
        {
          var error = Validation(model);
          if (error != null)
          {
            return error;
          }
          var tenant = _logicService.Cache.Tenants.Get(model.TenantId);
          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
          }
          var hasTenantSameName = _logicService.Cache
            .Tenants
            .GetExchangeSettings(model.TenantId)
            .Any(t => t.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase));

          if (hasTenantSameName)
          {
            return new ErrorModel(ErrorType.DUPLICATED, LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS);
          }
          return null;
        })
        .ThenImplement(currentUser =>
        {
          ExchangeSetting exchangeSetting = new ExchangeSetting()
          {
            Name = model.Name,
            AzureClientId = model.AzureClientId,
            AzureClientSecret = model.AzureClientSecret,
            AzureTenant = model.AzureTenant,
            CreatedBy = currentUser.Id,
            CreatedDate = DateTime.UtcNow,
            Description = model.Description,
            EmailAddress = string.IsNullOrEmpty(model.EmailAddress) ? null : model.EmailAddress.Trim().ToLower(),
            ExchangeUrl = model.ExchangeUrl,
            ExchangeVersion = model.ExchangeVersionModel.Name,
            HealthStatus = model.HealthStatus,
            IsEnabled = model.IsEnabled,
            LoginType = model.ExchangeLoginTypeModel.Id,
            ServiceUser = model.ServiceUser,
            ServiceUserPassword = model.ServiceUserPassword,
            TenantId = model.TenantId
          };
          if (exchangeSetting.IsEnabled)
          {
            var exchangeSettings = _exchangeSettings
              .Query(es => es.TenantId == exchangeSetting.TenantId)
              .ToList();
            if (exchangeSettings.Count() > 0)
            {
              foreach (var item in exchangeSettings)
              {
                item.IsEnabled = false;
              }
              _exchangeSettings.UpdateRange(exchangeSettings);
            }
          }
          _exchangeSettings.Add(exchangeSetting);
          _dbContext.Save();

          model.Id = exchangeSetting.Id;

          _logicService.Cache.Tenants.Clear();
          return model;
        });
      return result;
    }

    public ResultModel<bool> Delete(int tenantId, int id)
    {
      ExchangeSetting exchangeSetting = null;

      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        //.ThenAuthorizeTenant(tenantId, TenantManager.EXCHANGE_SETTING, id)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(currentUser =>
        {
          exchangeSetting = _exchangeSettings.Query(t => t.Id == id && t.TenantId == tenantId).FirstOrDefault();
          if (exchangeSetting == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          }
          return null;
        })
        .ThenImplement(currentUser =>
        {
          _exchangeSettings.Delete(exchangeSetting);

          int result = _dbContext.Save();
          _logicService.Cache.Tenants.Clear();

          return true;
        });

      return result;
    }

    public ResultModel<ExchangeSettingViewModel> GetAll(int tenantId, string searchText)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(currentUser =>
        {
          var tenant = _logicService.Cache
          .Tenants
          .GetValues()
          .Where(x => x.Id == tenantId)
          .FirstOrDefault();

          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
          }
          return null;
        })
        .ThenImplement(currentUser =>
        {
          ExchangeSettingViewModel exchangeSettingViewModel = new ExchangeSettingViewModel();
          var query = _logicService.Cache
            .Tenants
            .GetExchangeSettings(tenantId)
            .AsEnumerable();
          if (!string.IsNullOrEmpty(searchText))
          {
            query = query.Where(f => f.Name.ToLower().Contains(searchText.ToLower()));
          }

          var record = query
            .OrderBy(es => es.Name)
            .Select(es => new ExchangeSettingModel
            {
              Id = es.Id,
              Name = es.Name,
              LoginType = es.LoginType,
              HealthStatus = es.HealthStatus,
              IsEnabled = es.IsEnabled,
              ExchangeLoginTypeModelName = GlobalSetting.Exchange.LoginTypes.TryGetValue(es.LoginType, out var loginTypeName) ? loginTypeName : string.Empty,
              ExchangeVersion = es.ExchangeVersion,
              TenantId = es.TenantId
            });
          exchangeSettingViewModel.ExchangeSettings = record.ToList();
          return exchangeSettingViewModel;

        });
      return result;

    }

    public ResultModel<ExchangeSettingModel> GetById(int tenantId, int id)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        //.ThenAuthorizeTenant(tenantId, TenantManager.EXCHANGE_SETTING, id)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(currentUser =>
        {
          var exchangeSettings = _exchangeSettings.Query(x => x.Id == id && x.TenantId == tenantId).FirstOrDefault();
          if (exchangeSettings == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          }

          var tenant = _logicService.Cache.Tenants.Get(exchangeSettings.TenantId);
          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var exchangeSetting = _exchangeSettings.Query(x => x.Id == id)
            .Select(x => new ExchangeSettingModel()
            {
              Id = x.Id,
              Name = x.Name,
              AzureClientId = x.AzureClientId,
              AzureClientSecret = x.AzureClientSecret,
              AzureTenant = x.AzureTenant,
              Description = x.Description,
              EmailAddress = x.EmailAddress,
              ExchangeUrl = x.ExchangeUrl,
              ExchangeVersion = x.ExchangeVersion,
              HealthStatus = x.HealthStatus,
              IsEnabled = x.IsEnabled,
              LoginType = x.LoginType,
              ServiceUser = x.ServiceUser,
              ServiceUserPassword = x.ServiceUserPassword,
              TenantId = x.TenantId
            })
            .FirstOrDefault();
          return exchangeSetting;
        });
      return result;
    }

    public ResultModel<ExchangeSettingModel> Update(ExchangeSettingModel model)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        .ThenAuthorizeTenant(model?.TenantId)
        .ThenValidate(currentUser =>
        {
          var error = Validation(model);
          if (error != null)
          {
            return error;
          }

          if (model.Id <= 0)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_INVALID_ID);
          }

          var exchange = _exchangeSettings.Query(t => t.Id == model.Id).FirstOrDefault();
          if (exchange == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          }

          var tenant = _logicService.Cache.Tenants.Get(model.TenantId);
          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
          }

          var hasTenantSameName = _logicService.Cache
           .Tenants
           .GetExchangeSettings(model.TenantId)
           .Any(t => t.Id != model.Id && t.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase));

          if (hasTenantSameName)
          {
            return new ErrorModel(ErrorType.DUPLICATED, LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS);
          }
          return null;
        })
        .ThenImplement(currentUser =>
        {
          ExchangeSetting exchange = _exchangeSettings.Query(t => t.Id == model.Id).First();

          exchange.Name = model.Name;
          exchange.AzureClientId = model.AzureClientId;
          exchange.AzureClientSecret = model.AzureClientSecret;
          exchange.AzureTenant = model.AzureTenant;
          exchange.ModifiedBy = currentUser.Id;
          exchange.ModifiedDate = DateTime.UtcNow;
          exchange.Description = model.Description;
          exchange.EmailAddress = !string.IsNullOrEmpty(model.EmailAddress) ? model.EmailAddress.Trim().ToLower() : "";
          exchange.ExchangeUrl = model.ExchangeUrl;
          exchange.ExchangeVersion = model.ExchangeVersionModel.Name;
          exchange.HealthStatus = model.HealthStatus;
          exchange.IsEnabled = model.IsEnabled;
          exchange.LoginType = model.ExchangeLoginTypeModel.Id;
          exchange.ServiceUser = model.ServiceUser;
          exchange.ServiceUserPassword = model.ServiceUserPassword;

          if (exchange.IsEnabled)
          {
            var exchangeSettings = _exchangeSettings
              .Query(es => es.TenantId == exchange.TenantId && es.Id != exchange.Id)
              .ToList();
            if (exchangeSettings.Count() > 0)
            {
              foreach (var item in exchangeSettings)
              {
                item.IsEnabled = false;
              }
              _exchangeSettings.UpdateRange(exchangeSettings);
            }
          }

          int result = _dbContext.Save();

          model.Id = exchange.Id;
          _logicService.Cache.Tenants.Clear();

          return model;
        });
      return result;
    }

    private ErrorModel Validation(ExchangeSettingModel model)
    {
      #region model

      if (model == null)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
      }

      #endregion

      if (string.IsNullOrEmpty(model.Name))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NAME_IS_REQUIRED);
      }
      if (model.Name.Length > 50)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_NAME_IS);
      }
      if (model.ExchangeVersionModel == null)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED);
      }
      if (model.ExchangeLoginTypeModel == null)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_LOGIN_TYPE_IS_REQUIRED);
      }
      if (string.IsNullOrEmpty(model.ExchangeUrl))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_EXCHANGE_URL_IS_REQUIRED);
      }
      if (model.ExchangeUrl.Length > 255)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255);
      }

      #region new

      if (string.IsNullOrEmpty(model.EmailAddress))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_EMAIL_IS_REQUIRED);
      }
      if (ValidationHelper.IsValidEmail(model.EmailAddress) == false)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS);
      }
      if (model.EmailAddress.Length > 75)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75);
      }
      if (string.IsNullOrEmpty(model.ServiceUser))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_SERVICE_USER_IS_REQUIRED);
      }
      if (model.ServiceUser.Length > 50)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50);
      }

      if (model.ExchangeLoginTypeModel.Type == ExchangeLoginEnumType.NetworkLogin)
      {
        if (string.IsNullOrEmpty(model.ServiceUserPassword))
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED);
        }
        if (model.ServiceUserPassword.Length > 50)
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50);
        }
      }

      #endregion

      #region WebLogin

      if (model.ExchangeLoginTypeModel.Type == ExchangeLoginEnumType.WebLogin)
      {
        if (string.IsNullOrEmpty(model.AzureTenant))
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_AZURE_TENANT_IS_REQUIRED);
        }
        if (model.AzureTenant.Length > 255)
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255);
        }
        if (string.IsNullOrEmpty(model.AzureClientId))
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_AZURE_CLIENT_IS_REQUIRED);
        }
        if (model.AzureClientId.Length > 255)
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255);
        }
        if (string.IsNullOrEmpty(model.AzureClientSecret))
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED);
        }
        if (model.AzureClientSecret.Length > 255)
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255);
        }
      }

      #endregion

      return null;
    }
  }
}
