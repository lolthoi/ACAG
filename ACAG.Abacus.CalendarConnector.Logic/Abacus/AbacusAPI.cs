using System;
using System.Collections;
using System.Collections.Generic;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ACAG.Abacus.CalendarConnector.Logic.Abacus
{
  public interface IAbacusAPI
  {
    List<AppointmentModel> GetAbacusData(Models.TenantModel tenant);
  }

  public class AbacusAPI : IAbacusAPI
  {
    private readonly AbacusApiSettings _abacusApiSettings;
    private readonly WrapperAPI _wrapperAPI;
    private readonly CacheProvider _cache;
    private readonly ILogger _logger;
    private readonly IServiceProvider _provider;

    const string TEST_AbacusSetting_Description = "AbacusSetting Unit Test Description";

    public AbacusAPI(IOptions<AbacusApiSettings> abacusApiSettings,
      WrapperAPI wrapperAPI,
      CacheProvider cache,
      ILoggerFactory loggerFactory,
      IServiceProvider provider)
    {
      _abacusApiSettings = abacusApiSettings.Value;
      _wrapperAPI = wrapperAPI;
      _cache = cache;
      _logger = loggerFactory.CreateLogger<AbacusAPI>();
    }

    public List<AppointmentModel> GetAbacusData(Models.TenantModel tenant)
    {

      try
      {
        var setting = _cache.Tenants.GetAbacusSetting(tenant.Id);

        var abacusSetting = new AbacusSettingModel(
          setting.Name,
          TEST_AbacusSetting_Description,
          setting.ServiceUrl,
          setting.ServicePort,
          setting.ServiceUseSSL,
          setting.ServiceUser,
          setting.ServiceUserPassword);

        var tenantSetting = new TenantModel(
          tenant.Name,
          tenant.Description,
          tenant.Number,
          abacusSetting);

        var rq = _wrapperAPI.GetRequest("/api/reportaccessor", tenantSetting);
        var data = rq.ToContentObject<List<_ACAG_AbacusCalendarConnector>>();
        var appointments = new List<AppointmentModel>();
        var result = Logic.Abacus.Appointment.ConvertAPIData(data, ref appointments);

        return appointments ?? new List<AppointmentModel>();
      }
      catch (Exception ex)
      {
        var data = GetDataFromException(ex.Data);

        CreateLogDiary(tenant.Id, "AbaucsAPI:\r\n" + data, ex.Message);

        _logger.LogError("GetAbacusData: " + ex.ToString());

        return new List<AppointmentModel>();
      }
    }

    #region Private methods

    private string GetDataFromException(IDictionary data)
    {
      try
      {
        string rs = string.Empty;
        if (data.Count > 0)
        {
          foreach (DictionaryEntry de in data)
            rs += String.Format("Key: {0,-20} Value: {1}\r\n", "'" + de.Key.ToString() + "'", de.Value);
        }
        return rs;
      }
      catch (Exception ex)
      {
        _logger.LogError("GetDataFromException: " + ex.ToString());
      }
      return "Unknown Key-Value";
    }

    private void CreateLogDiary(int tenantId, string data, string error)
    {
      try
      {
        var logDiaryModel = new Models.LogDiaryModel
        {
          DateTime = DateTime.UtcNow,
          Data = data,
          Error = error,
          TenantId = tenantId
        };

        using (var scope = _provider.CreateScope())
        {
          var logDiaryService = scope.ServiceProvider.GetService<LogDiaryService>();
          logDiaryService.Create(logDiaryModel);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError("CreateLogDiary: " + ex.ToString());
      }
    }

    #endregion
  }
  /// <summary>
  /// Unit test for Scheduler
  /// </summary>
  public class Fake_AbacusAPI : IAbacusAPI
  {
    const int TEST_Data_EXPR_Project_RecNr = 1000;
    const int TEST_Data_NBW_LeArtNr = 1000;    
    const string TEST_Data_EXPR_E_Mail = "mbrt@all-consulting.ch";
    const string TEST_Data_Text = "abacustest by scheduler";
    DateTime TEST_Data_DateTime = DateTime.UtcNow;
    private _ACAG_AbacusCalendarConnector PrepareAbacusData(int counter)
    {
      var abacusData = new _ACAG_AbacusCalendarConnector()
      {
        EXPR_E_Mail = TEST_Data_EXPR_E_Mail,
        EXPR_Project_RecNr = string.Format("{0}", (TEST_Data_EXPR_Project_RecNr + counter)),
        INR = "",
        NAME = "",
        NBW_LeArtNr = string.Format("{0}", (TEST_Data_NBW_LeArtNr + counter)),
        TEXT = TEST_Data_Text,
        VORNAME = "",
        PKT_Event = "",
        PKT_StartTime = "2021-03-04 20:10:10",
        PKT_EndTime = "2021-03-04 21:11:11"
      };

      return abacusData;
    }

    public List<AppointmentModel> GetAbacusData(Models.TenantModel tenant)
    {
      var d = DateTime.UtcNow;
      List<AppointmentModel> appointments = new List<AppointmentModel>();
      List<_ACAG_AbacusCalendarConnector> abacusData = new List<_ACAG_AbacusCalendarConnector>();
      abacusData.Add(PrepareAbacusData(1));
      abacusData.Add(PrepareAbacusData(2));
      abacusData.Add(PrepareAbacusData(3));

      Dictionary<int, KeyValuePair<int, string>> result = Logic.Abacus.Appointment.ConvertAPIData(abacusData, ref appointments);

      return appointments;
    }
  }

}
