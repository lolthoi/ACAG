using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class CultureAction : ActionApi
  {
    public CultureAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    #region GET

    public async Task<ApiResult<List<LanguageModel>>> GetAllAsync()
    {
      var uri = $"api/Cultures";
      return await GetRequestAsync<List<LanguageModel>>(uri);
    }

    #endregion
  }
}
