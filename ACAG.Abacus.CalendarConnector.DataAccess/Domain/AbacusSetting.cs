using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
    public class AbacusSetting : AuditedEntity
    {
        public AbacusSetting()
        {
            Tenants = new HashSet<Tenant>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ServiceUrl { get; set; }
        public int ServicePort { get; set; }
        public bool ServiceUseSSL { get; set; }
        public string ServiceUser { get; set; }
        public string ServiceUserPassword { get; set; }
        public bool? HealthStatus { get; set; }

        public virtual ICollection<Tenant> Tenants { get; set; }
    }
}
