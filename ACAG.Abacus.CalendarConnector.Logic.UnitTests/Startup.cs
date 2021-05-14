using System;
using System.Net.Http.Headers;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using ACAG.Abacus.CalendarConnector.Logic.Common;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services;
using ACAG.Abacus.CalendarConnector.Scheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static ACAG.Abacus.CalendarConnector.Logic.Services.HeaderParam;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests
{
  public partial class Startup
  {
    private static Startup _instance;

    public ServiceProvider ServiceProvider { get; private set; }

    public IOptions<AccountSetting> Options { get; private set; }

    public static Startup Instance
    {
      get
      {
        if (_instance == null)
        {
          _instance = new Startup();
        }
        return _instance;
      }
    }

    private Startup()
    {
      var configuration = InitConfiguration();
      IServiceCollection services = new ServiceCollection();
      services.ReadConnectionSettingConfig(configuration, "CalendarConnectorContext");

      services.Configure<AccountSetting>(options =>
      {
        configuration.GetSection("AccountSettings").Bind(options);
      });

      services.Configure<AbacusApiSettings>(options =>
      {
        configuration.GetSection("AbacusApiSettings").Bind(options);
      });

      services.AddCustomizedDbContextFactory<CalendarConnectorContextFactory>();

      services
        .AddCustomizedService<AuthenTestLogin>()
        .AddScoped<ITestService, TestService>()
        .AddScoped<ICultureService, CultureService>()
        .AddScoped<IRoleService, RoleService>()
        .AddScoped<IAuthenticationService, AuthenticationService>()
        .AddScoped<IUserService, UserService>()
        .AddScoped<ITenantService, TenantService>()
        .AddScoped<IPayTypeService, PayTypeService>()
        .AddScoped<IAbacusSettingService, AbacusSettingService>()
        .AddScoped<IExchangeSettingService, ExchangeSettingService>()
        .AddScoped<ILogDiaryService, LogDiaryService>()
        .AddScoped<IScheduler, AbacusProvider>()
        .AddScoped<IAuthenticationService, AuthenticationService>()
        .AddSingleton<ILoggerFactory, LoggerFactory>()
        .AddSingleton<IHeaderAPI, HeaderAPI>()
        .AddSingleton<WrapperAPI>();
        ;

      var abacusSettings = configuration.GetSection(nameof(AbacusApiSettings)).Get<AbacusApiSettings>() ?? new AbacusApiSettings();
      services.AddHttpClient(abacusSettings.UrlClientName, c =>
      {
        c.BaseAddress = new Uri(abacusSettings.Url);
        c.DefaultRequestHeaders.Accept.Clear();
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
      });

      ServiceProvider = services.BuildServiceProvider();

      Options = ServiceProvider.GetService<IOptions<AccountSetting>>();

      CalendarConnectorDataInitializer.Initialize(ServiceProvider);

    }

    private IConfiguration InitConfiguration()
    {
      var config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .Build();
      return config;
    }
  }
}
