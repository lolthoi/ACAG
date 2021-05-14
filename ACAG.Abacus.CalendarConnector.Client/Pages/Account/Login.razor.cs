using System;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models.Authentication;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ACAG.Abacus.CalendarConnector.Client.Pages.Account
{
  public partial class Login
  {
    private LoginModel loginModel = new LoginModel();
    private bool ShowErrors;
    private string Error = "";
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    protected override async void OnInitialized()
    {
      _jSRuntime.InvokeVoidAsync("loginLanguage");
      //authService.Logout();
      //TODO: Better solution: Check authenticate state of user.
      var token = await _localStorage.GetItemAsync<string>("authToken");
      if (!String.IsNullOrEmpty(token))
      {
        navigationManager.NavigateTo(navigationManager.BaseUri + "tenant");
      }
    }

    private async Task HandleLogin()
    {
      ShowErrors = false;
      Error = "";

      var result = await authService.Login(loginModel);

      if (result.Successful)
      {
        navigationManager.NavigateTo(navigationManager.BaseUri + "tenant");
      }
      else
      {
        Error = result.Error;
        ShowErrors = true;
      }
    }

    private void OnFocusOutEmail()
    {
      if (loginModel != null && !String.IsNullOrEmpty(loginModel.Email))
      {
        loginModel.Email = loginModel.Email.Trim();
      }
    }
  }
}
