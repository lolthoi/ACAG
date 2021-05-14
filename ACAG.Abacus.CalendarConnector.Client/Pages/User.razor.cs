using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Client.Common;
using ACAG.Abacus.CalendarConnector.Client.Services;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ACAG.Abacus.CalendarConnector.Client.Pages
{
  public partial class User
  {
    private string _search = "";
    UserViewModel _userViewModel = null;
    DxDataGrid<UserModel> _grid;
    UserModel _editContext { get; set; } = null;
    bool _popupVisible = false;
    bool _enableCbxUserStatus = true;
    bool _enableBtnUpdateUser = true;
    bool _enableCbxUserRoles = true;

    Roles _currentUserRole;

    string _messages = "";
    List<TenantModelForUser> _tenants = null;

    string _noTextToDisplay = "";

    List<LanguageModel> _lstCultures = new List<LanguageModel>();
    AppRoleViewModel _lstRoles = new AppRoleViewModel();
    private string _currentUrl;
    private EditContext _EditContext;

    [Inject] 
    public HttpInterceptorService Interceptor { get; set; }

    protected override async Task OnInitializedAsync()
    {
      Interceptor.RegisterEvent();
      _noTextToDisplay = _localizer[LangKey.NO_DATA_TO_DISPLAY];

      var apiCultureResult = await _apiWrapper.Cultures.GetAllAsync();
      if (apiCultureResult.IsSuccess && apiCultureResult.Error == null)
      {
        _lstCultures = apiCultureResult.Data
          .Where(cul => cul.DisplayName != LangConfig.CULT_TRANSLATION)
          .ToList();
      }

      var apiRoleResult = await _apiWrapper.Roles.GetAllAsync();
      if (apiRoleResult.IsSuccess && apiRoleResult.Error == null)
      {
        _lstRoles = apiRoleResult.Data;
      }

      var apiUserResult = await _apiWrapper.Users.GetAllAsync(_search);
      if (apiUserResult.IsSuccess && apiUserResult.Error == null)
      {
        _userViewModel = apiUserResult.Data;
      }

      _currentUserRole = await _authService.GetCurrentUserRoleAsync();
      //_rolesForCreate = GetRolesForCreate();
      //_rolesForUpdate = GetRolesForUpdate();
    }

    async Task SearchAsync()
    {
      var apiUserResult = await _apiWrapper.Users.GetAllAsync(_search);
      if (apiUserResult.IsSuccess && apiUserResult.Error == null)
      {
        _userViewModel = apiUserResult.Data;
      }

      await InvokeAsync(StateHasChanged);
    }

    async Task OnAddRowAsync()
    {
      var apiTenantResult = await _apiWrapper.Tenants.GetAllForUserAsync();
      if (apiTenantResult.IsSuccess && apiTenantResult.Error == null)
      {
        _tenants = apiTenantResult.Data.Tenants.ToList();
      }

      _editContext = new UserModel()
      {
        AppRole = _lstRoles.Roles.Where(x => x.IsAdministrator == false).FirstOrDefault(),
        Language = _lstCultures.FirstOrDefault(),
        IsEnabled = true
      };

      _EditContext = new EditContext(_editContext);

      _messages = "";
      _enableCbxUserStatus = true;
      await EnableCbxUserRoles();
      await InvokeAsync(StateHasChanged);
      await _grid.StartRowEdit(null);
      await _jSRuntime.InvokeAsync<string>("focusEditor", "first-focus");
    }

    async Task OnEditRowAsync(UserModel item)
    {
      var apiTenantResult = await _apiWrapper.Tenants.GetAllForUserAsync();
      if (apiTenantResult.IsSuccess && apiTenantResult.Error == null)
      {
        _tenants = apiTenantResult.Data.Tenants.ToList();
      }

      var apiUserResult = await _apiWrapper.Users.GetByIdAsync(item.Id);
      if (apiUserResult.IsSuccess && apiUserResult.Error == null && apiUserResult.Data != null)
      {
        LanguageForModel(apiUserResult.Data);
        _editContext = apiUserResult.Data;
        _messages = "";
        _EditContext = new EditContext(_editContext);

        EnableCbxUserStatus();
        await EnableCbxUserRoles();
        await EnableUpdateButton();

        await InvokeAsync(StateHasChanged);
      }
    }

    async Task EnableCbxUserRoles()
    {
      var currentUser = await _authService.GetCurrentUserAsync();
      var currentUserMail = currentUser.Key;

      if (_editContext.Email == currentUserMail)
      {
        _enableCbxUserRoles = false;
      }
      else
      {
        _enableCbxUserRoles = true;
      }
    }

    void EnableCbxUserStatus()
    {
      if (_editContext.IsEnabled)
      {
        var role = _editContext.AppRole.Code;
        var contextRole = role == "SysAdmin" ? Roles.SYSADMIN : role == "Administrator" ? Roles.ADMINISTRATOR : Roles.USER;

        if (_currentUserRole == Models.Common.Roles.SYSADMIN)
        {

          if (contextRole == Models.Common.Roles.ADMINISTRATOR
            || contextRole == Models.Common.Roles.USER)
          {
            _enableCbxUserStatus = true;
          }
          else
          {
            _enableCbxUserStatus = false;
          }
        }
        else if (_currentUserRole == Models.Common.Roles.ADMINISTRATOR)
        {
          if (contextRole == Models.Common.Roles.USER)
          {
            _enableCbxUserStatus = true;
          }
          else
          {
            _enableCbxUserStatus = false;
          }
        }
        else if (_currentUserRole == Models.Common.Roles.USER)
        {
          _enableCbxUserStatus = false;
        }
      }
    }

    async Task EnableUpdateButton()
    {
      var currentUser = await _authService.GetCurrentUserAsync();
      var currentUserMail = currentUser.Key;

      var role = _editContext.AppRole.Code;
      var contextRole = role == "SysAdmin" ? Roles.SYSADMIN : role == "Administrator" ? Roles.ADMINISTRATOR : Roles.USER;

      if (_currentUserRole == Models.Common.Roles.SYSADMIN)
      {

        if (_editContext.Email != currentUserMail
          && contextRole == Models.Common.Roles.SYSADMIN)
        {
          _enableBtnUpdateUser = false;
        }
        else
        {
          _enableBtnUpdateUser = true;
        }
      }
      else if (_currentUserRole == Models.Common.Roles.ADMINISTRATOR)
      {
        if (_editContext.Email == currentUserMail
          || contextRole == Models.Common.Roles.USER)
        {
          _enableBtnUpdateUser = true;
        }
        else
        {
          _enableBtnUpdateUser = false;
        }
      }
      else if (_currentUserRole == Models.Common.Roles.USER)
      {
        _enableBtnUpdateUser = false;
      }
    }

    void OnCancelButtonClick()
    {
      _editContext = null;
      _grid.CancelRowEdit();
    }

    async Task HandleValidSubmit()
    {
      #region Validate 

      if (!string.IsNullOrEmpty(_editContext.FirstName))
      {
        _editContext.FirstName = _editContext.FirstName.Trim();
      }
      if (_editContext.FirstName.Length < 3)
      {
        _toastAppService.ShowWarning(_localizer[LangKey.MSG_MIN_LENGTH_OF_FIRST_NAME_IS_3]);
        return;
      }
      if (_editContext.FirstName.Length > 50)
      {
        _toastAppService.ShowWarning(_localizer[LangKey.MSG_MAX_LENGTH_OF_FIRST_NAME_IS_50]);
      }

      if (!string.IsNullOrEmpty(_editContext.LastName))
      {
        _editContext.LastName = _editContext.LastName.Trim();
      }
      if (_editContext.LastName.Length < 3)
      {
        _toastAppService.ShowWarning(_localizer[LangKey.MSG_MIN_LENGTH_OF_LAST_NAME_IS_3]);
        return;
      }
      if (_editContext.LastName.Length > 50)
      {
        _toastAppService.ShowWarning(_localizer[LangKey.MSG_MAX_LENGTH_OF_LAST_NAME_IS_50]);
      }

      if (!string.IsNullOrEmpty(_editContext.Email))
      {
        _editContext.Email = _editContext.Email.Trim();
      }
      if (_editContext.Email.Length > 75)
      {
        _toastAppService.ShowWarning(_localizer[LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75]);
        return;
      }

      if (_editContext.AppRole.IsAdministrator)
      {
        if (_editContext.Tenant != null)
        {
          _editContext.Tenant.Id = 0;
          _editContext.Tenant.Name = " ";
        }
      }

      #endregion

      var apiUserResult = _editContext.Id == 0
          ? await _apiWrapper.Users.CreateAsync(_editContext, AppDefiner.DefaultIdenityPwd)
          : await _apiWrapper.Users.UpdateAsync(_editContext);

      if (apiUserResult.IsSuccess && apiUserResult.Error == null && apiUserResult.Data != null)
      {
        _editContext = null;
        await _grid.CancelRowEdit();
        await SearchAsync();
      }
      else
      {
        _messages = _localizer[apiUserResult.Error.Message];
        _toastAppService.ShowWarning(_messages);
      }
    }

    async Task DeleteUser()
    {
      var result = await _apiWrapper.Users.DeleteAsync(_editContext.Id);

      if (result.IsSuccess && result.Error == null && result.Data == true)
      {
        _popupVisible = false;
        _editContext = null;

        var apiUserResult = await _apiWrapper.Users.GetAllAsync(_search);
        if (apiUserResult.IsSuccess && apiUserResult.Error == null)
        {
          _userViewModel = apiUserResult.Data;
        }

        await _grid.CancelRowEdit();
        await InvokeAsync(StateHasChanged);
      }
      else
      {
        _toastAppService.ShowWarning(_localizer[result.Error.Message]);
      }
    }

    void LanguageForModel<T>(T model)
    {
      var props = model.GetType().GetProperties();
      int index = 0;
      foreach (var item in props)
      {
        index++;
      }
    }

    async Task ResetPassword()
    {
      //This is lameshit right here. This will fix duplicate validate message of editcontext for now. Delete this if wasm updated and fix this bug.
      await _jSRuntime.InvokeVoidAsync("deleteAnotationErrorMessage");
      if (_EditContext.Validate())
      {
        try
        {
          _currentUrl = _navigationManager.BaseUri;
          ForgotPasswordModel forgotPassword = new ForgotPasswordModel()
          {
            Email = _editContext.Email,
            Url = _currentUrl
          };
          var result = await _apiWrapper.Accounts.ForgotPasswordAsync(forgotPassword);
          if (result.Error == null)
          {
            _toastAppService.ShowSuccess(string.Format(_localizer[LangKey.MSG_SEND_RESET_PASSWORD_MAIL_TO], _editContext.Email));
          }
          else
          {
            _toastAppService.ShowWarning(_localizer[result.Error.Message]);
          }
        }
        catch
        {
          _toastAppService.ShowWarning(_localizer[LangKey.MSG_SENDING_EMAILS_IS_NOT_SUCCESSFUL_PLEASE_CONTACT_TO_THE_ADMINISTRATOR]);
        }
      }
    }

    async Task OnDropDownUserTenantVisibleChangesAsync(bool value)
    {
      await _jSRuntime.InvokeVoidAsync("changeNoDataText", "CbxUserTenant", _noTextToDisplay, 110);
    }

    async Task SearchUserTenantAsync()
    {
      await _jSRuntime.InvokeVoidAsync("changeNoDataText", "CbxUserTenant", _noTextToDisplay, 110);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      await _jSRuntime.InvokeVoidAsync("afterUserRender", "customUserGrid", _noTextToDisplay);
    }
  }
}
