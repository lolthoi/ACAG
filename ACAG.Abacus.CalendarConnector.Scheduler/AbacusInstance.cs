using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.Exchange;
using ACAG.Abacus.CalendarConnector.Logic.Abacus;
using ACAG.Abacus.CalendarConnector.Logic.Common;
using ACAG.Abacus.CalendarConnector.Logic.Exchange;
using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.Scheduler
{
  public partial class AbacusInstance : IDisposable
  {
    private Models.TenantModel _tenant;
    private bool _isError;

    private CacheProvider _cache;
    private EWSService _ews;
    private AbacusListener _listener;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public AbacusInstance(Models.TenantModel tenant, IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
      _cache = _serviceProvider.GetService<CacheProvider>();
      _logger = _serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<AbacusInstance>();

      UpdateSetting(tenant);
    }

    public void UpdateSetting(Models.TenantModel tenant)
    {
      _tenant = tenant;

      var exSettingMD = _cache.Tenants.GetExchangeSetting(tenant.Id);
      if (exSettingMD == null || !exSettingMD.IsEnabled)
      {
        _isError = true;
        return;
      }

      var exchangeSetting = new Models.ExchangeSettingModel
      {
        Id = exSettingMD.Id,
        Name = exSettingMD.Name,
        AzureClientId = exSettingMD.AzureClientId,
        AzureClientSecret = exSettingMD.AzureClientSecret,
        AzureTenant = exSettingMD.AzureTenant,
        Description = exSettingMD.Description,
        EmailAddress = exSettingMD.EmailAddress,
        ExchangeUrl = exSettingMD.ExchangeUrl,
        ExchangeVersion = exSettingMD.ExchangeVersion,
        HealthStatus = exSettingMD.HealthStatus,
        IsEnabled = exSettingMD.IsEnabled,
        LoginType = exSettingMD.LoginType,
        ServiceUser = exSettingMD.ServiceUser,
        ServiceUserPassword = exSettingMD.ServiceUserPassword,
        TenantId = exSettingMD.TenantId
      };

      if (_ews == null)
      {
        _ews = new EWSService(exchangeSetting);
        _ews.OnExchangeDataInserted += OnExchangeInsertedDone;
      }
      else
      {
        _ews.Setting = exchangeSetting;
      }

      int timerInterval = tenant.ScheduleTimer.LimitToRange(1, 180) * 60 * 1000;
      if (_listener == null)
      {
        _listener = new AbacusListener(timerInterval);
        _listener.OnTimerAbacusJob += AbacusJob;
      }
      else
      {
        _listener.TimerInterval = timerInterval;
      }
    }

    private void OnExchangeInsertedDone(AppointmentModel data)
    {
      using (var scope = _serviceProvider.CreateScope())
      {
        var abacusService = scope.ServiceProvider.GetService<IAbacusService>();
        var status = abacusService.Save(data, _tenant.Id);
      }
    }

    public void Start()
    {
      if (IsValid)
      {
        _listener.Start();
      }
    }

    public void Stop()
    {
      if (IsValid)
      {
        _listener.Stop();
      }
    }

    public bool IsValid
    {
      get { return !_isError; }
    }

    public bool Status => _listener?.IsRunning ?? false;

    private void AbacusJob()
    {
      var data = GetAbacusData();
      using (var scope = _serviceProvider.CreateScope())
      {
        var abacusService = scope.ServiceProvider.GetService<IAbacusService>();
        DeleteAbacus(abacusService, data);

        ChangePayType(data);

        SaveAbacus(abacusService, data);
      }
    }

    private void DeleteAbacus(IAbacusService abacusService, List<AppointmentModel> data)
    {
      try
      {
        var deleteItems = abacusService.Delete(data, _tenant.Id);
        if (deleteItems != null && deleteItems.Count != 0)
        {
          deleteItems.ForEach(_ews.Delete);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        _logger.LogError(string.Empty, ex);
      }
    }

    private List<AppointmentModel> GetAbacusData()
    {
      using (var scope = _serviceProvider.CreateScope())
      {
        var _abacusApi = scope.ServiceProvider.GetService<IAbacusAPI>();
        var data = _abacusApi.GetAbacusData(_tenant);

        return data;
      }
    }

    private void ChangePayType(List<AppointmentModel> data)
    {
      var paytypes = _cache.Tenants.GetPayTypes(_tenant.Id).ToDictionary(t => t.Code, t => t);
      data.ForEach(t =>
      {
        if (int.TryParse(t.PayTypeCode, out int paytype) && paytypes.TryGetValue(paytype, out var payTypeMD))
        {
          t.Subject = payTypeMD.DisplayName;
          t.IsPrivate = payTypeMD.IsAppointmentPrivate;
          t.IsOutOfOffice = payTypeMD.IsAppointmentAwayState;
        }
      });
    }

    private void SaveAbacus(IAbacusService abacusService, List<AppointmentModel> data)
    {
      foreach (var item in data)
      {
        var existInDb = abacusService.CheckAbacusExist(item.AbacusID, _tenant.Id);
        if (!existInDb)
        {
          _ews.Insert(item);
          continue;
        }

        var entry = _ews.GetAbacusData(item);
        var ewsId = entry?.ExchangeID ?? string.Empty;
        if (string.IsNullOrEmpty(ewsId))
        {
          _ews.Insert(item);
        }
        else
        {
          entry.ExchangeID = ewsId;
          if (!IsEqual(entry, item))
          {
            item.ExchangeID = ewsId;
            _ews.Update(item);
          }
          var updated = abacusService.Update(item, _tenant.Id);
        }
      }
    }

    private bool IsEqual(AppointmentModel a1, AppointmentModel a2)
    {
      var res = a1.Subject == a2.Subject
        && IsEqual(a1.DateTimeStart, a2.DateTimeStart)
        && IsEqual(a1.DateTimeEnd, a2.DateTimeEnd)
        && a1.Status == a2.Status
        && a1.IsOutOfOffice == a2.IsOutOfOffice
        && a1.IsPrivate == a2.IsPrivate;

      return res;
    }

    private bool IsEqual(DateTime time1, DateTime time2)
    {
      var res = time1.Year == time2.Year
        && time1.Month == time2.Month
        && time1.Day == time2.Day
        && time1.Hour == time2.Hour
        && time1.Minute == time2.Minute;

      return res;
    }

    public void Dispose()
    {
      _listener?.Dispose();
    }
  }
}
