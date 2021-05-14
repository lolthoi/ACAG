using System.Threading.Tasks;
using Blazored.LocalStorage;
using Toolbelt.Blazor;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Services
{
  public class HttpInterceptorService
  {
    private readonly HttpClientInterceptor _interceptor;
    private readonly IAuthService _authService;
    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _jsRuntime;

    public HttpInterceptorService(HttpClientInterceptor interceptor, IAuthService authService, ILocalStorageService localStorage, IJSRuntime jsRuntime)
    {
      _interceptor = interceptor;
      _authService = authService;
      _localStorage = localStorage;
      _jsRuntime = jsRuntime;
    }

    public void RegisterEvent() => _interceptor.AfterSendAsync += InterceptAfterHttpAsync;

    public async Task InterceptAfterHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
      if(e.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
      {
        await _authService.Logout();
      }
      else if(e.Response.StatusCode == System.Net.HttpStatusCode.ExpectationFailed)
      {
        await _authService.ClearLocalStorage();
        await _jsRuntime.InvokeVoidAsync("showModalUnauthorized");
      }
    }

    public void DisposeEvent() => _interceptor.AfterSendAsync -= InterceptAfterHttpAsync;
  }
}
