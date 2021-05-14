using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.Language;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class TenantEditModel
  {
    public int Id { get; set; }
    [Required(ErrorMessageResourceName = LangKey.MSG_NAME_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(50, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_NAME_IS, ErrorMessageResourceType = typeof(Resource))]
    public string Name { get; set; }

    public string Description { get; set; }

    public int Number { get; set; }

    public int ScheduleTimer { get; set; }

    public bool IsEnabled { get; set; }
  }
}
