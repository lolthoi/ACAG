using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public abstract class ActionApi
  {
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILocalStorageService _localStorage;

    public ActionApi(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
    {
      _clientFactory = clientFactory;
      _localStorage = localStorage;
    }

    protected async Task<ApiResult<T>> GetRequestAsync<T>(string functionApi)
    {
      return await HttpClientUtils.GetRequestAync<T>(_clientFactory, _localStorage, functionApi);
    }

    protected async Task<ApiResult<T>> PostRequestAsync<T>(string functionApi, object model)
    {
      return await HttpClientUtils.PostRequestAsync<T>(_clientFactory, _localStorage, functionApi, model);
    }

    protected async Task<ApiResult<T>> PutRequestAsync<T>(string functionApi, object model)
    {
      return await HttpClientUtils.PutRequestAsync<T>(_clientFactory, _localStorage, functionApi, model);
    }

    protected async Task<ApiResult<T>> DeleteRequestAsync<T>(string functionApi)
    {
      return await HttpClientUtils.DeleteRequestAsync<T>(_clientFactory, _localStorage, functionApi);
    }

    protected async Task<ApiResult<T>> DeleteRequestAsync<T>(string functionApi, List<int> ids)
    {
      return await HttpClientUtils.DeleteRequestAsync<T>(_clientFactory, _localStorage, functionApi, ids);
    }
  }
}
