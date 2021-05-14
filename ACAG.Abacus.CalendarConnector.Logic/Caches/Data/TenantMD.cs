namespace ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data
{
    public class TenantMD
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Number { get; set; }
        public int? AbacusSettingId { get; set; }
        public int ScheduleTimer { get; set; }
        public bool IsEnabled { get; set; }
    }
}
