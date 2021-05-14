using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.WebApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ACAG.Abacus.CalendarConnector.WebApp.UnitTests
{
  public class Startup
  {
    private static Startup _instance;

    public ServiceProvider ServiceProvider { get; private set; }

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
      IConfiguration configuration = InitConfiguration();
      IServiceCollection services = new ServiceCollection();
      services.ReadConnectionSettingConfig(configuration, "CalendarConnectorContext");

      services.AddCustomizedDbContextFactory<CalendarConnectorContextFactory>();

      services
         .AddSingleton<CacheProvider>()
         .AddScoped<ITenantService>()
         .AddScoped<IUserService>()
         .AddScoped<IRoleService, RoleService>()
         .AddScoped<IAuthenticationService, AuthenticationService>()
         .AddScoped<ILanguageService, LanguageService>()
         .AddScoped<IMailService, MailService>()
         .AddSingleton<ILoggerFactory, LoggerFactory>()
         ;

      ServiceProvider = services.BuildServiceProvider();
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
