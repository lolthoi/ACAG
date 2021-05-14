using System.ComponentModel.DataAnnotations;
using ACAG.Abacus.CalendarConnector.Language;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class UserModel
  {
    public int Id { get; set; }
    public int CultureId { get; set; }

    private string _firstName;

    [Required(ErrorMessageResourceName = LangKey.MSG_FIRST_NAME_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(50, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_FIRST_NAME_IS_50, ErrorMessageResourceType = typeof(Resource))]
    [MinLength(3, ErrorMessageResourceName = LangKey.MSG_MIN_LENGTH_OF_FIRST_NAME_IS_3, ErrorMessageResourceType = typeof(Resource))]
    public string FirstName 
    {
      get
      {
        return string.IsNullOrWhiteSpace(_firstName) ? string.Empty : _firstName.Trim();
      }
      set
      {
        _firstName = value.Trim();
      }
    }

    private string _lastName;

    [Required(ErrorMessageResourceName = LangKey.MSG_LAST_NAME_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(50, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_LAST_NAME_IS_50, ErrorMessageResourceType = typeof(Resource))]
    [MinLength(3, ErrorMessageResourceName = LangKey.MSG_MIN_LENGTH_OF_LAST_NAME_IS_3, ErrorMessageResourceType = typeof(Resource))]
    public string LastName
    {
      get
      {
        return string.IsNullOrWhiteSpace(_lastName) ? string.Empty : _lastName.Trim();
      }
      set
      {
        _lastName = value.Trim();
      }
    }

    private string _email;
    [Required(ErrorMessageResourceName = LangKey.MSG_EMAIL_IS_REQUIRED, ErrorMessageResourceType = typeof(Resource))]
    [MaxLength(75, ErrorMessageResourceName = LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75, ErrorMessageResourceType = typeof(Resource))]
    [RegularExpression("^\\s*[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})\\s*$", 
      ErrorMessageResourceName = LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS, 
      ErrorMessageResourceType = typeof(Resource))
    ]
    public string Email 
    {
      get
      {
        return string.IsNullOrWhiteSpace(_email) ? string.Empty : _email.Trim();
      }
      set
      {
        _email = value.Trim();
      }
    }
    public string Comment { get; set; }
    public bool IsEnabled { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public string Status { get; set; }
    public AppRoleModel AppRole { get; set; }

    public TenantModelForUser Tenant { get; set; }
    public string TenantName { get; set; }

    public LanguageModel Language { get; set; }
    public bool IsShowSendPasswordButton { get; set; } = false;
    public bool CanDelete { get; set; } = true;
    public string FullName
    {
      get
      {
        return FirstName + " " + LastName;
      }
    }

  }
}
