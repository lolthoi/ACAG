using System.Security.Principal;

namespace ACAG.Abacus.CalendarConnector.Models.Abacus
{
  public partial class UserLogin : IIdentity
  {
    /// <summary>
    /// Username
    /// </summary>
    public string Name
    {
      get; set;
    }

    public string AuthenticationType { get { return Role == 1 ? "admin" : "user"; } }

    public bool IsAuthenticated
    {
      get { return true; }
    }
  }

  public partial class UserLogin
  {
    public int TenantId { get; set; }
    public int Role { get; set; }
  }
}