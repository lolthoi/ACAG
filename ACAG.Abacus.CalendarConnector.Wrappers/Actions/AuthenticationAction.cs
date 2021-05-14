using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class AuthenticationAction : ActionApi
  {
    public AuthenticationAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<UserAuthModel>> GetByEmailAsync(string email)
    {
      var uri = $"api/Authentications/{email}";
      return await GetRequestAsync<UserAuthModel>(uri);
    }

    #endregion
  }
}
