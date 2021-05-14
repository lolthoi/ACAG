using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.Logic.Services.Base
{
  public class LogicResult
  {
    public readonly LoginUserModel.LoginUser CurrentUser;
    public readonly ICommonService Services;

    public bool IsValid { get; private set; } = true;

    private ErrorModel _error;
    public ErrorModel Error
    {
      get { return _error; }
      set
      {
        _error = value;
        IsValid = _error == null;
      }
    }

    public LogicResult(LoginUserModel.LoginUser currentUser, ICommonService service)
    {
      CurrentUser = currentUser;
      Services = service;
    }

    public ValidationAuthenResult ThenAuthorize(params Roles[] roles)
    {
      if (roles != null && roles.Length != 0)
      {
        if (CurrentUser == null)
        {
          Error = new ErrorModel(ErrorType.NOT_AUTHORIZED, "Not authorized");
        }
        else
        {
          //check deactive user
          var user = Services.Cache.Users.Get(CurrentUser.Id);
          if (!user.IsEnabled)
          {
            Error = new ErrorModel(ErrorType.CONFLICTED_ROLE_CHANGE, "Conficted role");
          }
          //check current role with actual role
          var appRole = Services.Cache.Users.GetRole(CurrentUser.Id);
          if (appRole == null || appRole.Role != CurrentUser.Role)
          {
            Error = new ErrorModel(ErrorType.CONFLICTED_ROLE_CHANGE, "Conficted role");
          }
          else if (!roles.Contains(CurrentUser.Role))
          {
            Error = new ErrorModel(ErrorType.NO_ROLE, "No role");
          }
        }
      }
      
      return new ValidationAuthenResult(this);
    }
  }

  public class ValidationResult
  {
    protected readonly LogicResult _result;

    public ValidationResult(LogicResult result)
    {
      _result = result;
    }

    public virtual ImplementResult ThenValidate(Func<ErrorModel> func)
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

      return new ImplementResult(_result);
    }
  }

  public class ImplementResult
  {
    protected readonly LogicResult _result;

    public ImplementResult(LogicResult result)
    {
      _result = result;
    }

    public ResultModel<T> ThenImplement<T>(Func<T> func)
    {
      if (_result.IsValid)
      {
        try
        {
          var data = func.Invoke();
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
