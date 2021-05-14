using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Language;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [SwaggerTag("Account Management")]
  public class AccountController : ControllerBase
  {
    private readonly IConfiguration _configuration;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMailService _emailSender;
    private readonly IStringLocalizer<Resource> _localizer;
    public AccountController(IConfiguration configuration,IAuthenticationService authenticationService, IMailService mailService, IStringLocalizer<Resource> localizer)
    {
      _configuration = configuration;
      _authenticationService = authenticationService;
      _emailSender = mailService;
      _localizer = localizer;
    }

    /// <summary>
    /// Login with an account
    /// </summary>
    /// <remarks>
    /// Return the status whether login is successed of failed
    /// </remarks>
    /// <param name="login">Information of the account</param>
    /// <returns>Return the status whether login is successed of failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
      if(!String.IsNullOrEmpty(login.Email))
      {
        login.Email = login.Email.Trim();
      }

      var result = _authenticationService.Login(login.Email, login.Password);
      if (result.Error != null)
      {
        //return BadRequest(new LoginResult { Successful = false, Error = result.Error.Message });
        throw new LogicException(new Models.ErrorModel(Models.ErrorType.BAD_REQUEST, result.Error.Message));
      }

      var user = _authenticationService.GetByEmail(login.Email);

      var roles = user.Data.Role;
      var claims = new List<Claim>();
      claims.Add(new Claim(ClaimTypes.Sid, user.Data.Id.ToString()));
      claims.Add(new Claim(ClaimTypes.Email, login.Email));
      claims.Add(new Claim(ClaimTypes.Role, roles));

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityKey"]));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var expiry = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtExpiryInMinutes"]));

      var token = new JwtSecurityToken(
          _configuration["JwtIssuer"],
          _configuration["JwtAudience"],
          claims,
          expires: expiry,
          signingCredentials: creds
      );
      var results = new LoginResult 
      { 
        Successful = true, 
        Token = new JwtSecurityTokenHandler().WriteToken(token), 
        FirstName = user.Data.FirstName,
        LastName = user.Data.LastName,
        CultureId = user.Data.CultureId
      };
      return Ok(results);
    }

    /// <summary>
    /// Forgot Password with an email exits
    /// </summary>
    /// <remarks>
    /// Return the status whether login is successed of failed
    /// </remarks>
    /// <param name="forgotPasswordModel">Information of the forgotPassword</param>
    /// <returns>Return the status whether Forgot Password is successed of failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost("ForgotPassword")]
    [AllowAnonymous]
    public async Task<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
    {
      var forgotPassword = _authenticationService.ForgotPassword(forgotPasswordModel);
      if (forgotPassword.Error != null)
      {
        throw new LogicException(new ErrorModel(forgotPassword.Error.Type, forgotPassword.Error.Message));
      }

      var result = forgotPassword.Data.LanguageCode;
      CultureInfo culture;
      if (result != null)
        culture = new CultureInfo(result);
      else
        culture = new CultureInfo("en-US");
      CultureInfo.DefaultThreadCurrentCulture = culture;
      CultureInfo.DefaultThreadCurrentUICulture = culture;

      var code = forgotPassword.Data.Code;
      var callBack = string.Format("{0}ResetPassword/{1}", forgotPassword.Data.Url, code);

      var mailRequest = new Logic.MailRequest()
      {
        ToEmail = forgotPassword.Data.Email,
        Subject = _localizer[LangKey.RESET_PASSWORD_TITLE_RESET_PASSWORD],
        Body = $"{_localizer[LangKey.MSG_PLEASE_RESET_YOUR_PASSWORD_BY]} <a href='{HtmlEncoder.Default.Encode(callBack)}'>{_localizer[LangKey.MSG_CLICKING_HERE]}</a>."
      };

      await _emailSender.SendEmailAsync(mailRequest);

      return forgotPassword.ToResponse();
    }

    /// <summary>
    /// Check code exits for reset password
    /// </summary>
    /// <remarks>
    /// Return the status code is successed of failed
    /// </remarks>
    /// <param name="code">Information of Code</param>
    /// <returns>Return the status whether code is successed of failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Code not found</response>
    /// <response code="500">Server side error</response>
    [HttpGet("CodeForResetPassword")]
    [AllowAnonymous]
    public async Task<bool> CheckCodeForResetPassword(string code)
    {
      var data = _authenticationService.CheckCodeForResetPassword(code);
      return data.ToResponse();
    }

    /// <summary>
    /// Reset Password with an code exits
    /// </summary>
    /// <remarks>
    /// Return the status whether login is successed of failed
    /// </remarks>
    /// <param name="resetPasswordModel">Information of Reset Password</param>
    /// <returns>Return the status whether reset password is successed of failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    public async Task<ResetPasswordModel> ResetPassword(ResetPasswordModel resetPasswordModel)
    {
      var result = _authenticationService.ResetPassword(resetPasswordModel);
      
      return result.ToResponse();
    }
  }
}
