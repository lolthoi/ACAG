using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.Language;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class ForgotPasswordModel
  {
    [Required(ErrorMessageResourceName = LangKey.MSG_EMAIL_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(75, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, ErrorMessageResourceType = typeof(Resource))]
    [RegularExpression("^\\s*[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})\\s*$",
      ErrorMessageResourceName = LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS,
      ErrorMessageResourceType = typeof(Resource))
    ]
    public string Email { get; set; }

    public string Url { get; set; }

    public string Code { get; set; }
    public bool IsSuccess { get; set; } = false;

    public string LanguageCode { get; set; }
  }
}
