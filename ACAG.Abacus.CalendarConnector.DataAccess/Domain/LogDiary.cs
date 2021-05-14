using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
  public class LogDiary : AuditedEntity
  {
    [Key]
    public int Id { get; set; }
    public int TenantId { get; set; }
    public DateTime? DateTime { get; set; }
    public string Data { get; set; }
    public string Error { get; set; }
    public bool IsEnabled { get; set; }
    public virtual Tenant Tenant { get; set; }
  }
}
