namespace ACAG.Abacus.CalendarConnector.Models.Authentication
{
  public class LoginResult
  {
    public bool Successful { get; set; }
    public string Error { get; set; }
    public string Token { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? CultureId { get; set; }
    public string FullName
    {
      get
      {
        return FirstName + " " + LastName;
      }
    }
  }
}
