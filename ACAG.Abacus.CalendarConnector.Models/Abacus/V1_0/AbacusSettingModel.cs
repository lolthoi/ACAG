namespace ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0
{
  public class AbacusSettingModel
  {
    #region Properties
    public int ID { get; set; }
    public int TenantID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ServiceServerName { get; set; }
    public int ServicePort { get; set; }
    public bool UseSsl { get; set; }
    public string ServiceUser { get; set; }
    public string ServicePasswordCrypted { get; set; }
    #endregion

    #region Constructors
    public AbacusSettingModel(
      string name,
      string description,
      string serviceServerName,
      int servicePort,
      bool useSsl,
      string serviceUser,
      string servicePasswordCrypted)
    {
      Name = name;
      Description = description;
      ServiceServerName = serviceServerName;
      ServicePort = servicePort;
      UseSsl = useSsl;
      ServiceUser = serviceUser;
      ServicePasswordCrypted = servicePasswordCrypted;
    }
    #endregion
  }
}
