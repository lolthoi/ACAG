using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.Language;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class AbacusSettingEditModel
  {
    [Required(ErrorMessageResourceName = LangKey.MSG_NAME_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(255, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_NAME_IS_255, ErrorMessageResourceType = typeof(Resource))]
    public string Name { get; set; }

    [Required(ErrorMessageResourceName = LangKey.MSG_SERVICE_URL_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(255, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_SERVICE_URL_IS_255, ErrorMessageResourceType = typeof(Resource))]
    public string ServiceUrl { get; set; }

    [Required(ErrorMessageResourceName = LangKey.MSG_SERVICE_PORT_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [Range(1, 99999, ErrorMessageResourceName = LangKey.MSG_SERVICE_PORT_RANGE_IS_FROM_1_TO_99999, ErrorMessageResourceType = typeof(Resource))]
    public int ServicePort { get; set; }

    [Required(ErrorMessageResourceName = LangKey.MSG_SERVICE_USE_SSL_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    public bool ServiceUseSsl { get; set; }

    [Required(ErrorMessageResourceName = LangKey.MSG_SERVICE_USER_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(255, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_255, ErrorMessageResourceType = typeof(Resource))]
    public string ServiceUser { get; set; }

    [Required(ErrorMessageResourceName = LangKey.MSG_SERVICE_PASSWORD_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(50, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_SERVICE_PASSWORD_IS_50, ErrorMessageResourceType = typeof(Resource))]
    public string ServiceUserPassword { get; set; }
  }
}
