using System;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Extensions
{
    public interface IAuditedEntity
    {
        int CreatedBy { get; set; }

        DateTime CreatedDate { get; set; }

        int? ModifiedBy { get; set; }

        DateTime? ModifiedDate { get; set; }
    }

    public class AuditedEntity : IAuditedEntity
    {
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
