using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
    public class AppRoleRel : AuditedEntity
    {
        [Key]
        public int Id { get; set; }
        public int AppRoleId { get; set; }
        public int UserId { get; set; }

        public virtual AppRole AppRole { get; set; }
        public virtual User User { get; set; }
    }
}
