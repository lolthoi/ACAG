using System;

namespace ACAG.Abacus.CalendarConnector.Wrappers
{
  public class HeaderParam
  {
    public string Language { get; set; }

    public string Token { get; set; }

    public Guid? TenantId { get; set; }

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
}
