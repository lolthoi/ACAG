using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class UserAction : ActionApi
  {
    public UserAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<UserViewModel>> GetAllAsync(string searchText)
    {
      var uri = $"api/Users?searchText={searchText}";
      return await GetRequestAsync<UserViewModel>(uri);
    }

    public async Task<ApiResult<UserModel>> GetByIdAsync(int userId)
    {
      var uri = $"api/Users/{userId}";
      return await GetRequestAsync<UserModel>(uri);
    }

    public async Task<ApiResult<UserViewModel>> GetTenantUsers(int tenantId, string searchText)
    {
      var uri = $"api/Tenants/{tenantId}/Users?searchText={searchText}";
      return await GetRequestAsync<UserViewModel>(uri);
    }

    #endregion

    #region POST

    public async Task<ApiResult<UserModel>> CreateAsync(UserModel model, string password)
    {
      var uri = $"api/Users?password={password}";
      return await PostRequestAsync<UserModel>(uri, model);
    }

    #endregion

    #region PUT

    public async Task<ApiResult<UserModel>> UpdateAsync(UserModel model)
    {
      var uri = $"api/Users";
      return await PutRequestAsync<UserModel>(uri, model);
    }

    #endregion

    #region DELETE

    public async Task<ApiResult<bool>> DeleteAsync(int userId)
    {
      var uri = $"api/Users/{userId}";
      return await DeleteRequestAsync<bool>(uri);
    }

    #endregion
  }
}
