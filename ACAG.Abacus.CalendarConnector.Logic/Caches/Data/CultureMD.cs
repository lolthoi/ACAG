using System.Collections.Generic;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data
{
  public partial class CultureMD
  {
    public int Id { get; set; }
    public string Code { get; set; }
    public string DisplayName { get; set; }

    public bool IsEnabled { get; set; }

  }

  public partial class CultureMD
  {
    private Dictionary<string, string> _source = new Dictionary<string, string>();

    public string Get(string key)
    {
      if (_source.TryGetValue(key, out string result))
        return result;

      return key;
    }

    public void InitSource(Dictionary<string, string> source)
    {
      _source = source;
    }
  }
}
