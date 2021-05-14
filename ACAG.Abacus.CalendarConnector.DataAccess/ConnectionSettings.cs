using System.Collections.Generic;

namespace ACAG.Abacus.CalendarConnector.DataAccess
{
    public class ConnectionSettings
    {
        public bool UseConnectionStrings { get; set; }

        public Dictionary<string, string> ConnectionStrings { get; set; }
    }
}
