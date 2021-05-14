using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class AppSettingAction : ActionApi
  {
    public AppSettingAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<AppFooterModel>> GetAllAsync()
    {
      var uri = $"api/AppSettings";
      return await GetRequestAsync<AppFooterModel>(uri);
    }

    #endregion
  }
}
