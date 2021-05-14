using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class RoleAction : ActionApi
  {
    public RoleAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<AppRoleViewModel>> GetAllAsync()
    {
      var uri = $"api/Roles";
      return await GetRequestAsync<AppRoleViewModel>(uri);
    }

    #endregion
  }
}
