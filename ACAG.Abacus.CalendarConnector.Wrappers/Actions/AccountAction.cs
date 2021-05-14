using System.Net.Http;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Authentication;
using Blazored.LocalStorage;

namespace ACAG.Abacus.CalendarConnector.Wrappers.Actions
{
  public class AccountAction : ActionApi
  {
    public AccountAction(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
      : base(clientFactory, localStorage)
    {

    }

    public async Task<ApiResult<LoginResult>> LoginAsync(LoginModel loginModel)
    {
      var uri = $"api/Account";
      return await PostRequestAsync<LoginResult>(uri, loginModel);
    }

    public async Task<ApiResult<ForgotPasswordModel>> ForgotPasswordAsync(ForgotPasswordModel forgotPasswordModel)
    {
      var uri = $"api/Account/ForgotPassword";
      return await PostRequestAsync<ForgotPasswordModel>(uri, forgotPasswordModel);
    }

    public async Task<ApiResult<ResetPasswordModel>> ResetPasswordAsync(ResetPasswordModel resetPassword)
    {
      var uri = $"api/Account/ResetPassword";
      return await PostRequestAsync<ResetPasswordModel>(uri, resetPassword);
    }

    public async Task<ApiResult<bool>> CodeForResetPassword(string code)
    {
      var uri = $"api/Account/CodeForResetPassword?code={code}";
      return await GetRequestAsync<bool>(uri);
    }
  }
}
