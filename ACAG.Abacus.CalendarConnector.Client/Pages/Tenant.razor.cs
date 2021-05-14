using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Client.Services;
using ACAG.Abacus.CalendarConnector.Models;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Pages
{
  public partial class Tenant
  {
    private string search = "";
    private string searchUser = "";
    private bool _isAdmin = false;
    private bool _isBtnDeleteTenantVisible = false;
    TenantViewModel dataSource = null;
    UserViewModel dataSourceUser = null;
    List<LogDiaryModel> dataSourceLog = null;
    List<int> selectedLogDiaryIds = null;
    DxDataGrid<TenantModel> grid;
    string message = "";
    List<ExchangeSettingModel> _exchangeSettingModels;
    ExchangeSettingModel _exchangeSettingSelected;
    AbacusSettingModel _abacusSettingModel;
    string noTextToDisplay = "";

    bool PopupVisible { get; set; } = false;
    bool PopupVisibleExchangeSetting = false;
    bool AbacusSettingPopupVisible = false;
    bool VisibleBtnDeleteLog = false;
    bool VisibleBtnSaveTenant = true;

    bool isVisiblePopupPayTypeSetting { get; set; } = false;
    int ActiveTabIndex { get; set; } = 0;
    int SelectedTenantId { get; set; } = 0;

    TenantModel EditContext = null;
    string _tenant_Name = "";
    int _tenant_Number = 0;

    Components.MenuLogComponent menuLogComponent;
    EditContext EditFormContext;
    ValidationMessageStore msgStore;

    [Inject] public HttpInterceptorService Interceptor { get; set; }

    protected override async Task OnInitializedAsync()
    {
      Interceptor.RegisterEvent();
      noTextToDisplay = _localizer[LangKey.NO_DATA_TO_DISPLAY];

      var result = await _apiWrapper.Tenants.GetAllAsync(search);

      if (result.IsSuccess && result.Error == null)
      {
        dataSource = result.Data;
      }
    }

    async Task SearchAsync()
    {
      var result = await _apiWrapper.Tenants.GetAllAsync(search);

      if (result.IsSuccess && result.Error == null)
      {
        dataSource = result.Data;
      }

      await InvokeAsync(StateHasChanged);
    }

    async Task OnEditRowAsync(TenantModel item)
    {
      ActiveTabIndex = 0;
      await OnTabChangeAsync(0);
      var apiTenantResult = await _apiWrapper.Tenants.GetByIdAsync(item.Id);

      if (apiTenantResult.IsSuccess && apiTenantResult.Error == null)
      {
        _exchangeSettingSelected = null;
        if (apiTenantResult.Data != null)
        {
          _tenant_Name = apiTenantResult.Data.Name;
          _tenant_Number = apiTenantResult.Data.Number;
          EditContext = apiTenantResult.Data;
          await InvokeAsync(StateHasChanged);
          message = "";

          var apiUserResult = await _apiWrapper.Users.GetTenantUsers(item.Id, searchUser);
          if (apiUserResult.IsSuccess && apiUserResult.Error == null)
          {
            dataSourceUser = apiUserResult.Data;
          }
          else
          {
            dataSourceUser = new UserViewModel();
          }

          var apiLogResult = await _apiWrapper.LogDiaries.GetByTenantAsync(item.Id);
          if (apiLogResult.IsSuccess && apiLogResult.Error == null)
          {
            dataSourceLog = apiLogResult.Data;
          }

          SelectedTenantId = item.Id;

          var apiExchangeSettingResult = await _apiWrapper.ExchangeSettings.GetAllAsync(item.Id, "");

          if (apiExchangeSettingResult.IsSuccess && apiExchangeSettingResult.Error == null)
          {
            _exchangeSettingModels = apiExchangeSettingResult.Data.ExchangeSettings;

            if (_exchangeSettingModels.Count > 0)
            {
              _exchangeSettingSelected = _exchangeSettingModels.Where(t => t.IsEnabled).FirstOrDefault();
              if (_exchangeSettingSelected == null)
              {
                _exchangeSettingSelected = null;
              }
            }
          }

          if (EditContext.AbacusSettingId != null)
          {
            var apiAbacusSettingResutl = await _apiWrapper.AbacusSettings.GetByIdAsync(item.Id, EditContext.AbacusSettingId.Value);

            if (apiAbacusSettingResutl.IsSuccess && apiAbacusSettingResutl.Error == null && apiAbacusSettingResutl.Data != null)
            {
              _abacusSettingModel = apiAbacusSettingResutl.Data;
              EditContext.AbacusHealthStatus = _abacusSettingModel.HealthStatus;
              EditContext.AbacusSettingName = _abacusSettingModel.Name;
            }
          }
        }
      }
    }

    async Task HandleValidSubmitAsync()
    {
      if (EditContext.Name != null && !string.IsNullOrEmpty(EditContext.Name))
      {
        EditContext.Name = EditContext.Name.Trim();

        if (EditContext.Name.Length > 50 || string.IsNullOrEmpty(EditContext.Name))
        {
          return;
        }

        var result = EditContext.Id == 0
            ? await _apiWrapper.Tenants.CreateAsync(EditContext)
            : await _apiWrapper.Tenants.UpdateAsync(EditContext);

        if (result.IsSuccess && result.Error == null)
        {
          if (result.Data != null)
          {
            await grid.CancelRowEdit();
            EditContext = null;

            await SearchAsync();
          }
          else
          {
            message = _localizer[result.Error.Message];
            _toastAppService.ShowWarning(message);
          }
        }
        else
        {
          message = _localizer[(result.Error.Message)];
          _toastAppService.ShowWarning(message);
        }
      }
    }

    async Task HandleOnSubmitAsync()
    {
      EditFormContext = new EditContext(EditContext);
      msgStore = new ValidationMessageStore(EditFormContext);
      msgStore.Clear();
      if (EditFormContext.Validate())
      {
        await HandleValidSubmitAsync();
      }
    }

    async Task OnAddNewAsync()
    {
      ActiveTabIndex = 0;
      EditContext = new TenantModel();
      EditContext.ScheduleTimer = 1;
      EditContext.Number = 1;
      EditContext.IsEnabled = false;
      await grid.CancelRowEdit();
      message = "";
      await grid.StartRowEdit(null);
      await _jSRuntime.InvokeAsync<string>("focusEditor", "first-focus");
    }

    /// <summary>
    /// This is a lame shit. We could'nt find out why dataanotation model keep multiplying when switching tab of tenants
    /// This fix will do the job for now. Delete this if Microsoft blazor updated.
    /// </summary>
    void OnFocusTenant()
    {
      _jSRuntime.InvokeVoidAsync("deleteAnotationErrorMessage");
    }

    void OnCancelButtonClick()
    {
      selectedLogDiaryIds = null;
      EditContext = null;
      grid.CancelRowEdit();
    }

    void OnShowPopupDelete(bool isShow)
    {
      PopupVisible = isShow;
    }

    void OnShowPopupDeleteExchangeSetting()
    {
      PopupVisibleExchangeSetting = true;
    }

    void OnHidePopupDeleteExchangeSetting()
    {
      PopupVisibleExchangeSetting = false;
    }

    void AbacusSettingPopupVisibleChanged(bool isShow)
    {
      AbacusSettingPopupVisible = isShow;
    }

    void AbacusSettingHandleChanged(TenantModel tenant)
    {
      EditContext.AbacusSettingId = tenant.AbacusSettingId;
      EditContext.AbacusSettingName = tenant.AbacusSettingName;
      EditContext.AbacusHealthStatus = tenant.AbacusHealthStatus;
      InvokeAsync(StateHasChanged);
    }

    async Task ExchangeSettingsChangedAsync(int tenantId)
    {
      var apiTenantResult = await _apiWrapper.Tenants.GetByIdAsync(tenantId);

      if (apiTenantResult.IsSuccess && apiTenantResult.Error == null)
      {
        if (apiTenantResult.Data != null)
        {
          var apiExchangeSettingResult = await _apiWrapper.ExchangeSettings.GetAllAsync(tenantId, "");
          if (apiExchangeSettingResult.IsSuccess && apiExchangeSettingResult.Error == null)
          {
            _exchangeSettingModels = apiExchangeSettingResult.Data.ExchangeSettings;

            if (_exchangeSettingModels.Count > 0)
            {
              _exchangeSettingSelected = _exchangeSettingModels.Where(t => t.IsEnabled).FirstOrDefault();
              if (_exchangeSettingSelected == null)
              {
                _exchangeSettingSelected = null;
              }
            }
            else
            {
              _exchangeSettingSelected = null;
            }

            await InvokeAsync(StateHasChanged);
          }
        }
      }
    }

    async Task OnTabChangeAsync(int pageIndex)
    {
      ActiveTabIndex = pageIndex;

      switch (ActiveTabIndex)
      {
        case 0:
          if (_isAdmin)
          {
            _isBtnDeleteTenantVisible = true;
          }
          VisibleBtnSaveTenant = true;
          VisibleBtnDeleteLog = false;
          break;
        case 1:
          if (_isAdmin)
          {
            _isBtnDeleteTenantVisible = true;
          }
          VisibleBtnSaveTenant = true;
          VisibleBtnDeleteLog = false;
          break;
        case 2:
          if (_isAdmin)
          {
            _isBtnDeleteTenantVisible = false;

          }
          VisibleBtnSaveTenant = false;
          VisibleBtnDeleteLog = false;
          break;
        case 3:
          if (_isAdmin)
          {
            _isBtnDeleteTenantVisible = false;
          }
          VisibleBtnSaveTenant = false;
          VisibleBtnDeleteLog = true;

          var result = await _apiWrapper.LogDiaries.GetByTenantAsync(SelectedTenantId);

          if (result.IsSuccess && result.Error == null)
          {
            dataSourceLog = result.Data;
          }
          else
          {
            dataSourceLog = new List<LogDiaryModel>();
          }

          break;
        default:
          break;
      }
    }

    async Task OnDeleteLogAsync(int tenantId)
    {
      if (selectedLogDiaryIds != null)
      {
        var result = await _apiWrapper.LogDiaries.DisableRangeAsync(tenantId, selectedLogDiaryIds);

        if (result.IsSuccess && result.Error == null)
        {
          selectedLogDiaryIds = null;

          var apiLogResult = await _apiWrapper.LogDiaries.GetByTenantAsync(tenantId);

          if (apiLogResult.IsSuccess && apiLogResult.Error == null)
          {
            dataSourceLog = apiLogResult.Data;
          }
          else
          {
            message = apiLogResult.Error.Message;
            _toastAppService.ShowWarning(_localizer[message]);
          }

          menuLogComponent.grid.ClearSelection();
        }
        else
        {
          message = result.Error.Message;
          _toastAppService.ShowWarning(_localizer[message]);
        }
      }
    }

    void HandleSelectedLogDiaryChanged(DataGridSelection<LogDiaryModel> selections)
    {
      var rs = selections.SelectedKeys?.Result;

      selectedLogDiaryIds = rs?.Cast<int>()?.ToList<int>();
    }

    void OnHidePopupAbacusSetting()
    {
      AbacusSettingPopupVisible = false;
      InvokeAsync(StateHasChanged);
    }

    void OnShowPayTypeSettingPopup()
    {
      isVisiblePopupPayTypeSetting = true;
    }

    void OnHidePayTypeSettingPopup()
    {
      isVisiblePopupPayTypeSetting = false;
    }

    async Task DeleteTenantAsync()
    {
      var result = await _apiWrapper.Tenants.DeleteAsync(EditContext.Id);

      if (result.IsSuccess && result.Error == null)
      {
        PopupVisible = false;
        await grid.CancelRowEdit();
        EditContext = null;

        var apiTenantResult = await _apiWrapper.Tenants.GetAllAsync(search);

        if (apiTenantResult.IsSuccess && apiTenantResult.Error == null)
        {
          dataSource = apiTenantResult.Data;
        }
        else
        {
          message = apiTenantResult.Error.Message;
          _toastAppService.ShowWarning(_localizer[message]);
        }

        await InvokeAsync(StateHasChanged);
      }
      else
      {
        message = result.Error.Message;
        _toastAppService.ShowWarning(_localizer[message]);
      }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      await _jSRuntime.InvokeVoidAsync("afterTenantRender", "customTenantGrid", noTextToDisplay);
    }
  }
}
