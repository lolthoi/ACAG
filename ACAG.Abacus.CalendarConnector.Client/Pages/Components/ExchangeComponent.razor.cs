using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Client.Services;
using ACAG.Abacus.CalendarConnector.Models;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Pages.Components
{
  public partial class ExchangeComponent
  {
    [Parameter]
    public int TenantId { get; set; }

    [Parameter]
    public EventCallback<int> ExchangeSettingsChanged { get; set; }

    private string _search = "";
    string _messages = "";
    ExchangeSettingViewModel _dataSource = null;
    DxDataGrid<ExchangeSettingModel> _grid;
    bool _PopupVisibleDeleteExchangeSetting = false;
    private List<ExchangeVersionModel> _exchangeVersionModels = null;
    private List<ExchangeLoginTypeModel> _exchangeLoginTypeModels = null;
    string _heathStatusexchangeSetting = "";
    string noTextToDisplay = "";
    public ExchangeSettingModel _editContext { get; set; } = null;
    string _titleDelete = "";
    string messageTestConnectionError = "";
    private EditContext EditContext;

    [Inject] public HttpInterceptorService Interceptor { get; set; }

    #region password
    bool _isPassShowed = true;
    bool _showPassword
    {
      get
      {
        return _isPassShowed;
      }
      set
      {
        _isPassShowed = value;
        _jSRuntime.InvokeVoidAsync("changePasswordVisibility", "PasswordTextBox", _isPassShowed);
      }
    }
    string _typePassword
    {
      get
      {
        return _showPassword == true ? "password" : "text";
      }
    }
    #endregion

    protected override async Task OnInitializedAsync()
    {
      Interceptor.RegisterEvent();
      noTextToDisplay = _localizer[LangKey.NO_DATA_TO_DISPLAY];

      var result = await _apiWrapper.ExchangeSettings.GetAllAsync(TenantId, _search);
      if (result.IsSuccess && result.Error == null)
      {
        _dataSource = result.Data;
      }
    }

    async Task SearchAsync()
    {
      var result = await _apiWrapper.ExchangeSettings.GetAllAsync(TenantId, _search);
      if (result.IsSuccess && result.Error == null)
      {
        _dataSource = result.Data;
      }

      await _grid.Refresh();

      if (_dataSource == null || _dataSource.ExchangeSettings == null || _dataSource.ExchangeSettings.Count == 0)
      {
        await _jSRuntime.InvokeVoidAsync("changeNoDataText", "customExchangeGrid", noTextToDisplay, 110);
      }
    }

    async Task AddNewGetDetailRowRender()
    {
      //This is lameshit right here. This will fix duplicate validate message of editcontext for now. Delete this if wasm updated and fix this bug.
      await _jSRuntime.InvokeVoidAsync("deleteAnotationErrorMessage");

      _editContext = new ExchangeSettingModel();
      await InvokeAsync(StateHasChanged);
      _editContext.TenantId = TenantId;
      _editContext.IsEnabled = true;
      _heathStatusexchangeSetting = _editContext.HealthStatus != null ? (_editContext.HealthStatus == true ? _localizer[LangKey.TEST_PASS] : _localizer[LangKey.TEST_FAIL]) : "";
      EditContext = new EditContext(_editContext);
      var apiVersionsResult = await _apiWrapper.ExchangeSettings.GetAllExchangeVersionsAsync();
      if (apiVersionsResult.IsSuccess && apiVersionsResult.Error == null)
      {
        _exchangeVersionModels = apiVersionsResult.Data;
      }

      var apiLoginTypesResult = await _apiWrapper.ExchangeSettings.GetAllLoginTypesAsync();
      if (apiLoginTypesResult.IsSuccess && apiLoginTypesResult.Error == null)
      {
        _exchangeLoginTypeModels = apiLoginTypesResult.Data;
      }

      await _grid.CancelRowEdit();
      _messages = "";
      await _grid.StartRowEdit(null);
      await _jSRuntime.InvokeAsync<string>("focusEditor", "first-focus");
      _isPassShowed = true;
      messageTestConnectionError = "";

    }

    async Task OnEditRowExchangeSetting(ExchangeSettingModel model)
    {
      var apiVersionsResult = await _apiWrapper.ExchangeSettings.GetAllExchangeVersionsAsync();
      if (apiVersionsResult.IsSuccess && apiVersionsResult.Error == null)
      {
        _exchangeVersionModels = apiVersionsResult.Data;
      }

      var apiLoginTypesResult = await _apiWrapper.ExchangeSettings.GetAllLoginTypesAsync();
      if (apiLoginTypesResult.IsSuccess && apiLoginTypesResult.Error == null)
      {
        _exchangeLoginTypeModels = apiLoginTypesResult.Data;
      }

      var apiExchangeResult = await _apiWrapper.ExchangeSettings.GetByIdAsync(TenantId, model.Id);

      if (apiExchangeResult.IsSuccess && apiExchangeResult.Error == null && apiExchangeResult.Data != null)
      {
        _titleDelete = apiExchangeResult.Data.Name;
        _editContext = apiExchangeResult.Data;
        _editContext.ExchangeLoginTypeModel = _exchangeLoginTypeModels.Where(x => x.Id == apiExchangeResult.Data.LoginType).FirstOrDefault();
        _editContext.ExchangeVersionModel = _exchangeVersionModels.Where(x => x.Name == apiExchangeResult.Data.ExchangeVersion).FirstOrDefault();
        _heathStatusexchangeSetting = _editContext.HealthStatus == null ? "" : (_editContext.HealthStatus == true ? _localizer[LangKey.TEST_PASS] : _localizer[LangKey.TEST_FAIL]);
        _messages = "";
        EditContext = new EditContext(_editContext);
        await InvokeAsync(StateHasChanged);
        _isPassShowed = true;
      }
      messageTestConnectionError = "";
    }

    void OnCancelButtonClick()
    {
      _editContext = null;
      _grid.CancelRowEdit();
    }

    async Task HandleValidSubmit()
    {
      var data = _editContext.Id == 0 ?
          await _apiWrapper.ExchangeSettings.CreateAsync(TenantId, _editContext) :
          await _apiWrapper.ExchangeSettings.UpdateAsync(TenantId, _editContext.Id, _editContext);

      if (data.Data != null)
      {
        _editContext = null;
        await _grid.CancelRowEdit();
        await SearchAsync();
        await ExchangeSettingsChanged.InvokeAsync(TenantId);
      }
      else
      {
        _messages = _localizer[data.Error.Message];
      }
    }

    async Task DeleteExchangeSetting()
    {
      var result = await _apiWrapper.ExchangeSettings.DeleteAsync(TenantId, _editContext.Id);

      if (result.IsSuccess && result.Error == null && result.Data == true)
      {
        _PopupVisibleDeleteExchangeSetting = false;
        await _grid.CancelRowEdit();
        _editContext = null;
        await SearchAsync();
        await ExchangeSettingsChanged.InvokeAsync(TenantId);
      }
    }

    async Task HandelOnClickTestConnection()
    {
      //This is lameshit right here. This will fix duplicate validate message of editcontext for now. Delete this if wasm updated and fix this bug.
      await _jSRuntime.InvokeVoidAsync("deleteAnotationErrorMessage");

      if (EditContext.Validate())
      {
        await _jSRuntime.InvokeVoidAsync("disableScreen", true);
        messageTestConnectionError = "";

        var connectResult = await _apiWrapper.ExchangeSettings.CheckConnectionAsync(TenantId, _editContext);

        if (connectResult.IsSuccess && connectResult != null)
        {
          if (connectResult.Error != null)
          {
            if (!string.IsNullOrEmpty(connectResult.Error.Message))
            {
              _toastAppService.ShowWarning(_localizer[connectResult.Error.Message]);
            }
            _editContext.HealthStatus = false;
          }
          else
          {
            if (string.IsNullOrEmpty(connectResult.Data))
            {
              _editContext.HealthStatus = true;
            }
            else
            {
              messageTestConnectionError = connectResult.Data;
              _editContext.HealthStatus = false;
            }
          }
        }

        _heathStatusexchangeSetting = _editContext.HealthStatus == null ? "" : (_editContext.HealthStatus == true ? _localizer[LangKey.TEST_PASS] : _localizer[LangKey.TEST_FAIL]);
        if (_editContext.HealthStatus == true)
        {
          _toastAppService.ShowSuccess(_heathStatusexchangeSetting);
        }
        else
        {
          if (!string.IsNullOrEmpty(messageTestConnectionError))
          {
            _toastAppService.ShowWarning(messageTestConnectionError);
          }
        }


        await _jSRuntime.InvokeVoidAsync("disableScreen", false);
      }
    }

    protected override void OnAfterRender(bool firstRender)
    {
      _jSRuntime.InvokeVoidAsync("afterExchangeSettingComponentRender", "customExchangeGrid", noTextToDisplay);
    }
    void HandleInvalidSubmit()
    {

    }
  }
}
