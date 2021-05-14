using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
    public class TenantUserRel : AuditedEntity
    {
        [Key]
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int UserId { get; set; }

        public virtual Tenant Tenant { get; set; }
        public virtual User User { get; set; }
    }
}
