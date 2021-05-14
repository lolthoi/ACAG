using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Authentication;
using Microsoft.AspNetCore.Components.Forms;

namespace ACAG.Abacus.CalendarConnector.Client.Pages.Account
{
  public partial class ForgotPassword
  {
    private string _currentUrl;
    private ForgotPasswordModel _forgotPassword;
    EditContext _editFormContext;
    ValidationMessageStore msgStore;
    protected override async Task OnInitializedAsync()
    {
      _currentUrl = _navigationManager.BaseUri;
      _forgotPassword = new ForgotPasswordModel()
      {
        Url = _currentUrl
      };
      _editFormContext = new EditContext(_forgotPassword);
      await _authService.Logout();
    }
    private bool ShowErrors;
    private string Error = "";

    async Task HandleOnSubmitAsync()
    {
      ShowErrors = false;
      Error = "";
      msgStore = new ValidationMessageStore(_editFormContext);
      msgStore.Clear();
      if (_editFormContext.Validate())
      {
        await HandleValidSubmitAsync();
      }
    }
    private async Task HandleValidSubmitAsync()
    {
      var result = await _apiWrapper.Accounts.ForgotPasswordAsync(_forgotPassword);

      if (result.Error == null)
      {
        _navigationManager.NavigateTo(_navigationManager.BaseUri + "/ForgotPasswordConfirmation");
      }

      else
      {
        Error = result.Error.Message;
        ShowErrors = true;
      }
    }

    private void OnFocusOutEmail()
    {
      if (_forgotPassword != null && !String.IsNullOrEmpty(_forgotPassword.Email))
      {
        _forgotPassword.Email = _forgotPassword.Email.Trim();
      }
    }
  }
}
