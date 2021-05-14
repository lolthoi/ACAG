namespace ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0
{
  public class TenantModel
  {
    #region Properies
    public string Name { get; set; }
    public string Description { get; set; }
    public int Number { get; set; }
    public AbacusSettingModel AbacusSetting { get; set; }
    #endregion

    #region Constructors
    public TenantModel(string name, string description, int number, AbacusSettingModel abacusSetting)
    {
      Name = name;
      Description = description;
      Number = number;
      AbacusSetting = abacusSetting;
    }
    #endregion
  }
}
