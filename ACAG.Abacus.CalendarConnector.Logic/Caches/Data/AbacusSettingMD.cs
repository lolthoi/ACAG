namespace ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data
{
    public class AbacusSettingMD
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ServiceUrl { get; set; }
        public int ServicePort { get; set; }
        public bool ServiceUseSSL { get; set; }
        public string ServiceUser { get; set; }
        public string ServiceUserPassword { get; set; }
        public bool? HealthStatus { get; set; }
    }
}
