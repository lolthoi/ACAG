using System;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data
{
  public partial class AppRoleMD
  {
    public int Id { get; set; }

    public string Code { get; set; }

    public bool IsAdministrator { get; set; }
  }

  public partial class AppRoleMD
  {
    private Roles? _role;
    public Roles Role
    {
      get
      {
        if (_role == null)
        {
          if (Enum.TryParse(Code.ToUpper(), out Roles role))
            _role = role;
        }

        return _role.Value;
      }
    }
  }
}
