namespace ACAG.Abacus.CalendarConnector.Logic.UnitTests.Services
{
  public class AccountSetting
  {

    public Account Admin { get; set; }

    public Account User { get; set; }

    public Account SysAdmin { get; set; }

    public class Account
    {
      public string UserName { get; set; }
      public string Password { get; set; }
    }
  }
}
