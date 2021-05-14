using Microsoft.AspNetCore.Components;

namespace ACAG.Abacus.CalendarConnector.Client.Shared.ColumnDisplayContentComponent
{
  public partial class ColumnDisplayContentComponent
  {
    [Parameter]
    public string FieldValue { get; set; }
    [Parameter]
    public string ColumnWidth { get; set; }
  }
}
