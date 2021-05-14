using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAG.Abacus.CalendarConnector.Abacus.UnitTests
{
  public class AppSettings
  {
    public string Url { get; set; }

    public string UrlClientName { get; set; }

    public string SubUrl { get; set; }

    private int apiTimeout;
    public int APITimeout
    {
      get
      {
        if (apiTimeout == 0)
        {
            apiTimeout = 30; // default timeout value
        }
        return apiTimeout;
      }
      set { apiTimeout = value; }
    }
  }
}
