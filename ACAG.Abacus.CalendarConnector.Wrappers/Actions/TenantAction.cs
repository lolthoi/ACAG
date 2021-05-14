using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class TenantAction : ActionApi
  {
    public TenantAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<TenantModelForUserViewModel>> GetAllForUserAsync()
    {
      var uri = $"api/Tenants/GetAllTenants";
      return await GetRequestAsync<TenantModelForUserViewModel>(uri);
    }

    public async Task<ApiResult<TenantViewModel>> GetAllAsync(string searchText)
    {
      var uri = $"api/Tenants?searchText={searchText}";
      return await GetRequestAsync<TenantViewModel>(uri);
    }

    public async Task<ApiResult<TenantModel>> GetByIdAsync(int tenantId)
    {
      var uri = $"api/Tenants/{tenantId}";
      return await GetRequestAsync<TenantModel>(uri);
    }

    #endregion

    #region POST

    public async Task<ApiResult<TenantModel>> CreateAsync(TenantModel model)
    {
      var uri = $"api/Tenants/";
      return await PostRequestAsync<TenantModel>(uri, model);
    }

    #endregion

    #region PUT

    public async Task<ApiResult<TenantModel>> UpdateAsync(TenantModel model)
    {
      var uri = $"api/Tenants/";
      return await PutRequestAsync<TenantModel>(uri, model);
    }

    #endregion

    #region DELETE

    public async Task<ApiResult<bool>> DeleteAsync(int tenantId)
    {
      var uri = $"api/Tenants/{tenantId}";
      return await DeleteRequestAsync<bool>(uri);
    }

    #endregion
  }
}
