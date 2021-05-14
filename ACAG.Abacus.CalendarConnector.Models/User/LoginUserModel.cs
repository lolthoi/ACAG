using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class LoginUserModel
  {
    public string Key { get; set; }

    public LoginUser User { get; set; }

    public class LoginUser
    {
      public int Id { get; set; }
      public string Email { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      public int? CultureId { get; set; }
      public int? TenantId { get; set; }
      public Roles Role { get; set; }

      public string FullName
      {
        get
        {
          return FirstName + " " + LastName;
        }
      }
    }
  }
}
