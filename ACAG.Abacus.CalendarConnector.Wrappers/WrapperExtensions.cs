using System;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ACAG.Abacus.CalendarConnector.Wrappers
{
  public static class WrapperExtensions
  {
    public static WebAssemblyHostBuilder AddWrapper(this WebAssemblyHostBuilder builder)
    {
      WrapperApiInfo wrapperApiInfo = new WrapperApiInfo()
      {
        UrlClientName = builder.Configuration["UrlClientName"],
        Url = builder.Configuration["Url"],
        APITimeout = builder.Configuration["APITimeout"],
      };

      builder.Services.AddHttpClient(wrapperApiInfo.UrlClientName, c =>
      {
#if DEBUG
        c.BaseAddress = new Uri(wrapperApiInfo.Url);
#else
        c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "server/");
#endif
        c.Timeout.Add(new TimeSpan(0, 0, int.Parse(wrapperApiInfo.APITimeout)));
        c.DefaultRequestHeaders.Accept.Clear();
      });
      builder.Services.AddScoped<IApiWrapper, ApiWrapper>();
      builder.Services.AddSingleton<Wrappers.HeaderParam.IHeaderAPI, Wrappers.HeaderParam.HeaderAPI>();

      return builder;
    }
  }
}
