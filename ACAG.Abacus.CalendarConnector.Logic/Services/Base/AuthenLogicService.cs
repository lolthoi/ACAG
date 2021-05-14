using System;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.Logic.Services.Base
{
  public interface IAuthenLogicService<T> : ICommonService
  {
    LogicResult Start();
  }

  public class AuthenLogicService<T> : CommonService, IAuthenLogicService<T>
  {
    private readonly IIdentityService _identityService;

    public AuthenLogicService(
      IIdentityService identityService,

      IServiceProvider serviceProvider,
      IUnitOfWork<CalendarConnectorContext> dbContext,
      CacheProvider cacheProvider,
      ILoggerFactory loggerFactory)
      : base(serviceProvider, dbContext, cacheProvider, loggerFactory.CreateLogger<T>())
    {
      _identityService = identityService;
    }

    private LoginUserModel.LoginUser GetCurrentUser()
    {
      var currentUser = _identityService.GetCurrentUser();
      return currentUser.User;
    }

    public LogicResult Start()
    {
      var currentUser = GetCurrentUser();
      return new LogicResult(currentUser, this);
    }
  }
}
