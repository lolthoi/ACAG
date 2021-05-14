using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.Language;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class PayTypeEditModel
  {
    [Range(1, 99999, ErrorMessageResourceName = "MSG_CODE_NUMBER_RANGE_IS_FROM_1_TO_99999", ErrorMessageResourceType = typeof(Resource))]
    public int Code { get; set; }

    [Required(ErrorMessageResourceName = "MSG_PAYTYPE_DISPLAY_NAME_IS_REQUIRED", ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(250, ErrorMessageResourceName = "MAX_LENGTH_OF_DISPLAY_NAME_IS_250", ErrorMessageResourceType = typeof(Resource))]
    [MinLength(1, ErrorMessageResourceName = "MIN_LENGTH_OF_DISPLAY_NAME_IS_1", ErrorMessageResourceType = typeof(Resource))]
    public string DisplayName { get; set; }
    
    public bool IsAppointmentPrivate { get; set; }

    public bool IsAppointmentAwayState { get; set; }

    public bool IsEnabled { get; set; }
  }
}
