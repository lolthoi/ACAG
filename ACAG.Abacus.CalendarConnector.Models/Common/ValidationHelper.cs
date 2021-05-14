using System.Text.RegularExpressions;

namespace ACAG.Abacus.CalendarConnector.Models
{
  public static class ValidationHelper
  {
    public static bool IsValidEmail(string email)
    {
      if (string.IsNullOrEmpty(email)) return false;
      string validEmail = "^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";
      bool isEmail = Regex.IsMatch(email, validEmail, RegexOptions.IgnoreCase);
      return isEmail;
    }

    public static bool IsValidPasswordLength(string password)
    {
      return password.Length >= 6 && password.Length <= 100;
    }

    public static bool IsValidPasswordHasNumber(string password)
    {
      return Regex.IsMatch(password, @"/\d+/", RegexOptions.ECMAScript);
    }

    public static bool IsValidPasswordHasBothUpperAndLower(string password)
    {
      return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$", RegexOptions.ECMAScript);
    }

    public static bool IsValidPasswordHasSpecials(string password)
    {
      return Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript);
    }
  }
}
