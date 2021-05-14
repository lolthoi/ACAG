using System;
using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.Language;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class TenantModel
  {
    public int Id { get; set; }
    private string _name;
    [Required(ErrorMessageResourceName = LangKey.MSG_NAME_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(50, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_NAME_IS, ErrorMessageResourceType = typeof(Resource))]
    public string Name 
    {
      get
      {
        return string.IsNullOrWhiteSpace(_name) ? string.Empty : _name.Trim();
      }
      set
      {
        _name = value.Trim();
      }
    }
    public string Description { get; set; }
    public int Number { get; set; }
    public int? AbacusSettingId { get; set; }
    public string AbacusSettingName { get; set; }
    public bool? AbacusHealthStatus { get; set; }

    public int? ExchangeSettingId { get; set; }

    public string ExchangeSettingName { get; set; }
    public bool? ExchangeHealthStatus { get; set; }

    public int ScheduleTimer { get; set; }
    public bool IsEnabled { get; set; }
    public string Status { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
  }
}
