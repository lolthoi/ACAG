using System.Collections.Generic;

namespace ACAG.Abacus.CalendarConnector.Client.Common
{
  public class AppDefiner
  {
    public static int UserLoggedInTimeoutBySeconds = 1 * 60 * 30; // 30 minutes
    public const string DefaultIdenityPwd = "123456";

    public static Dictionary<string, string> AppThemes = new Dictionary<string, string>();

    public const string MessageBadRequest = "Your request is bad!";
    public const string EmailValidationPattern = "^\\s*[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})\\s*$";

    public static class Roles
    {
      public const string ADMINISTRATOR = "Administrator";
      public const string USER = "User";
      public const string ADMINISTRATOR_USER = "Administrator, User";
      public const string ADMINISTRATOR_SYSADMIN = "Administrator, SysAdmin";
    }

    public const string ConnectionStrConfigName = "CalendarConnectorContext";
    public const string MailServiceConfigName = "MailSettings";
    public const string ResourcesConfigName = "Resources";
    public const string DefaultIdentityUserName = "DefaultIdentityUserName";
    public const string AbacusApiConfigName = "AbacusApiSettings";
  }
}
