using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class AbacusSettingAction : ActionApi
  {
    public AbacusSettingAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<AbacusSettingModel>> GetByIdAsync(int tenantId, int abacusSettingId)
    {
      var uri = $"api/Tenant/{tenantId}/AbacusSettings/{abacusSettingId}";
      return await GetRequestAsync<AbacusSettingModel>(uri);
    }

    public async Task<ApiResult<AbacusSettingModel>> GetByTenantAsync(int tenantId)
    {
      var uri = $"api/Tenant/{tenantId}/AbacusSettings";
      return await GetRequestAsync<AbacusSettingModel>(uri);
    }

    #endregion

    #region POST

    public async Task<ApiResult<AbacusSettingModel>> CreateAsync(int tenantId, AbacusSettingEditModel model)
    {
      var uri = $"api/Tenant/{tenantId}/AbacusSettings";
      return await PostRequestAsync<AbacusSettingModel>(uri, model);
    }

    public async Task<ApiResult<bool>> CheckConnectionAsync(int tenantId, AbacusSettingConnectionModel model)
    {
      var uri = $"api/Tenant/{tenantId}/AbacusSettings/Connection";
      return await PostRequestAsync<bool>(uri, model);
    }

    #endregion

    #region PUT

    public async Task<ApiResult<AbacusSettingModel>> UpdateAsync(int tenantId, int abacusSettingId, AbacusSettingEditModel model)
    {
      var uri = $"api/Tenant/{tenantId}/AbacusSettings/{abacusSettingId}";
      return await PutRequestAsync<AbacusSettingModel>(uri, model);
    }

    #endregion

    #region DELETE

    public async Task<ApiResult<bool>> DeleteAsync(int tenantId, int abacusSettingId)
    {
      var uri = $"api/Tenant/{tenantId}/AbacusSettings/{abacusSettingId}";
      return await DeleteRequestAsync<bool>(uri);
    }

    #endregion
  }
}
