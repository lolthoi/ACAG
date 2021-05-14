using ACAG.Abacus.CalendarConnector.Models;

namespace ACAG.Abacus.CalendarConnector.Server
{
  public static class ResultExtensions
  {
    public static T ToResponse<T>(this ResultModel<T> result)
    {
      if (result.Error == null)
        return result.Data;

      throw new LogicException(result.Error);
    }
  }
}
