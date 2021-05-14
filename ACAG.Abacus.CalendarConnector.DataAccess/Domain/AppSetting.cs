using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
  public class AppSetting
  {
    [Key]
    public string Id { get; set; }
    public string Value { get; set; }
    public bool Status { get; set; }
  }
}
