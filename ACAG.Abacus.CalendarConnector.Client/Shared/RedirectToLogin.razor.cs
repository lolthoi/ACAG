using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ACAG.Abacus.CalendarConnector.Client.Shared
{
  public partial class RedirectToLogin
  {
    [CascadingParameter]
    private Task<AuthenticationState> AuthenticationStateTask { get; set; }

    protected override async Task OnInitializedAsync()
    {
      var authenticationState = await AuthenticationStateTask;

      if (authenticationState?.User?.Identity is null || !authenticationState.User.Identity.IsAuthenticated)
      {

        Console.WriteLine("authenticationState?.User?.Identity:  " + authenticationState?.User?.Identity);
        Console.WriteLine("authenticationState.User.Identity.IsAuthenticated:  " + authenticationState.User.Identity.IsAuthenticated);

        _navigationManager.NavigateTo($"Login", false);
      }
    }
  }
}
