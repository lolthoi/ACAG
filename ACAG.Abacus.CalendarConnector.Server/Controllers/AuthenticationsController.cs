using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [SwaggerTag("Authentications Management")]
  [Authorize]
  public class AuthenticationsController : ControllerBase
  {
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationsController(IAuthenticationService authenticationService)
    {
      _authenticationService = authenticationService;
    }

    #region GET

    /// <summary>
    /// Find a user by its email
    /// </summary>
    /// <remarks>
    /// Return a single user
    /// </remarks>
    /// <param name="email">Email of the user</param>
    /// <returns>Return a single user</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("{email}")]
    public UserAuthModel GetByEmail(string email)
    {
      var result = _authenticationService.GetByEmail(email);
      return result.ToResponse();
    }

    #endregion

    #region POST

    /// <summary>
    /// Login with a user
    /// </summary>
    /// <remarks>
    /// Return information of the logged-in user
    /// </remarks>
    /// <param name="email">Email of the user</param>
    /// <param name="password">New password</param>
    /// <returns>Return information of the logged-in user</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost]
    public UserAuthModel Login(string email, string password)
    {
      var result = _authenticationService.Login(email, password);
      return result.ToResponse();
    }

    #endregion
  }
}
