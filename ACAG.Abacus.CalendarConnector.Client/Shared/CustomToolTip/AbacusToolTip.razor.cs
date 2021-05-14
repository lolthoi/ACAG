using Microsoft.AspNetCore.Components;

namespace ACAG.Abacus.CalendarConnector.Client.Shared.CustomToolTip
{
  public partial class AbacusToolTip
  {
    [Parameter]
    public RenderFragment Control { get; set; }

    [Parameter]
    public RenderFragment ErrorMessage { get; set; }

    protected override void OnParametersSet()
    {
      base.OnParametersSet();
      StateHasChanged();
    }

    protected override void OnInitialized()
    {
      //_jSRuntime.InvokeAsync<string>("enableToolTip");
    }
  }
}
