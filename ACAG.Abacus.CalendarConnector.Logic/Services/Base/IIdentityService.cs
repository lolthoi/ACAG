using ACAG.Abacus.CalendarConnector.Models;

namespace ACAG.Abacus.CalendarConnector.Logic.Services.Base
{
  public interface IIdentityService
  {
    LoginUserModel GetCurrentUser();
  }
}
