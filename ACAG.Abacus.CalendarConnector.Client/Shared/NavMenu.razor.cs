using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Shared
{
  public partial class NavMenu
  {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        await _jSRuntime.InvokeVoidAsync("enableNavMenu", false, 1000);
      }
      else
      {
        await _jSRuntime.InvokeVoidAsync("enableNavMenu", true, 0);
      }
    }
  }
}
