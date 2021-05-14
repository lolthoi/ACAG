using System.Threading.Tasks;

namespace ACAG.Abacus.CalendarConnector.Client.Pages
{
  public partial class Index
  {
    protected override async Task OnInitializedAsync()
    {
      var tenantPath = $"{_navigationManager.BaseUri}tenant";
      _navigationManager.NavigateTo(tenantPath, true);
    }
  }
}
