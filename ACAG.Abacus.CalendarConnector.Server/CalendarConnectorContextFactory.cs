using ACAG.Abacus.CalendarConnector.DataAccess;
using Microsoft.Extensions.Options;

namespace ACAG.Abacus.CalendarConnector.Server
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
  }
}
