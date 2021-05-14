using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Language;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class ResetPasswordModel
  {

    [Required(ErrorMessageResourceName = LangKey.MSG_PASSWORD_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [StringLength(100,
      ErrorMessageResourceName = LangKey.MSG_PASSWORD_IS_REQUIRED_MIN_LENGHT_IS_6_MAX_LENGHT_IS_100,
      ErrorMessageResourceType = typeof(Resource), MinimumLength = 6)
    ]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(ResourceType = typeof(Resource), Name = LangKey.CONFIRM_PASSWORD)]
    [Compare("Password",
      ErrorMessageResourceName = LangKey.MSG_THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH,
      ErrorMessageResourceType = typeof(Resource))
    ]
    public string ConfirmPassword { get; set; }

    public string Code { get; set; }

    public bool Status { get; set; } = false;
  }
}
