namespace ACAG.Abacus.CalendarConnector.Models
{
  public class AbacusSettingConnectionModel
  {
    #region AbacusSetting

    public string AbacusSettingName { get; set; }
    public string AbacusSettingDescription { get; set; }
    public string AbacusSettingServiceUrl { get; set; }
    public int AbacusSettingServicePort { get; set; }
    public bool AbacusSettingServiceUseSsl { get; set; }
    public string AbacusSettingServiceUser { get; set; }
    public string AbacusSettingServiceUserPassword { get; set; }

    #endregion

    #region Tenant

    public string TenantName { get; set; }
    public string TenantDescription { get; set; }
    public int TenantNumber { get; set; }

    #endregion
  }
}
