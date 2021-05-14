using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ACAG.Abacus.CalendarConnector.Abacus.UnitTests
{
  public abstract class TestBase
  {
    protected WrapperAPI WrapperAPI { get; private set; }

    public TestBase()
    {
      var startup = Startup.Instance;
      WrapperAPI = startup.ServiceProvider.GetService<WrapperAPI>();
    }

  }

  public interface IHeaderAPI
  {
    HeaderParam GetHeaderParam();
  }
  public class HeaderAPI : IHeaderAPI
  {
    public HeaderParam GetHeaderParam()
    {
      return new HeaderParam
      {
        Language = "en",
        Token = "nam_test"
      };
    }
  }
}
