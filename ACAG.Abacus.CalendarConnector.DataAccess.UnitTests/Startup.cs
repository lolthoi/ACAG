using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests
{
  public class Startup
  {

    public ServiceProvider ServiceProvider { get; private set; }

    #region Constructor
    private static Startup _instance;

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
