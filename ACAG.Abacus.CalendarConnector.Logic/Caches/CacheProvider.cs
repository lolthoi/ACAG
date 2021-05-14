using ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Caches
{
  public class CacheProvider
  {
    private readonly IServiceProvider _serviceProvider;

    #region Tenants
    public TenantAllCache Tenants { get; private set; }

    private DataKeyCache<int, List<UserMD>> _tenantUsers = new DataKeyCache<int, List<UserMD>>();

    private DataKeyCache<int, AbacusSettingMD> _abacusSettings = new DataKeyCache<int, AbacusSettingMD>();


    /// <summary>
    /// int: tenantId
    /// </summary>
    private DataKeyCache<int, List<ExchangeSettingMD>> _exchangeSettings = new DataKeyCache<int, List<ExchangeSettingMD>>();

    /// <summary>
    /// int: tenantId
    /// </summary>
    private DataKeyCache<int, List<PayTypeMD>> _payTypes = new DataKeyCache<int, List<PayTypeMD>>();

    #endregion

    #region Users

    public UserAllCache Users { get; private set; }

    public DataAllCache<int, AppRoleMD> AppRoles = new DataAllCache<int, AppRoleMD>();

    private DataKeyCache<int, List<TenantMD>> _userTenants = new DataKeyCache<int, List<TenantMD>>();

    /// <summary>
    /// int: userId
    /// </summary>
    private DataKeyCache<int, List<AppRoleMD>> _userAppRoles = new DataKeyCache<int, List<AppRoleMD>>();

    #endregion

    #region Other properties

    public CultureAllCache Cultures { get; private set; } = new CultureAllCache();

    public AppSettingAllCache AppSettings { get; private set; } = new AppSettingAllCache();
    #endregion

    #region Constructor and private functions

    public CacheProvider(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;

      Tenants = new TenantAllCache(this);
      Users = new UserAllCache(this);

      RegisterInitData();
    }

    private T UseDbContext<T>(Func<IUnitOfWork<CalendarConnectorContext>, T> func)
    {
      using (var scope = _serviceProvider.CreateScope())
      {
        var dbContext = scope.ServiceProvider.GetService<IUnitOfWork<CalendarConnectorContext>>();
        return func.Invoke(dbContext);
      }
    }

    private void RegisterInitData()
    {
      Tenants.OnNeedResource += Tenants_OnNeedResource;
      _tenantUsers.OnNeedValueIfKeyNotFound += _tenantUsers_OnNeedValueIfKeyNotFound;
      _abacusSettings.OnNeedValueIfKeyNotFound += _abacusSettings_OnNeedValueIfKeyNotFound;
      _exchangeSettings.OnNeedValueIfKeyNotFound += _exchangeSettings_OnNeedValueIfKeyNotFound;
      _payTypes.OnNeedValueIfKeyNotFound += _payTypes_OnNeedValueIfKeyNotFound;

      Users.OnNeedResource += Users_OnNeedResource;
      _userTenants.OnNeedValueIfKeyNotFound += _userTenants_OnNeedValueIfKeyNotFound;

      AppRoles.OnNeedResource += _appRoles_OnNeedResource;
      _userAppRoles.OnNeedValueIfKeyNotFound += _userAppRoles_OnNeedValueIfKeyNotFound;
      Cultures.OnNeedResource += Cultures_OnNeedResource;

      AppSettings.OnNeedResource += AppSettings_OnNeedResource;
    }

    #endregion

    #region Events

    #region Tenant events

    private Dictionary<int, TenantMD> Tenants_OnNeedResource(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var result = dbContext.GetRepository<Tenant>()
              .Query()
              .ToDictionary(t => t.Id, t => new TenantMD
              {
                Id = t.Id,
                Name = t.Name,
                AbacusSettingId = t.AbacusSettingId,
                Description = t.Description,
                IsEnabled = t.IsEnabled,
                Number = t.Number,
                ScheduleTimer = t.ScheduleTimer
              });

        return result;
      });
    }

    private List<UserMD> _tenantUsers_OnNeedValueIfKeyNotFound(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var tenantId = (int)paramArrs.First();

        var data = dbContext.GetRepository<TenantUserRel>()
                  .Query(t => t.TenantId == tenantId)
                  .Select(t => t.UserId)
                  .ToList();

        var result = data
                  .Select(t => Users.Get(t))
                  .Where(t => t != null)
                  .ToList();

        return result;
      });
    }

    private List<PayTypeMD> _payTypes_OnNeedValueIfKeyNotFound(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var tenantId = (int)paramArrs.First();

        var result = dbContext.GetRepository<PayType>()
                  .Query(t => t.TenantId == tenantId)
                  .Select(t => new PayTypeMD
                  {
                    Id = t.Id,
                    TenantId = t.TenantId,
                    IsEnabled = t.IsEnabled,
                    Code = t.Code,
                    DisplayName = t.DisplayName,
                    IsAppointmentAwayState = t.IsAppointmentAwayState,
                    IsAppointmentPrivate = t.IsAppointmentPrivate
                  })
                  .ToList();

        return result;
      });
    }

    private List<ExchangeSettingMD> _exchangeSettings_OnNeedValueIfKeyNotFound(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var tenantId = (int)paramArrs.First();

        var result = dbContext.GetRepository<ExchangeSetting>()
                  .Query(t => t.TenantId == tenantId)
                  .Select(t => new ExchangeSettingMD
                  {
                    Id = t.Id,
                    Name = t.Name,
                    TenantId = t.TenantId,
                    AzureClientId = t.AzureClientId,
                    AzureClientSecret = t.AzureClientSecret,
                    AzureTenant = t.AzureTenant,
                    Description = t.Description,
                    EmailAddress = t.EmailAddress,
                    ExchangeUrl = t.ExchangeUrl,
                    ExchangeVersion = t.ExchangeVersion,
                    HealthStatus = t.HealthStatus,
                    IsEnabled = t.IsEnabled,
                    LoginType = t.LoginType,
                    ServiceUser = t.ServiceUser,
                    ServiceUserPassword = t.ServiceUserPassword
                  })
                  .ToList();

        return result;
      });
    }

    private AbacusSettingMD _abacusSettings_OnNeedValueIfKeyNotFound(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var id = (int)paramArrs.First();

        var result = dbContext.GetRepository<AbacusSetting>()
                  .Query(t => t.Id == id)
                  .Select(t => new AbacusSettingMD
                  {
                    Id = t.Id,
                    Name = t.Name,
                    ServiceUrl = t.ServiceUrl,
                    ServicePort = t.ServicePort,
                    ServiceUseSSL = t.ServiceUseSSL,
                    ServiceUser = t.ServiceUser,
                    ServiceUserPassword = t.ServiceUserPassword,
                    HealthStatus = t.HealthStatus
                  })
                  .FirstOrDefault();

        return result;
      });
    }

    #endregion

    #region User events

    private Dictionary<int, UserMD> Users_OnNeedResource(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var result = dbContext.GetRepository<User>()
          .Query()
          .ToDictionary(t => t.Id, t => new UserMD
          {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            CultureId = t.CultureId,
            Comment = t.Comment,
            IsEnabled = t.IsEnabled,
            ExpireTime = t.ExpireTime,
            ResetCode = t.ResetCode
          });

        return result;
      });
    }

    private List<TenantMD> _userTenants_OnNeedValueIfKeyNotFound(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var userId = (int)paramArrs.First();

        var data = dbContext.GetRepository<TenantUserRel>()
                  .Query(t => t.UserId == userId)
                  .Select(t => t.TenantId)
                  .ToList();

        var result = data
                  .Select(t => Tenants.Get(t))
                  .Where(t => t != null)
                  .ToList();

        return result;
      });
    }

    private List<AppRoleMD> _userAppRoles_OnNeedValueIfKeyNotFound(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var userId = (int)paramArrs.First();

        var data = dbContext.GetRepository<AppRoleRel>()
                  .Query(t => t.UserId == userId)
                  .Select(t => t.AppRoleId)
                  .ToList();

        var result = data
                  .Select(t => AppRoles.Get(t))
                  .Where(t => t != null)
                  .ToList();

        return result;
      });
    }

    private Dictionary<int, AppRoleMD> _appRoles_OnNeedResource(object sender, params object[] paramArrs)
    {
      return UseDbContext(dbContext =>
      {
        var result = dbContext.GetRepository<AppRole>()
              .Query()
              .Where(t => t.IsEnabled)
              .ToDictionary(t => t.Id, t => new AppRoleMD
              {
                Id = t.Id,
                Code = t.Code,
                IsAdministrator = t.IsAdministrator
              });

        return result;
      });
    }

    #endregion

    private Dictionary<string, AppSettingMD> AppSettings_OnNeedResource(object sender, params object[] paramArrs)
    {
      var appSettings = UseDbContext(dbContext =>
      {
        var data = dbContext.GetRepository<AppSetting>()
          .Query()
          .Select(t => new AppSettingMD
          {
            Id = t.Id,
            Value = t.Value,
            Status = t.Status
          })
          .ToList();

        var result = data
        .GroupBy(t => t.Value)
        .ToDictionary(t => t.Key, t => t.First());

        return result;
      });
      return appSettings;
    }

    private Dictionary<string, CultureMD> Cultures_OnNeedResource(object sender, params object[] paramArrs)
    {
      var cultures = UseDbContext(dbContext =>
      {
        var data = dbContext.GetRepository<Culture>()
              .Query()
              .ToList();

        var result = data
                  .GroupBy(t => t.Code)
                  .ToDictionary(t => t.Key, t =>
                  {
                    var culture = t.First();
                    return new CultureMD
                    {
                      Id = culture.Id,
                      Code = culture.Code,
                      DisplayName = culture.DisplayName,
                      IsEnabled = culture.IsEnabled
                    };
                  });

        return result;
      });

      return cultures;
    }

    #endregion

    #region Cache extensions

    public class CultureAllCache : DataAllCache<string, CultureMD>
    {
    }

    public class AppSettingAllCache : DataAllCache<string, AppSettingMD>
    {

    }

    public class TenantAllCache : DataAllCache<int, TenantMD>
    {
      private readonly CacheProvider _provider;
      public TenantAllCache(CacheProvider provider)
      {

        _provider = provider;
      }

      public List<UserMD> GetUsers(int tenantId)
      {
        var users = _provider._tenantUsers.Get(tenantId);
        return users;
      }
      public AbacusSettingMD GetAbacusSetting(int tenantId)
      {
        var tenant = Get(tenantId);
        if (tenant == null || tenant.AbacusSettingId == null)
          return null;

        var result = _provider._abacusSettings.Get(tenant.AbacusSettingId.Value);
        return result;
      }

      public void ClearAbacusSetting(int tenantId)
      {
        var tenant = Get(tenantId);
        if (tenant == null || tenant.AbacusSettingId == null)
          return;

        _provider._abacusSettings.Remove(tenant.AbacusSettingId.Value);
      }

      public List<ExchangeSettingMD> GetExchangeSettings(int tenantId)
      {
        var result = _provider._exchangeSettings.Get(tenantId);
        return result;
      }

      public ExchangeSettingMD GetExchangeSetting(int tenantId)
      {
        var data = _provider._exchangeSettings.Get(tenantId);
        var result = data.FirstOrDefault(t => t.IsEnabled);
        return result;
      }

      public List<PayTypeMD> GetPayTypes(int tenantId)
      {
        var result = _provider._payTypes.Get(tenantId);
        return result;
      }

      public override bool Remove(int tenantId)
      {
        base.Remove(tenantId);

        _provider._tenantUsers.Remove(tenantId);
        _provider._abacusSettings.Remove(tenantId);
        _provider._exchangeSettings.Remove(tenantId);
        _provider._payTypes.Remove(tenantId);

        _provider._userTenants.Clear();

        return true;
      }
      public override void Clear()
      {
        base.Clear();

        _provider._tenantUsers.Clear();
        _provider._abacusSettings.Clear();
        _provider._exchangeSettings.Clear();
        _provider._payTypes.Clear();

        _provider._userTenants.Clear();

      }
    }

    public class UserAllCache : DataAllCache<int, UserMD>
    {
      private readonly CacheProvider _provider;
      public UserAllCache(CacheProvider provider)
      {

        _provider = provider;
      }

      public List<AppRoleMD> GetRoles(int userId)
      {
        return _provider._userAppRoles.Get(userId);
      }

      public AppRoleMD GetRole(int userId)
      {
        var roles = _provider._userAppRoles.Get(userId);
        if (roles == null || roles.Count == 0)
          return null;

        return roles.First();
      }

      public List<TenantMD> GetTenants(int userId)
      {
        return _provider._userTenants.Get(userId);
      }

      public TenantMD GetTenant(int userId)
      {
        var tenants = _provider._userTenants.Get(userId);
        if (tenants == null || tenants.Count == 0)
          return null;

        return tenants.First();
      }

      public override bool Remove(int userId)
      {
        base.Remove(userId);
        _provider._userAppRoles.Remove(userId);
        _provider._userTenants.Remove(userId);

        _provider._tenantUsers.Clear();

        return true;
      }
      public override void Clear()
      {
        base.Clear();

        _provider._userAppRoles.Clear();
        _provider._userTenants.Clear();
        _provider._tenantUsers.Clear();
      }
    }

    #endregion
  }
}
