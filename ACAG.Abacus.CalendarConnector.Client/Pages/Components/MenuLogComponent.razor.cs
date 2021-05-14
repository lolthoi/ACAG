using System.Collections.Generic;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Pages.Components
{
  public partial class MenuLogComponent
  {
    [Parameter]
    public TenantModel EditContextLog { get; set; }

    [Parameter]
    public List<LogDiaryModel> LogDiaryModels { get; set; }

    [Parameter]
    public EventCallback<DataGridSelection<LogDiaryModel>> SelectedLogDiaryChanged { get; set; }

    public DxDataGrid<LogDiaryModel> grid;

    public string noTextToDisplay { get; set; }

    protected override void OnInitialized()
    {
      noTextToDisplay = _localizer[LangKey.NO_DATA_TO_DISPLAY];
    }

    protected async Task OnSelectionChanged(DataGridSelection<LogDiaryModel> selection)
    {
      await SelectedLogDiaryChanged.InvokeAsync(selection);
    }

    protected override void OnAfterRender(bool firstRender)
    {
      _jSRuntime.InvokeVoidAsync("afterMenuLogComponentRender", "customMenuLogGrid", noTextToDisplay);
    }
  }
}
