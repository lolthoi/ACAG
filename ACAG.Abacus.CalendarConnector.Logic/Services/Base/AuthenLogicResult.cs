using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.Logic.Services.Base
{
  public enum TenantManager
  {
    PAY_TYPE,
    ABACUS_SETTING,
    EXCHANGE_SETTING,
    USER
  }

  public class ValidationAuthenResult : ValidationResult
  {

    public ValidationAuthenResult(LogicResult result)
      : base(result)
    {
    }

    public ValidationAuthenResult ThenAuthorizeUser(int? userId)
    {
      if (_result.IsValid)
      {
        var user = _result.Services.Cache.Users.Get(userId.Value);
        if(user == null)
        {
          _result.Error = new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
        }
        if (_result.CurrentUser.Role == Roles.USER && _result.CurrentUser.Id != userId)
        {
          _result.Error = new ErrorModel(ErrorType.NO_DATA_ROLE, LangKey.DATA_DOES_NOT_EXIST);
        }
      }
      return this;
    }

    public ValidationAuthenResult ThenAuthorizeTenant(int? tenantId)
    {
      if (_result.IsValid)
      {
        if (tenantId != null && tenantId <= 0)
        {
          _result.Error = new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
        }
        else if (_result.CurrentUser.Role == Roles.USER && _result.CurrentUser.TenantId != tenantId)
        {
          _result.Error = new ErrorModel(ErrorType.NO_DATA_ROLE, LangKey.DATA_DOES_NOT_EXIST);
        }
        else if (tenantId != null && !HasTenant(tenantId.Value))
        {
          _result.Error = new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
        }
      }
      return this;
    }

    private bool HasTenant(int tenantId)
    {
      var result = _result.Services.Cache.Tenants.Get(tenantId);

      return result == null ? false : true;
    }

    public ImplementAuthenResult ThenValidate(Func<LoginUserModel.LoginUser, ErrorModel> func)
    {
      if (_result.IsValid)
      {
        try
        {
          if (func != null)
            _result.Error = func.Invoke(_result.CurrentUser);
        }
        catch (Exception ex)
        {
          _result.Services.Logger.LogError("VALIDATEDATA: " + ex.ToString());
          _result.Error = new ErrorModel(ErrorType.INTERNAL_ERROR, ex.Message);
        }
      }

      return new ImplementAuthenResult(_result);
    }

    public override ImplementAuthenResult ThenValidate(Func<ErrorModel> func)
    {
      if (_result.IsValid)
      {
        try
        {
          _result.Error = func.Invoke();
        }
        catch (Exception ex)
        {
          _result.Services.Logger.LogError("VALIDATEDATA: " + ex.ToString());
          _result.Error = new ErrorModel(ErrorType.INTERNAL_ERROR, ex.Message);
        }
      }

      return new ImplementAuthenResult(_result);
    }
  }
  public class ImplementAuthenResult : ImplementResult
  {
    public ImplementAuthenResult(LogicResult result) : base (result) { }

    public ResultModel<T> ThenImplement<T>(Func<LoginUserModel.LoginUser, T> func)
    {
      if (_result.IsValid)
      {
        try
        {
          var data = func.Invoke(_result.CurrentUser);
          return new ResultModel<T>(data);
        }
        catch (Exception ex)
        {
          _result.Services.Logger.LogError("IMPLEMENT: " + ex.ToString());
          _result.Error = new ErrorModel(ErrorType.INTERNAL_ERROR, ex.Message);
        }
      }

      return new ResultModel<T>(_result.Error);
    }
  }
}
