using System;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data
{
  public partial class UserMD
  {
    public int Id { get; set; }
    public int CultureId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Comment { get; set; }
    public bool IsEnabled { get; set; }
    public string ResetCode { get; set; }
    public DateTime? ExpireTime { get; set; }
  }
}
