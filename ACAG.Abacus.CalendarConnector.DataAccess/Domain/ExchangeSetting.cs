using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
    public class ExchangeSetting : AuditedEntity
    {
        [Key]
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string ExchangeVersion { get; set; }
        public string ExchangeUrl { get; set; }
        public int LoginType { get; set; }
        public string AzureTenant { get; set; }
        public string AzureClientId { get; set; }
        public string AzureClientSecret { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public string ServiceUser { get; set; }
        public string ServiceUserPassword { get; set; }
        public bool? HealthStatus { get; set; }
        public bool IsEnabled { get; set; }

        public virtual Tenant Tenant { get; set; }
    }
}
