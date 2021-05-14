using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Client.Services;
using ACAG.Abacus.CalendarConnector.Models;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Pages.Components
{
  public partial class PayType
  {
    #region Variables

    [Parameter]
    public int TenantId { get; set; }

    DxDataGrid<PayTypeModel> grid;
    List<PayTypeModel> dataSource = null;
    PayTypeModel _editContext = null;

    bool isVisiblePopupConfirmDeleteRow = false;

    string _messages, _searchText = "";
    string noTextToDisplay = "";
    string _titleDelete = "";

    [Inject] public HttpInterceptorService Interceptor { get; set; }

    #endregion Variables

    #region INVOKE

    protected override async Task OnInitializedAsync()
    {
      Interceptor.RegisterEvent();
      noTextToDisplay = _localizer[LangKey.NO_DATA_TO_DISPLAY];

      var result = await _apiWrapper.PayTypes.GetAllAsync(TenantId, _searchText);
      if (result.IsSuccess && result.Error == null)
      {
        dataSource = result.Data.PayTypeModels;
      }
    }

    #endregion INVOKE

    #region PAYTYPE CREATE, UPDATE, DELETE

    private void OnAddNewRow()
    {
      _editContext = new PayTypeModel();
      _editContext.TenantId = TenantId;
      _editContext.IsEnabled = true;
      _messages = "";
      grid.CancelRowEdit();
      grid.StartRowEdit(null);
    }

    private async Task OnEditRowAsync(PayTypeModel item)
    {
      var result = await _apiWrapper.PayTypes.GetByIdAsync(TenantId, item.Id);

      if (result.IsSuccess && result.Error == null)
      {
        _editContext = result.Data;
        _editContext.TenantId = TenantId;
        _messages = "";
        _titleDelete = string.Format("{0}, {1}", _editContext.Code, _editContext.DisplayName);
        await InvokeAsync(StateHasChanged);
      }
    }

    private async Task DeleteRowAsync()
    {
      var result = await _apiWrapper.PayTypes.DeleteAsync(TenantId, _editContext.Id);

      if (result.IsSuccess && result.Data)
      {
        _editContext = null;
        await grid.CancelRowEdit();
        _toastAppService.ShowSuccess(_localizer[LangKey.PAYTYPE_HAS_BEEN_DELETED_SUCCESSFULLY]);
      }
      else
      {
        _toastAppService.ShowWarning(_localizer[LangKey.FAIL_TO_DELETE_PAYTYPE]);
      }

      OnHidePopupDelete();
      await SearchAsync();
    }

    private async Task HandleValidSubmit()
    {
      _messages = "";

      var editModel = new PayTypeEditModel()
      {
        Code = _editContext.Code,
        DisplayName = _editContext.DisplayName,
        IsAppointmentPrivate = _editContext.IsAppointmentPrivate,
        IsAppointmentAwayState = _editContext.IsAppointmentAwayState,
        IsEnabled = _editContext.IsEnabled
      };

      if (_editContext.Id == 0)
      {
        var result = await _apiWrapper.PayTypes.CreateAsync(TenantId, editModel);

        if (result.IsSuccess && result.Data != null)
        {
          await grid.CancelRowEdit();
          _editContext = null;
          await SearchAsync();
          _toastAppService.ShowSuccess(_localizer[LangKey.PAYTYPE_HAS_BEEN_ADDED_SUCCESSFULLY]);
        }
        else
        {
          _messages = @_localizer[result.Error.Message];
          _toastAppService.ShowWarning(_messages);
        }
      }
      else
      {
        var result = await _apiWrapper.PayTypes.UpdateAsync(TenantId, _editContext.Id, editModel);

        if (result.IsSuccess && result.Data != null)
        {
          await grid.CancelRowEdit();
          _editContext = null;
          await SearchAsync();
          _toastAppService.ShowSuccess(_localizer[LangKey.PAYTYPE_HAS_BEEN_UPDATED_SUCCESSFULLY]);
        }
        else
        {
          _messages = @_localizer[result.Error.Message];
          _toastAppService.ShowWarning(_messages);
        }
      }
    }

    #endregion PAYTYPE CREATE, UPDATE, DELETE

    #region POPUP CONFIRM DELETE

    private void OnHidePopupDelete()
    {
      isVisiblePopupConfirmDeleteRow = false;
    }

    private void OnShowPopupDelete()
    {
      isVisiblePopupConfirmDeleteRow = true;
    }
    #endregion POPUP CONFIRM DELETE

    #region BUTTON HANDLE

    private void OnCancelButtonClick()
    {
      _editContext = null;
      grid.CancelRowEdit();
    }

    #endregion BUTTON HANDLE

    #region SEARCH

    async Task SearchAsync()
    {
      var result = await _apiWrapper.PayTypes.GetAllAsync(TenantId, _searchText);

      if (result.IsSuccess && result.Error == null)
      {
        dataSource = result.Data.PayTypeModels;
        await InvokeAsync(StateHasChanged);
      }

      if (dataSource == null || dataSource == null || dataSource.Count == 0)
      {
        await _jSRuntime.InvokeVoidAsync("changeNoDataText", "customPayTypeGrid", noTextToDisplay, 110);
      }
    }

    #endregion SEARCH

    protected override void OnAfterRender(bool firstRender)
    {
      _jSRuntime.InvokeVoidAsync("afterPayTypeComponentRender", "customPayTypeGrid", noTextToDisplay);
    }
  }
}
