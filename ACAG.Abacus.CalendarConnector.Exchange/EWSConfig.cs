using Microsoft.Exchange.WebServices.Data;

namespace ACAG.Abacus.CalendarConnector.Exchange
{
  public class EWSConfig
  {
    public const string EXTEND_SUBCRIPTION_ID = "CustomItemId2020";
    public const string EXTEND_SUBCRIPTION_VALUE = "CustomItemData2020";

    private static PropertyDefinitionBase[] _selectedProperties => new PropertyDefinitionBase[]
    {
      AppointmentSchema.LegacyFreeBusyStatus,
      AppointmentSchema.Subject,
      AppointmentSchema.Start,
      AppointmentSchema.End,
      AppointmentSchema.Body,
      AppointmentSchema.Id,
      ItemSchema.Sensitivity,
      new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, EXTEND_SUBCRIPTION_ID, MapiPropertyType.String),
      new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, EXTEND_SUBCRIPTION_VALUE, MapiPropertyType.String),
    };

    public static PropertyDefinitionBase[] SelectedProperties => _selectedProperties;
  }
}
