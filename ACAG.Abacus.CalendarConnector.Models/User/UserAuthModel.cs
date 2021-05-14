using System;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public class UserAuthModel
  {
    public int Id { get; set; }
    public int? TenantId { get; set; }
    public int? CultureId { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ExperiedOn { get; set; }
  }
}
