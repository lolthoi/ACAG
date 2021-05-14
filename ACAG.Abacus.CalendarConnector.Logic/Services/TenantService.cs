using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface ITenantService
  {
    ResultModel<TenantViewModel> GetAll(string searchText = "");
    ResultModel<TenantModelForUserViewModel> GetAllTenants();
    ResultModel<TenantModel> GetById(int id);
    ResultModel<TenantModel> Update(TenantModel tenantModel);
    ResultModel<TenantModel> Create(TenantModel tenantModel);
    ResultModel<bool> Delete(int id);
    TenantSettingModel GetSettings(int id);
  }

  public class TenantService : ITenantService
  {
    private readonly IAuthenLogicService<TenantService> _logicService;
    private readonly ILogDiaryService _logDiaryService;
    private readonly IScheduler _scheduler;

    private readonly IUnitOfWork<CalendarConnectorContext> _dbContext;

    private readonly IEntityRepository<Tenant> _tenants;
    private readonly IEntityRepository<TenantUserRel> _tenantUserRels;
    private readonly IEntityRepository<ExchangeSetting> _exchangeSettings;
    private readonly IEntityRepository<PayType> _payTypes;    

    public TenantService(IAuthenLogicService<TenantService> logicService,
      IScheduler scheduler,
      ILogDiaryService logDiaryService)
    {
      _logicService = logicService;
      _scheduler = scheduler;
      _logDiaryService = logDiaryService;
      _dbContext = logicService.DbContext;

      _tenants = _dbContext.GetRepository<Tenant>();
      _tenantUserRels = _dbContext.GetRepository<TenantUserRel>();
      _exchangeSettings = _dbContext.GetRepository<ExchangeSetting>();
      _payTypes = _dbContext.GetRepository<PayType>();
    }

    public ResultModel<TenantModelForUserViewModel> GetAllTenants()
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenValidate(currentUser => null)
        .ThenImplement(currentUser =>
        {
          TenantModelForUserViewModel tenantViewModel = new TenantModelForUserViewModel();
          IEnumerable<TenantMD> query = null;
          if (currentUser.Role == Roles.USER)
          {
            query = _logicService.Cache.Users.GetTenants(currentUser.Id);
          }
          else
          {
            query = _logicService.Cache.Tenants.GetValues().AsEnumerable();
          }

          var records = query
              .OrderBy(t => t.Name)
              .Select(t => new TenantModelForUser
              {
                Id = t.Id,
                Name = t.Name,
              })
              .ToList();

          tenantViewModel.Tenants = records;
          return tenantViewModel;
        });
      return result;
    }

    public ResultModel<TenantViewModel> GetAll(string searchText = "")
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenValidate(currentUser => null)
        .ThenImplement(currentUser =>
        {
          TenantViewModel tenantViewModel = new TenantViewModel();
          IEnumerable<TenantMD> query = null;
          if (currentUser.Role == Roles.USER)
          {
            query = _logicService.Cache.Users.GetTenants(currentUser.Id);
          }
          else
          {
            query = _logicService.Cache.Tenants.GetValues().AsEnumerable();
          }
          if (!string.IsNullOrWhiteSpace(searchText))
          {
            searchText = searchText.Trim();
            query = query.Where(f =>
              f.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
              f.Number.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase));
          }

          var records = query
              .OrderBy(t => t.Name)
              .Select(t => new TenantModel
              {
                Id = t.Id,
                Name = t.Name,
                AbacusSettingId = t.AbacusSettingId,
                Description = t.Description,
                IsEnabled = t.IsEnabled,
                Number = t.Number,
                ScheduleTimer = t.ScheduleTimer
              })
              .OrderBy(t => t.Name)
              .ToList();

          tenantViewModel.Tenants = records;
          return tenantViewModel;
        });
      return result;
    }

    public ResultModel<TenantModel> GetById(int id)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenAuthorizeTenant(id)
        .ThenValidate(currentUser =>
        {
          var tenant = _logicService.Cache.Tenants.Get(id);
          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          }
          return null;
        })
        .ThenImplement(currentUser =>
        {
          var tenantMD = _logicService.Cache.Tenants.Get(id);
          if (tenantMD == null)
            return null;

          var tenant = new TenantModel
          {
            Id = tenantMD.Id,
            Name = tenantMD.Name,
            AbacusSettingId = tenantMD.AbacusSettingId,
            Description = tenantMD.Description,
            IsEnabled = tenantMD.IsEnabled,
            Number = tenantMD.Number,
            ScheduleTimer = tenantMD.ScheduleTimer
          };

          return tenant;
        });

      return result;
    }

    public ResultModel<TenantModel> Update(TenantModel tenantModel)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenValidate(currentUser =>
        {
          var error = ValidateTenant(tenantModel);
          if (error != null)
          {
            return error;
          }

          if (tenantModel.Id <= 0)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_INVALID_ID);
          }

          var tenant = _logicService.Cache.Tenants.Get(tenantModel.Id);
          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          }

          var hasTenantSameName = _logicService.Cache
            .Tenants
            .GetValues()
            .Any(t => t.Id != tenantModel.Id && t.Name.Equals(tenantModel.Name, StringComparison.OrdinalIgnoreCase));

          if (hasTenantSameName)
          {
            return new ErrorModel(ErrorType.DUPLICATED, LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS);
          }

          var hasTenantSameNumber = _logicService.Cache
          .Tenants
          .GetValues()
          .Any(t => t.Id != tenantModel.Id && t.Number == tenantModel.Number);

          if (hasTenantSameNumber)
          {
            return new ErrorModel(ErrorType.DUPLICATED, LangKey.MSG_THE_ENTERED_NUMBER_ALREADY_EXISTS);
          }
          return null;

        })
        .ThenImplement(currentUser =>
        {
          Tenant tenant = _tenants.Query(t => t.Id == tenantModel.Id).FirstOrDefault();
          tenant.Name = tenantModel.Name;
          tenant.Description = tenantModel.Description ?? string.Empty;
          tenant.Number = tenantModel.Number;
          tenant.IsEnabled = tenantModel.IsEnabled;
          tenant.ModifiedBy = currentUser.Id;
          tenant.ModifiedDate = DateTime.UtcNow;
          tenant.ScheduleTimer = tenantModel.ScheduleTimer;

          int result = _dbContext.Save();

          tenantModel.Id = tenant.Id;

          _logicService.Cache.Tenants.Clear();

          _scheduler.Start(tenant.Id);

          return tenantModel;
        });

      return result;
    }

    public ResultModel<TenantModel> Create(TenantModel tenantModel)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR)
        .ThenValidate(currentUser =>
        {
          var error = ValidateTenant(tenantModel);
          if (error != null)
            return error;

          var hasTenantSameName = _logicService.Cache
          .Tenants
          .GetValues()
          .Any(t => t.Name.Equals(tenantModel.Name, StringComparison.OrdinalIgnoreCase));

          if (hasTenantSameName)
          {
            return new ErrorModel(ErrorType.DUPLICATED, LangKey.MSG_THE_ENTERED_NAME_ALREADY_EXISTS);
          }
          var hasTenantSameNumber = _logicService.Cache
          .Tenants
          .GetValues()
          .Any(t => t.Number == tenantModel.Number);

          if (hasTenantSameNumber)
          {
            return new ErrorModel(ErrorType.DUPLICATED, LangKey.MSG_THE_ENTERED_NUMBER_ALREADY_EXISTS);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var tenant = new Tenant()
          {
            Name = tenantModel.Name,
            Description = tenantModel.Description ?? string.Empty,
            Number = tenantModel.Number,
            IsEnabled = tenantModel.IsEnabled,
            CreatedBy = currentUser.Id,
            CreatedDate = DateTime.UtcNow,
            ScheduleTimer = tenantModel.ScheduleTimer
          };
          _tenants.Add(tenant);

          int result = _dbContext.Save();

          tenantModel.Id = tenant.Id;
          _logicService.Cache.Tenants.Clear();

          _scheduler.Start(tenant.Id);

          return tenantModel;

        });

      return result;
    }

    public ResultModel<bool> Delete(int id)
    {
      Tenant tenant = null;
      List<TenantUserRel> tenantUserRels = null;
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR)
        .ThenValidate(currentUser =>
        {
          tenant = _tenants.Query(t => t.Id == id).FirstOrDefault();
          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          }
          tenantUserRels = _tenantUserRels.Query(x => x.TenantId == id).ToList();
          if (tenantUserRels != null && tenantUserRels.Count > 0)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_THIS_TENANT_HAS_BEEN_USED);
          }
          return null;
        })
        .ThenImplement(currentUser =>
        {
          var exchangeSettings = _exchangeSettings.Query(es => es.TenantId == id).ToList();
          if (exchangeSettings != null || exchangeSettings.Count > 0)
          {
            _exchangeSettings.DeleteRange(exchangeSettings);
          }

          var payTypes = _payTypes.Query(pt => pt.TenantId == id).ToList();
          if (payTypes != null || payTypes.Count > 0)
          {
            _payTypes.DeleteRange(payTypes);
          }

          _logDiaryService.DeleteAll(id);
          _tenants.Delete(tenant);

          int result = _dbContext.Save();
          _logicService.Cache.Tenants.Remove(id);

          _scheduler.Stop(id);

          return true;
        });

      return result;
    }

    public TenantSettingModel GetSettings(int id)
    {
      throw new NotImplementedException();
    }

    private ErrorModel ValidateTenant(TenantModel tenantModel)
    {
      if (tenantModel == null)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
      }
      if (string.IsNullOrEmpty(tenantModel.Name))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NAME_IS_REQUIRED);
      }
      if (tenantModel.Name.Length > 50)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_NAME_IS);
      }
      if (tenantModel.Number < 1 || tenantModel.Number > 99999)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NUMBER_MUST_BETWEEN_1_AND_99999);
      }
      if (tenantModel.ScheduleTimer < 1 || tenantModel.ScheduleTimer > 180)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_SCHEDULER_SHOULD_BE_BETWEEN_1_AND_180);
      }

      return null;
    }
  }
}
