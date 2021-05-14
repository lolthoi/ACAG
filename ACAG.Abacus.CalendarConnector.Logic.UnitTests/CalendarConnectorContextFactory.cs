using ACAG.Abacus.CalendarConnector.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests
{
  public class CalendarConnectorContextFactory : ContextFactory
  {

    public CalendarConnectorContextFactory(
        IOptions<ConnectionSettings> options)
        : base(options)
    {
    }

    protected override string GetConnectionString()
    {
      return _connectionSettings.ConnectionStrings["CalendarConnectorContext"];
    }

    protected override DbContextOptions BuildDbContextOption<TContext>()
    {
      var builder = new DbContextOptionsBuilder<TContext>();
      builder.UseInMemoryDatabase("LogicTest");
      var options = builder.Options;
      return options;
    }
  }
}
