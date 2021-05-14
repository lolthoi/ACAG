using System;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.Logic.Services.Base
{
  public interface ILogicService<T> : ICommonService
  {
    ValidationResult Start();
  }

  public class LogicService<T> : CommonService, ILogicService<T>
  {
    public LogicService(
      IServiceProvider serviceProvider,
      IUnitOfWork<CalendarConnectorContext> dbContext,
      CacheProvider cacheProvider,
      ILoggerFactory loggerFactory)
      : base (serviceProvider, dbContext, cacheProvider, loggerFactory.CreateLogger<T>())
    {
    }

    public ValidationResult Start()
    {
      var logicResult = new LogicResult(null, this);
      return new ValidationResult(logicResult);
    }
  }
}
