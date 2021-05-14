using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Pages.Components
{
  public partial class MenuUserComponent
  {
    [Parameter]
    public UserViewModel DataSourceUser { get; set; }

    [Parameter]
    public TenantModel EditContextUser { get; set; }

    public DxDataGrid<UserModel> GridUserModel { get; set; }

    string noTextToDisplay = "";
    string searchUser = "";

    protected override void OnInitialized()
    {
      noTextToDisplay = _localizer[LangKey.NO_DATA_TO_DISPLAY];
    }

    async Task SearchUserAsync()
    {
      var result = await _apiWrapper.Users.GetTenantUsers(EditContextUser.Id, searchUser);
      if (result.IsSuccess && result.Error == null && result.Data != null)
      {
        DataSourceUser = result.Data;
      }

      if (DataSourceUser.Users == null || DataSourceUser.Users.Count == 0)
      {
        await _jSRuntime.InvokeVoidAsync("changeNoDataText", "customDataGrid", noTextToDisplay, 110);
      }
    }

    protected override void OnAfterRender(bool firstRender)
    {
      _jSRuntime.InvokeVoidAsync("afterMenuUserComponentRender", "customMenuUserGrid", noTextToDisplay);
    }
  }
}
