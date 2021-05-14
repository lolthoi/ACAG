using System;
using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
  public class AbacusData : AuditedEntity
  {
    [Key]
    public int ID { get; set; }

    public int TenantId { get; set; }
    public long AbacusID { get; set; }
    public string ExchangeID { get; set; }
    public DateTime? InsertDateTime { get; set; }
    public string MailAccount { get; set; }
    public string Subject { get; set; }
    public DateTime? DateTimeStart { get; set; }
    public DateTime? DateTimeEnd { get; set; }
    public int Status { get; set; }

  }
}
