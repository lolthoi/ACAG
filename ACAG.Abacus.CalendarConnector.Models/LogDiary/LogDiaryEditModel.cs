using System;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class LogDiaryEditModel
  {
    [Required]
    public DateTime? DateTime { get; set; }
    [Required]
    public string Data { get; set; }
    [Required]
    public string Error { get; set; }
  }
}
