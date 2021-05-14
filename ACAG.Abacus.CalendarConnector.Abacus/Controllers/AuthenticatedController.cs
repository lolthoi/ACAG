using ACAG.Abacus.CalendarConnector.Models.Abacus;

using System.Web;
using System.Web.Http;

namespace ACAG.Abacus.CalendarConnector.Abacus.Controllers
{
  public abstract class AuthenticatedController : ApiController
  {
    private UserLogin _currentUser;
    public UserLogin CurrentUser
    {
      get
      {
        if (_currentUser == null)
          _currentUser = HttpContext.Current.User.Identity as UserLogin;
        return _currentUser;
      }
    }
  }
}