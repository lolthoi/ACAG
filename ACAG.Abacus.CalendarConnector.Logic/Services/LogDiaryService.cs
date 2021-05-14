using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface ILogDiaryService
  {
    ResultModel<LogDiaryModel> Create(LogDiaryModel logDiaryModel);
    ResultModel<List<LogDiaryModel>> GetByTenant(int tenantId);
    ResultModel<bool> DisableRange(int tenantId, List<int> ids);
    ResultModel<bool> DeleteAll(int tenantId);
  }

  public class LogDiaryService : ILogDiaryService
  {
    private readonly IAuthenLogicService<LogDiaryService> _logicService;
    private readonly IUnitOfWork<CalendarConnectorContext> _dbContext;

    private readonly IEntityRepository<User> _users;
    private readonly IEntityRepository<Tenant> _tenants;
    private readonly IEntityRepository<LogDiary> _logDiaries;

    public LogDiaryService(IAuthenLogicService<LogDiaryService> logicService)
    {
      _logicService = logicService;
      _dbContext = logicService.DbContext;

      _users = _dbContext.GetRepository<User>();
      _tenants = _dbContext.GetRepository<Tenant>();
      _logDiaries = _dbContext.GetRepository<LogDiary>();
    }

    public ResultModel<LogDiaryModel> Create(LogDiaryModel logDiaryModel)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(null)
        .ThenAuthorizeTenant(logDiaryModel?.TenantId)
        .ThenValidate(() =>
        {
          return Validate(logDiaryModel);
        })
        .ThenImplement(currentUser =>
        {
          var logDiary = new LogDiary()
          {
            TenantId = logDiaryModel.TenantId,
            DateTime = logDiaryModel.DateTime,
            Data = logDiaryModel.Data,
            Error = logDiaryModel.Error,
            IsEnabled = true,
            CreatedBy = currentUser.Id,
            CreatedDate = DateTime.UtcNow
          };

          _logDiaries.Add(logDiary);

          var tenant = _tenants
                .Query(t => t.Id == logDiaryModel.TenantId)
                .FirstOrDefault();

          if (tenant != null)
          {
            tenant.LogDiarys.Add(logDiary);
          }

          int result = _dbContext.Save();

          logDiaryModel.Id = logDiary.Id;

          _logicService.Cache.Tenants.Clear();

          return logDiaryModel;
        });

      return result;
    }

    public ResultModel<bool> DeleteAll(int tenantId)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(null)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(() =>
        {
          if (tenantId <= 0)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          var tenant = _logicService.Cache.Tenants.Get(tenantId);

          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var logDiaries = _logDiaries.Query(l => l.TenantId == tenantId);

          foreach (var log in logDiaries)
          {
            _logDiaries.Delete(log);
          }

          _dbContext.Save();
          _logicService.Cache.Tenants.Clear();

          return true;
        });

      return result;
    }

    public ResultModel<bool> DisableRange(int tenantId, List<int> ids)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(null)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(() =>
        {
          if (tenantId <= 0)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          var tenant = _logicService.Cache.Tenants.Get(tenantId);

          if (ids == null || tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var logDiaries = _logDiaries.Query(l => l.IsEnabled == true && l.TenantId == tenantId).ToList();

          foreach (var id in ids)
          {
            foreach (var log in logDiaries)
            {
              if(id == log.Id)
              {
                log.IsEnabled = false;
                log.ModifiedDate = DateTime.UtcNow;
                log.ModifiedBy = currentUser.Id;

                break;
              }
            }
          }

          _dbContext.Save();
          _logicService.Cache.Tenants.Clear();

          return true;
        });

      return result;
    }

    public ResultModel<List<LogDiaryModel>> GetByTenant(int tenantId)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(null)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(currentUser =>
        {
          if (tenantId <= 0)
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);

          var tenant = _logicService.Cache.Tenants.Get(tenantId);

          if (tenant == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var tenant = _tenants.Query(t => t.Id == tenantId)
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
          .FirstOrDefault();

          var rs = _logDiaries.Query(l => l.IsEnabled == true && l.TenantId == tenantId)
          .Select(l => new LogDiaryModel() { 
            Id = l.Id,
            TenantId = l.TenantId,
            DateTime = l.DateTime,
            Data = l.Data,
            Error = l.Error,
            IsEnabled = l.IsEnabled,
            Tenant = tenant
          })
          .OrderByDescending(l => l.DateTime)
          .ThenBy(l => l.Data)
          .ThenBy(l => l.Error)
          .ToList();

          return rs;
        });
      return result;
    }

    #region Private methods

    private ErrorModel Validate(LogDiaryModel model)
    {
      if (model == null
        || model.DateTime == null 
        || string.IsNullOrEmpty(model.Data) 
        || string.IsNullOrEmpty(model.Error))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
      }

      return null;
    }

    #endregion
  }
}
