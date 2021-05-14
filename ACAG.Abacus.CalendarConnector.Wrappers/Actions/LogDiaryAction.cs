using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class LogDiaryAction : ActionApi
  {
    public LogDiaryAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<List<LogDiaryModel>>> GetByTenantAsync(int tenantId)
    {
      var uri = $"api/Tenant/{tenantId}/LogDiaries";
      return await GetRequestAsync<List<LogDiaryModel>>(uri);
    }

    #endregion

    #region POST

    public async Task<ApiResult<LogDiaryModel>> CreateAsync(int tenantId, LogDiaryEditModel model)
    {
      var uri = $"api/Tenant/{tenantId}/LogDiary";
      return await PostRequestAsync<LogDiaryModel>(uri, model);
    }

    #endregion

    #region DELETE

    public async Task<ApiResult<bool>> DeleteAllAsync(int tenantId)
    {
      var uri = $"api/Tenant/{tenantId}/LogDiaries";
      return await DeleteRequestAsync<bool>(uri);
    }

    public async Task<ApiResult<bool>> DisableRangeAsync(int tenantId, List<int> ids)
    {
      var uri = $"api/Tenant/{tenantId}/LogDiaries/Ids";
      return await DeleteRequestAsync<bool>(uri, ids);
    }

    #endregion
  }
}
