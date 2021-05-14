using System.Collections.Generic;

namespace ACAG.Abacus.CalendarConnector.Models
{
    public class LanguageModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
    }

    public class LanguageData
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
