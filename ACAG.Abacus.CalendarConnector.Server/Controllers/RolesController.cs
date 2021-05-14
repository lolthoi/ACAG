using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [SwaggerTag("Roles Management")]
  [Authorize]
  public class RolesController : ControllerBase
  {
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
      _roleService = roleService;
    }

    #region GET

    /// <summary>
    /// Find all existing roles
    /// </summary>
    /// <remarks>
    /// Return all existing roles
    /// </remarks>
    /// <returns>Return all existing roles</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet()]
    public AppRoleViewModel GetAll()
    {
      var result = _roleService.GetAll();
      return result.ToResponse();
    }

    #endregion
  }
}
