using System.Collections.Generic;

namespace ACAG.Abacus.CalendarConnector.Logic
{
  internal class GlobalSetting
  {
    internal static class Exchange
    {
      public static Dictionary<int, string> Versions = new Dictionary<int, string>() {
        { 7, "Office365" },
        { 9, "Exchange2016" }
      };

      public static Dictionary<int, string> LoginTypes = new Dictionary<int, string>() {
        { 1, "WebLogin" },
        { 2, "NetworkLogin" }
      };
    }
  }
}
