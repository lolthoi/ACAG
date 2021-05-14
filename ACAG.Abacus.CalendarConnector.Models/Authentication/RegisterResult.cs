using System.Collections.Generic;

namespace ACAG.Abacus.CalendarConnector.Models.Authentication
{
  public class RegisterResult
  {
    public bool Successful { get; set; }
    public IEnumerable<string> Errors { get; set; }
  }
}
