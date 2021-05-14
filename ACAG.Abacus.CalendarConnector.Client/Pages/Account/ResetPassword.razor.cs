using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Components;

namespace ACAG.Abacus.CalendarConnector.Client.Pages.Account
{
  public partial class ResetPassword
  {
    [Parameter]
    public string Code { get; set; }
    private ResetPasswordModel _resetPassword = null;
    private bool ShowErrors;
    private string Error = "";
    private bool _isCodeValid = true;

    protected override async Task OnInitializedAsync()
    {
      _resetPassword = new ResetPasswordModel()
      {
        Code = Code
      };
      var result = await _apiWrapper.Accounts.CodeForResetPassword(Code);
      if (!result.Data)
      {
        _isCodeValid = false;
      }
    }

    private async Task HandleResetPassword()
    {
      ShowErrors = false;
      Error = "";

      var result = await _apiWrapper.Accounts.ResetPasswordAsync(_resetPassword);

      if (result.Error == null)
      {
        await _authService.Logout();
        _navigationManager.NavigateTo(_navigationManager.BaseUri + "/ResetPasswordConfirmation");
      }
      else
      {
        Error = result.Error.Message;
        ShowErrors = true;
      }
    }
  }
}
