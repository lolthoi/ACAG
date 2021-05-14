using Microsoft.Extensions.DependencyInjection;


namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests
{
  public abstract class TestBase
  {
    protected IUnitOfWork<CalendarConnectorContext> _dbContext;

    public TestBase()
    {
      var startup = Startup.Instance;
      CalendarConnectorDataInitializer.Initialize(startup.ServiceProvider);

      var scope = startup.ServiceProvider.CreateScope();
      _dbContext = scope.ServiceProvider.GetService<IUnitOfWork<CalendarConnectorContext>>();

    }
  }
}
