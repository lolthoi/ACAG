using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Client.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Pages.Components
{
  public partial class AbacusSettingComponent
  {
    [Parameter]
    public TenantModel Tenant { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback<TenantModel> AbacusSettingChanged { get; set; }

    [Inject] public HttpInterceptorService Interceptor { get; set; }

    private EditContext EditContext;

    public bool MakeNewAbacusSetting = false;
    public bool AbacusSettingPopupVisible = false;

    public AbacusSettingModel AbacusSetting { get; set; } = null;

    bool isPassShowed = false;
    bool showPassword
    {
      get
      {
        return isPassShowed;
      }
      set
      {
        isPassShowed = value;
        _jSRuntime.InvokeVoidAsync("changePasswordVisibility", "PasswordTextBox", isPassShowed);
      }
    }

    protected override async Task OnInitializedAsync()
    {
      Interceptor.RegisterEvent();

      var result = await _apiWrapper.AbacusSettings.GetByTenantAsync(Tenant.Id);
      if (result.IsSuccess && result.Error == null)
      {
        AbacusSetting = result.Data;
      }

      if (AbacusSetting == null)
      {
        AbacusSetting = new AbacusSettingModel();
        MakeNewAbacusSetting = true;
      }

      AbacusSetting.TenantId = Tenant.Id;
      EditContext = new EditContext(AbacusSetting);
    }

    protected override void OnAfterRender(bool firstRender)
    {
      if (firstRender)
      {
        _jSRuntime.InvokeVoidAsync("changePasswordVisibility", "PasswordTextBox", false);
      }
    }

    async Task HandelOnClickTestConnection()
    {
      if (EditContext.Validate())
      {
        await _jSRuntime.InvokeVoidAsync("disableScreen", true);

        var connectModel = new AbacusSettingConnectionModel()
        {
          AbacusSettingName = AbacusSetting.Name,
          AbacusSettingDescription = AbacusSetting.Description,
          AbacusSettingServiceUrl = AbacusSetting.ServiceUrl,
          AbacusSettingServicePort = AbacusSetting.ServicePort,
          AbacusSettingServiceUseSsl = AbacusSetting.ServiceUseSsl,
          AbacusSettingServiceUser = AbacusSetting.ServiceUser,
          AbacusSettingServiceUserPassword = AbacusSetting.ServiceUserPassword,
          TenantName = Tenant.Name,
          TenantDescription = Tenant.Description,
          TenantNumber = Tenant.Number
        };

        var apiConnectResult = await _apiWrapper.AbacusSettings.CheckConnectionAsync(Tenant.Id, connectModel);

        if (apiConnectResult.IsSuccess && apiConnectResult.Data && apiConnectResult.Error == null)
        {
          AbacusSetting.HealthStatus = true;
          _toastAppService.ShowSuccess(_localizer[LangKey.TEST_PASS]);
        }
        else
        {
          AbacusSetting.HealthStatus = false;
          _toastAppService.ShowWarning(_localizer[LangKey.TEST_FAIL]);
        }

        await _jSRuntime.InvokeVoidAsync("disableScreen", false);
      }
    }

    async Task HandleValidSubmit()
    {
      var editModel = new AbacusSettingEditModel()
      {
        Name = AbacusSetting.Name,
        ServiceUrl = AbacusSetting.ServiceUrl,
        ServicePort = AbacusSetting.ServicePort,
        ServiceUseSsl = AbacusSetting.ServiceUseSsl,
        ServiceUser = AbacusSetting.ServiceUser,
        ServiceUserPassword = AbacusSetting.ServiceUserPassword
      };

      if (MakeNewAbacusSetting)
      {

        var result = await _apiWrapper.AbacusSettings.CreateAsync(Tenant.Id, editModel);

        if (result.IsSuccess && result.Error == null)
        {
          AbacusSetting = result.Data;
          MakeNewAbacusSetting = false;
          await VisibleChanged.InvokeAsync(false);
          UpdateTenantAndInvokeChange();
        }
        else
        {
          _toastAppService.ShowWarning(_localizer[result.Error.Message]);
        }
      }
      else
      {
        var result = await _apiWrapper.AbacusSettings.UpdateAsync(Tenant.Id, AbacusSetting.Id, editModel);

        if (result.IsSuccess && result.Error == null)
        {
          AbacusSetting = result.Data;
          await VisibleChanged.InvokeAsync(false);
          UpdateTenantAndInvokeChange();
        }
        else
        {
          _toastAppService.ShowWarning(_localizer[result.Error.Message]);
        }
      }
    }

    void HandleInvalidSubmit()
    {

    }

    void UpdateTenantAndInvokeChange()
    {
      Tenant.AbacusSettingId = AbacusSetting.Id;
      Tenant.AbacusSettingName = AbacusSetting.Name;
      Tenant.AbacusHealthStatus = AbacusSetting.HealthStatus;

      AbacusSettingChanged.InvokeAsync(Tenant);
    }
  }
}
