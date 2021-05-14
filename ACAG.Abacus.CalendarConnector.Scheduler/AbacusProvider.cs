using System;
using System.Collections.Concurrent;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.Scheduler
{
  public class AbacusProvider : IScheduler, IDisposable
  {
    private readonly ConcurrentDictionary<int, AbacusInstance> _instances = new ConcurrentDictionary<int, AbacusInstance>();

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public AbacusProvider(IServiceProvider serviceProvider,
      CacheProvider cacheProvider)
    {
      _serviceProvider = serviceProvider;
      _logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<AbacusProvider>();
    }

    public void StartAll()
    {
      StopAll();

      using (var scope = _serviceProvider.CreateScope())
      {
        var _cacheProvider = scope.ServiceProvider.GetService<CacheProvider>();
        var tenants = _cacheProvider.Tenants.GetValues();
        foreach (var tenant in tenants.Where(t => t.IsEnabled))
        {
          Start(tenant);
        }
      }
    }

    public void Start(int tenantId)
    {
      using (var scope = _serviceProvider.CreateScope())
      {
        var _cacheProvider = scope.ServiceProvider.GetService<CacheProvider>();
        var tenant = _cacheProvider.Tenants.Get(tenantId);
        if (tenant == null || !tenant.IsEnabled)
        {
          if (_instances.TryRemove(tenantId, out var instance) && instance.Status)
          {
            instance.Stop();
            instance.Dispose();
          }
          return;
        }
        Start(tenant);
      }

    }

    private void Start(TenantMD tenant)
    {
      try
      {
        var instance = _instances.TryGetValue(tenant.Id, out var item)
          ? SaveInstance(item, tenant)
          : SaveInstance(null, tenant);

        if (instance != null)
        {
          instance.Start();
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(string.Format("StartTenant:TenantId={0}", tenant.Id), ex);
      }
    }

    public void StopAll()
    {
      foreach (var item in _instances)
      {
        Stop(item.Key);
      }
    }

    public void Stop(int tenantId)
    {
      try
      {
        if (_instances.TryGetValue(tenantId, out var item) && item.Status)
        {
          item.Stop();
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(string.Format("StopTenant:TenantId={0}", tenantId), ex);
      }
    }

    private AbacusInstance SaveInstance(AbacusInstance instance, TenantMD tenant)
    {
      if (!tenant.IsEnabled || !tenant.AbacusSettingId.HasValue)
      {
        return null;
      }

      var tenantModel = new Models.TenantModel
      {
        Id = tenant.Id,
        Name = tenant.Name,
        AbacusSettingId = tenant.AbacusSettingId,
        Description = tenant.Description,
        IsEnabled = tenant.IsEnabled,
        Number = tenant.Number,
        ScheduleTimer = tenant.ScheduleTimer
      };

      if (instance == null)
      {
        instance = new AbacusInstance(tenantModel, _serviceProvider);
      }
      else
      {
        instance.UpdateSetting(tenantModel);
      }

      if (!instance.IsValid)
      {
        return null;
      }
      var isAdded = _instances.TryAdd(tenant.Id, instance);
      return isAdded ? instance : null;
    }

    public void Dispose()
    {
      StopAll();
    }

    ~AbacusProvider()
    {
      Dispose();
    }
  }
}
