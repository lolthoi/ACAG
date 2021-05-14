using System;
using System.Globalization;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Client.Common;
using ACAG.Abacus.CalendarConnector.Client.Services;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Wrappers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace ACAG.Abacus.CalendarConnector.Client
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebAssemblyHostBuilder.CreateDefault(args);

      builder.RootComponents.Add<App>("#app");
      builder.Services.AddDevExpressBlazor();
      
      builder.Services.AddLocalization(options => options.ResourcesPath = AppDefiner.ResourcesConfigName);
      
      builder.Services.AddBlazoredLocalStorage();
      builder.Services.AddHttpClientInterceptor();

      WrapperApiInfo wrapperApiInfo = new WrapperApiInfo()
      {
        UrlClientName = builder.Configuration["UrlClientName"],
        Url = builder.Configuration["Url"],
        APITimeout = builder.Configuration["APITimeout"],
      };

      builder.Services.AddHttpClient(wrapperApiInfo.UrlClientName, (sp, c) =>
      {
#if DEBUG
        c.BaseAddress = new Uri(wrapperApiInfo.Url);
#else
        c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress.Replace("client", "server"));
#endif
        c.EnableIntercept(sp);
        c.Timeout.Add(new TimeSpan(0, 0, 30));
        c.DefaultRequestHeaders.Accept.Clear();
      });
      builder.Services.AddScoped<IApiWrapper, ApiWrapper>();
      builder.Services.AddSingleton<Wrappers.HeaderParam.IHeaderAPI, Wrappers.HeaderParam.HeaderAPI>();

      builder.Services.AddScoped<HttpInterceptorService>();

      builder.Services.AddScoped<ThemeAppService>();
      builder.Services.AddScoped<IToastAppService, ToastAppService>();
      builder.Services.AddScoped<IAuthService, AuthService>();

      builder.Services.AddAuthorizationCore();
      builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
      builder.Services.AddAuthorizationCore(config =>
      {
        config.AddPolicy("SysAdmin", new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                             .RequireRole("SysAdmin")
                                             .Build());
        config.AddPolicy("Administrator", new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                             .RequireRole("Admin")
                                             .Build());
        config.AddPolicy("User", new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                             .RequireRole("User")
                                             .Build());
      });

      var host = builder.Build();

      await host.SetDefaultCulture();
      await host.RunAsync();
    }
  }

  public static class WebAssemblyHostExtension
  {
    public async static Task SetDefaultCulture(this WebAssemblyHost host)
    {
      var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
      var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
      CultureInfo culture;
      if (result != null)
        culture = new CultureInfo(result);
      else
        culture = new CultureInfo("en-US");
      CultureInfo.DefaultThreadCurrentCulture = culture;
      CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
  }

}
