using ACAG.Abacus.CalendarConnector.Models.Abacus;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ACAG.Abacus.CalendarConnector.Abacus
{
  public class CustomAuthenticationHandler : DelegatingHandler
  {
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
      if (request.Headers.TryGetValues("Authorization", out IEnumerable<string> values))
      {
        var token = values.FirstOrDefault();
        var user = DecryptToken(token, out string[] roles);

        if (user != null)
        {
          HttpContext.Current.User = new GenericPrincipal(user, roles);
        }
      }
      return base.SendAsync(request, cancellationToken);
    }

    private UserLogin DecryptToken(string token, out string[] roles)
    {
      if (string.IsNullOrEmpty(token))
      {
        roles = null;
        return null;
      }

      roles = new string[] { "admin" };

      return new UserLogin
      {
        Name = "Nam Test"
      };
    }
  }
}