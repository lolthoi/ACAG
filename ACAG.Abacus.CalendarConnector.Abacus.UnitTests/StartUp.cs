using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Net;
using System.Net.Http.Headers;

namespace ACAG.Abacus.CalendarConnector.Abacus.UnitTests
{
  public class Startup
  {

    public ServiceProvider ServiceProvider { get; private set; }

    #region Constructor
    private static readonly Startup _instance;

    public static Startup Instance
    {
      get { return _instance ?? new Startup(); }
    }

    private Startup()
    {
      IConfiguration configuration = InitConfiguration();
      IServiceCollection services = new ServiceCollection();

      services.Configure<AppSettings>(options =>
      {
        configuration.GetSection(nameof(AppSettings)).Bind(options);
      });

      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddSingleton<IHeaderAPI, HeaderAPI>();
      services.AddSingleton<WrapperAPI>();

      var appSettings = new AppSettings();
      configuration.GetSection(nameof(AppSettings)).Bind(appSettings);

      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      services.AddHttpClient(appSettings.UrlClientName, c =>
      {
        c.BaseAddress = new Uri(appSettings.Url);
        c.DefaultRequestHeaders.Accept.Clear();
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      });

      ServiceProvider = services.BuildServiceProvider();
    }

    #endregion

    private IConfiguration InitConfiguration()
    {
      var config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .Build();
      return config;
    }
  }
}
