namespace ACAG.Abacus.CalendarConnector.Logic
{
  public class AbacusApiSettings
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
          apiTimeout = 30;
        }
        return apiTimeout;
      }
      set { apiTimeout = value; }
    }
  }
}
