using System;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.Logic.Services.Base
{
  public interface ICommonService
  {
    IServiceProvider ServiceProvider { get; }

    IUnitOfWork<CalendarConnectorContext> DbContext { get; }

    public CacheProvider Cache { get; }

    ILogger Logger { get; }

  }

  public abstract class CommonService : ICommonService
  {
    private readonly IServiceProvider _serviceProvider;
    IServiceProvider ICommonService.ServiceProvider { get { return _serviceProvider; } }

    private readonly IUnitOfWork<CalendarConnectorContext> _dbContext;
    IUnitOfWork<CalendarConnectorContext> ICommonService.DbContext { get { return _dbContext; } }

    private readonly CacheProvider _cacheProvider;
    CacheProvider ICommonService.Cache { get { return _cacheProvider; } }

    private readonly ILogger _logger;
    ILogger ICommonService.Logger { get { return _logger; } }

    public CommonService(
      IServiceProvider serviceProvider,
      IUnitOfWork<CalendarConnectorContext> dbContext,
      CacheProvider cacheProvider,
      ILogger logger
      )
    {
      _serviceProvider = serviceProvider;
      _dbContext = dbContext;
      _cacheProvider = cacheProvider;
      _logger = logger;
    }
  }
}
