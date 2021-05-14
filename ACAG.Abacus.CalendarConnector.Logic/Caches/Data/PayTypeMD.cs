namespace ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data
{
    public class PayTypeMD
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int Code { get; set; }
        public string DisplayName { get; set; }
        public bool IsAppointmentPrivate { get; set; }
        public bool IsAppointmentAwayState { get; set; }
        public bool IsEnabled { get; set; }
    }
}
