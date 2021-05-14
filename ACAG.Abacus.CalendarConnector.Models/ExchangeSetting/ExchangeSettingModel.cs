using System;
using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.Language;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class ExchangeSettingModel
  {
    public int Id { get; set; }
    public int TenantId { get; set; }
    [Required(ErrorMessageResourceName = LangKey.MSG_NAME_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(50, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_NAME_IS, ErrorMessageResourceType = typeof(Resource))
    ]
    public string Name { get; set; }
    public string ExchangeVersion { get; set; }
    [Required(ErrorMessageResourceName = LangKey.MSG_EXCHANGE_URL_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))
    ]
    [MaxLength(255,ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_EXCHANGE_URL_IS_255, ErrorMessageResourceType = typeof(Resource))
    ]
    public string ExchangeUrl { get; set; }
    public int LoginType { get; set; }

    [RequiredCustomLoginType(nameof(ExchangeLoginTypeModel), ErrorMessageResourceName = LangKey.MSG_AZURE_TENANT_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(255, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_AZURE_TENANT_IS_255, ErrorMessageResourceType = typeof(Resource))]
    public string AzureTenant { get; set; }

    [RequiredCustomLoginType(nameof(ExchangeLoginTypeModel), ErrorMessageResourceName = LangKey.MSG_AZURE_CLIENT_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(255, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_IS_255, ErrorMessageResourceType = typeof(Resource))]
    public string AzureClientId { get; set; }

    [RequiredCustomLoginType(nameof(ExchangeLoginTypeModel), ErrorMessageResourceName = LangKey.MSG_AZURE_CLIENT_SECRET_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(255, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_AZURE_CLIENT_SECRET_IS_255, ErrorMessageResourceType = typeof(Resource))]
    public string AzureClientSecret { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessageResourceName = LangKey.MSG_EMAIL_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(75, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, ErrorMessageResourceType = typeof(Resource))]
    [RegularExpression("^\\s*[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})\\s*$",
      ErrorMessageResourceName = LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS,
      ErrorMessageResourceType = typeof(Resource))
    ]
    public string EmailAddress { get; set; }
    [Required(ErrorMessageResourceName = LangKey.MSG_SERVICE_USER_IS_REQUIRED,ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(50,ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_IS_50,ErrorMessageResourceType = typeof(Resource))]
    public string ServiceUser { get; set; }

    [MaxLength(50,ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_SERVICE_USER_PASSWORD_IS_50,ErrorMessageResourceType = typeof(Resource))]
    [RequiredCustomLoginNetworkType(nameof(ExchangeLoginTypeModel), ErrorMessageResourceName = LangKey.MSG_SERVICE_USER_PASSWORD_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    public string ServiceUserPassword { get; set; }

    public bool? HealthStatus { get; set; }
    public bool IsEnabled { get; set; }
    public string ExchangeLoginTypeModelName { get; set; }
    [Required(ErrorMessageResourceName = LangKey.MSG_LOGIN_TYPE_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    public ExchangeLoginTypeModel ExchangeLoginTypeModel { get; set; }
    [Required(ErrorMessageResourceName = LangKey.MSG_EXCHANGE_VERSION_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    public ExchangeVersionModel ExchangeVersionModel { get; set; }
    
  }
}
