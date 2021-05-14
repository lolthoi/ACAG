using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Authentication;
using ACAG.Abacus.CalendarConnector.Models.Common;
using ACAG.Abacus.CalendarConnector.Wrappers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace ACAG.Abacus.CalendarConnector.Client.Services
{
  public interface IAuthService
  {
    Task<LoginResult> Login(LoginModel loginModel);
    Task Logout();
    Task<RegisterResult> Register(RegisterModel registerModel);
    Task ClearLocalStorage();
    Task<string> GetCurrentFullName();
    Task<LoginUserModel> GetCurrentUserAsync();
    Task<Roles> GetCurrentUserRoleAsync();
  }

  public class AuthService : IAuthService
  {
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;
    private readonly IApiWrapper _apiWrapper;

    public AuthService(AuthenticationStateProvider authenticationStateProvider,
                       ILocalStorageService localStorage,
                       IApiWrapper apiWrapper)
    {
      _authenticationStateProvider = authenticationStateProvider;
      _localStorage = localStorage;
      _apiWrapper = apiWrapper;
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
      var response = await _apiWrapper.Accounts.LoginAsync(loginModel);
      LoginResult result = new LoginResult();

      if (response.IsSuccess && response.Error == null)
      {
        result = response.Data;
        await _localStorage.SetItemAsync("authToken", result.Token);
        await _localStorage.SetItemAsync("fullName", result.FullName);
        await _localStorage.SetItemAsync("userEmail", loginModel.Email);
        await _localStorage.SetItemAsync("userCultureId", result.CultureId.ToString());

        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token);
        return result;
      }
      else
      {
        result.Error = response.Error.Message;
        result.Successful = false;
        return result;
      }

      return result;
    }

    public async Task Logout()
    {
      await ClearLocalStorage();
      ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
    }

    public Task<RegisterResult> Register(RegisterModel registerModel)
    {
      throw new NotImplementedException();
    }

    public async Task ClearLocalStorage()
    {
      await _localStorage.RemoveItemAsync("authToken");
      await _localStorage.RemoveItemAsync("fullName");
      await _localStorage.RemoveItemAsync("userEmail");
      await _localStorage.RemoveItemAsync("userCultureId");
    }

    public async Task<string> GetCurrentFullName()
    {
      var curUser = await _localStorage.GetItemAsync<string>("fullName");
      return curUser;
    }

    IEnumerable<Claim> _claims = Enumerable.Empty<Claim>();

    public async Task<LoginUserModel> GetCurrentUserAsync()
    {
      var userEmail = await _localStorage.GetItemAsync<string>("userEmail");
      var currentUser = new LoginUserModel { Key = userEmail };
      return currentUser;
    }

    public async Task<Roles> GetCurrentUserRoleAsync()
    {
      var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
      var user = authState.User;

      Roles currentUserRole = Roles.USER;

      if (user.Identity.IsAuthenticated)
      {
        _claims = user.Claims;
        var role = user.FindFirst(ClaimTypes.Role);
        currentUserRole = role.Value == "SysAdmin" ? Roles.SYSADMIN : role.Value == "Administrator" ? Roles.ADMINISTRATOR : Roles.USER;
      }

      return currentUserRole;
    }
  }
}
