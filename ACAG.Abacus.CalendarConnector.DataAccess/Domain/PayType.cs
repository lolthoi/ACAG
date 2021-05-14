using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
    public class PayType : AuditedEntity
    {
        [Key]
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int Code { get; set; }
        public string DisplayName { get; set; }

        /// <summary>
        /// if true => appointment in exchange is marked as private
        /// </summary>
        public bool IsAppointmentPrivate { get; set; }

        /// <summary>
        /// if true => appointment in exchange is marked as away; default is false
        /// </summary>
        public bool IsAppointmentAwayState { get; set; }
        public bool IsEnabled { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}
