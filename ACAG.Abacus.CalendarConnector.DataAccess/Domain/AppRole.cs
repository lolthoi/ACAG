using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
    public class AppRole : AuditedEntity
    {
        public AppRole()
        {
            this.AppRoleRels = new HashSet<AppRoleRel>();
        }

        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsEnabled { get; set; }

        public virtual ICollection<AppRoleRel> AppRoleRels { get; set; }
    }
}
