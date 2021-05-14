using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
    public class Tenant : AuditedEntity
    {

        public Tenant()
        {
            ExchangeSettings = new HashSet<ExchangeSetting>();
            PayTypes = new HashSet<PayType>();
            TenantUserRels = new HashSet<TenantUserRel>();
            LogDiarys = new HashSet<LogDiary>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public int? AbacusSettingId { get; set; }
        public int ScheduleTimer { get; set; }
        public bool IsEnabled { get; set; }

        public virtual AbacusSetting AbacusSetting { get; set; }
        public virtual ICollection<ExchangeSetting> ExchangeSettings { get; set; }
        public virtual ICollection<PayType> PayTypes { get; set; }
        public virtual ICollection<TenantUserRel> TenantUserRels { get; set; }
        public virtual ICollection<LogDiary> LogDiarys { get; set; }
    }
}
