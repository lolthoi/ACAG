namespace ACAG.Abacus.CalendarConnector.Models
{
  public class ExchangeLoginTypeModel
  {
    public int Id { get; set; }
    public string Name { get; set; }

    public ExchangeLoginEnumType Type { get; set; }
  }

  public enum ExchangeLoginEnumType
  {
    WebLogin = 1,
    NetworkLogin = 2
  }
}
