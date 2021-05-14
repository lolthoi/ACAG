using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.Logic.Exchange
{
  public interface IAbacusService
  {
    bool CheckAbacusExist(long abacusId, int tenantId);
    bool Insert(AppointmentModel data, int tenantId);
    bool Update(AppointmentModel data, int tenantId);
    bool Save(AppointmentModel data, int tenantId);


    /// <summary>
    /// return deleted items. Return null if error
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    List<AppointmentModel> Delete(List<AppointmentModel> data, int tenantId);
  }
  public class AbacusService : IAbacusService
  {
    private readonly IUnitOfWork<CalendarConnectorContext> _dbContext;
    private readonly ILogger _logger;
    public AbacusService(IUnitOfWork<CalendarConnectorContext> dbContext, ILoggerFactory loggerFactory)
    {
      _dbContext = dbContext;
      _logger = loggerFactory.CreateLogger<AbacusService>();
    }

    public bool CheckAbacusExist(long abacusId, int tenantId)
    {
      var res = _dbContext.GetRepository<AbacusData>()
        .Query(t => t.AbacusID == abacusId && t.TenantId == tenantId)
        .FirstOrDefault();

      return res != null;
    }

    public bool Insert(AppointmentModel data, int tenantId)
    {
      try
      {
        var instance = _dbContext.GetRepository<AbacusData>();
        var d = DateTime.UtcNow;
        instance.Add(new AbacusData
        {
          AbacusID = data.AbacusID,
          TenantId = tenantId,
          ExchangeID = data.ExchangeID ?? string.Empty,
          InsertDateTime = d,
          MailAccount = data.MailAccount,
          Subject = data.Subject,
          DateTimeStart = data.DateTimeStart,
          DateTimeEnd = data.DateTimeEnd,
          Status = data.Status,
          CreatedBy = -1,
          CreatedDate = d
        });

        _dbContext.Save();

        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(string.Empty, ex);
        Console.WriteLine(ex.Message);
        return false;
      }
    }

    public bool Update(AppointmentModel data, int tenantId)
    {
      try
      {
        var res = _dbContext.GetRepository<AbacusData>().Query(t => t.ID == data.ID && t.AbacusID == data.AbacusID).FirstOrDefault();
        if (res == null) return false;
        var d = DateTime.UtcNow;
        res.TenantId = tenantId;
        res.ExchangeID = data.ExchangeID ?? string.Empty;
        res.MailAccount = data.MailAccount;
        res.Subject = data.Subject;
        res.DateTimeStart = data.DateTimeStart;
        res.DateTimeEnd = data.DateTimeEnd;
        res.Status = data.Status;
        res.ModifiedBy = -1;
        res.ModifiedDate = d;

        _dbContext.Save();

        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(string.Empty, ex);
        Console.WriteLine(ex.Message);
        return false;
      }
    }

    public bool Save(AppointmentModel data, int tenantId)
    {
      var exist = CheckAbacusExist(data.AbacusID, tenantId);
      if (exist)
      {
        return Update(data, tenantId);
      }
      else
      {
        return Insert(data, tenantId);
      }
    }


    /// <summary>
    /// return deleted items. Return null if error
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public List<AppointmentModel> Delete(List<AppointmentModel> data, int tenantId)
    {
      try
      {
        var dics = data.ToDictionary(t => t.AbacusID, t => t);
        
        var abacusDatas = _dbContext.GetRepository<AbacusData>();
        var dbAbacusDatas = abacusDatas.Query(t => t.TenantId == tenantId).ToList();

        var outputs = new List<AppointmentModel>();
        foreach (var item in dbAbacusDatas)
        {
          if (!dics.ContainsKey(item.AbacusID))
          {
            abacusDatas.Delete(item);
            outputs.Add(new AppointmentModel
            {
              AbacusID = item.AbacusID,
              ID = item.ID,
              MailAccount = item.MailAccount
            });
          }
        };

        _dbContext.Save();
        return outputs;
      }
      catch (Exception ex)
      {
        _logger.LogError(string.Empty, ex);
        Console.WriteLine(ex.Message);
        return null;
      }
    }

  }
}
