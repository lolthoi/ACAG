using System;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class LogDiaryModel
  {
    public int Id { get; set; }
    public int TenantId { get; set; }
    [Required]
    public DateTime? DateTime { get; set; }
    [Required]
    public string Data { get; set; }
    [Required]
    public string Error { get; set; }
    public bool IsEnabled { get; set; }
    public TenantModel Tenant { get; set; }
  }
}
