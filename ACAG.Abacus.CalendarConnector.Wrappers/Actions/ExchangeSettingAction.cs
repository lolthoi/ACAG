using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class ExchangeSettingAction : ActionApi
  {
    public ExchangeSettingAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<ExchangeSettingViewModel>> GetAllAsync(int tenantId, string searchText)
    {
      var uri = $"api/Tenant/{tenantId}/ExchangeSettings?searchText={searchText}";
      return await GetRequestAsync<ExchangeSettingViewModel>(uri);
    }

    public async Task<ApiResult<ExchangeSettingModel>> GetByIdAsync(int tenantId, int exchangeSettingId)
    {
      var uri = $"api/Tenant/{tenantId}/ExchangeSettings/{exchangeSettingId}";
      return await GetRequestAsync<ExchangeSettingModel>(uri);
    }

    public async Task<ApiResult<List<ExchangeVersionModel>>> GetAllExchangeVersionsAsync()
    {
      var uri = $"api/ExchangeSettings/ExchangeVersions";
      return await GetRequestAsync<List<ExchangeVersionModel>>(uri);
    }

    public async Task<ApiResult<List<ExchangeLoginTypeModel>>> GetAllLoginTypesAsync()
    {
      var uri = $"api/ExchangeSettings/LoginTypes";
      return await GetRequestAsync<List<ExchangeLoginTypeModel>>(uri);
    }

    #endregion

    #region POST

    public async Task<ApiResult<ExchangeSettingModel>> CreateAsync(int tenantId, ExchangeSettingModel model)
    {
      var uri = $"api/Tenant/{tenantId}/ExchangeSettings";
      return await PostRequestAsync<ExchangeSettingModel>(uri, model);
    }

    public async Task<ApiResult<string>> CheckConnectionAsync(int tenantId, ExchangeSettingModel model)
    {
      var uri = $"api/Tenant/{tenantId}/ExchangeSettings/Connection";
      return await PostRequestAsync<string>(uri, model);
    }

    #endregion

    #region PUT

    public async Task<ApiResult<ExchangeSettingModel>> UpdateAsync(int tenantId, int exchangeSettingId, ExchangeSettingModel model)
    {
      var uri = $"api/Tenant/{tenantId}/ExchangeSettings/{exchangeSettingId}";
      return await PutRequestAsync<ExchangeSettingModel>(uri, model);
    }

    #endregion

    #region DELETE

    public async Task<ApiResult<bool>> DeleteAsync(int tenantId, int exchangeSettingId)
    {
      var uri = $"api/Tenant/{tenantId}/ExchangeSettings/{exchangeSettingId}";
      return await DeleteRequestAsync<bool>(uri);
    }

    #endregion
  }
}
