using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
  public class User : AuditedEntity
  {
    public User()
    {
      this.AppRoleRels = new HashSet<AppRoleRel>();
      this.TenantUserRels = new HashSet<TenantUserRel>();
    }

    [Key]
    public int Id { get; set; }
    public int CultureId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Comment { get; set; }
    public string SaltPassword { get; set; }
    public string ResetCode { get; set; }
    public bool IsEnabled { get; set; }

    public DateTime? ExpireTime { get; set; }
    public virtual Culture Culture { get; set; }

    public virtual ICollection<AppRoleRel> AppRoleRels { get; set; }

    public virtual ICollection<TenantUserRel> TenantUserRels { get; set; }
  }
}
