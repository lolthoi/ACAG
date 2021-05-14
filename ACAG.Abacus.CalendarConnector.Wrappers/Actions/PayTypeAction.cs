using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class PayTypeAction : ActionApi
  {
    public PayTypeAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<PayTypeViewModel>> GetAllAsync(int tenantId, string searchText)
    {
      var uri = $"api/Tenant/{tenantId}/PayTypes?searchText={searchText}";
      return await GetRequestAsync<PayTypeViewModel>(uri);
    }

    public async Task<ApiResult<PayTypeModel>> GetByIdAsync(int tenantId, int payTypeId)
    {
      var uri = $"api/Tenant/{tenantId}/PayTypes/{payTypeId}";
      return await GetRequestAsync<PayTypeModel>(uri);
    }

    #endregion

    #region POST

    public async Task<ApiResult<PayTypeModel>> CreateAsync(int tenantId, PayTypeEditModel model)
    {
      var uri = $"api/Tenant/{tenantId}/PayTypes";
      return await PostRequestAsync<PayTypeModel>(uri, model);
    }

    #endregion

    #region PUT

    public async Task<ApiResult<PayTypeModel>> UpdateAsync(int tenantId, int payTypeId, PayTypeEditModel model)
    {
      var uri = $"api/Tenant/{tenantId}/PayTypes/{payTypeId}";
      return await PutRequestAsync<PayTypeModel>(uri, model);
    }

    #endregion

    #region DELETE

    public async Task<ApiResult<bool>> DeleteAsync(int tenantId, int payTypeId)
    {
      var uri = $"api/Tenant/{tenantId}/PayTypes/{payTypeId}";
      return await DeleteRequestAsync<bool>(uri);
    }

    #endregion
  }
}
