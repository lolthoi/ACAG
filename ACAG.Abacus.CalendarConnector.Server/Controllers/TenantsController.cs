using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [SwaggerTag("Tenants Management")]
  [Authorize]
  public class TenantsController : ControllerBase
  {
    private readonly ITenantService _tenantService;
    private readonly IUserService _userService;

    public TenantsController(ITenantService tenantService, IUserService userService)
    {
      _tenantService = tenantService;
      _userService = userService;
    }

    #region GET

    /// <summary>
    /// Find all tenants by search text
    /// </summary>
    /// <remarks>
    /// Return all tenants that meet the criteria
    /// </remarks>
    /// <param name="searchText">The criteria to filter tenant</param>
    /// <returns>Return all tenants that meet the criteria</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet]
    public TenantViewModel GetAll([FromQuery] string searchText)
    {
      var result = _tenantService.GetAll(searchText);
      return result.ToResponse();
    }

    /// <summary>
    /// Find all tenants where tenant is enable
    /// </summary>
    /// <remarks>
    /// Return all tenants that meet the criteria
    /// </remarks>
    /// <returns>Return all tenants that meet the criteria</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("GetAllTenants")]
    public TenantModelForUserViewModel GetAllTenants()
    {
      var result = _tenantService.GetAllTenants();
      return result.ToResponse();
    }

    /// <summary>
    /// Find a tenant by its id
    /// </summary>
    /// <remarks>
    /// Return a single tenant
    /// </remarks>
    /// <param name="tenantId">Tenant id of the pay type</param>
    /// <returns>Return a single tenant</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("{tenantId:int}")]
    public TenantModel GetById(int tenantId)
    {
      var result = _tenantService.GetById(tenantId);
      return result.ToResponse();
    }

    /// <summary>
    /// Find all users by tenantId and search text
    /// </summary>
    /// <remarks>
    /// Return all users that meet the criteria
    /// </remarks>
    /// <param name="tenantId"></param>
    /// <param name="searchText"></param>
    /// <returns>Return all users that meet the criteria</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("{tenantId:int}/Users")]
    public UserViewModel Users(int tenantId, [FromQuery]string searchText)
    {
      var result = _userService.GetTenantUsers(tenantId, searchText);
      return result.ToResponse();
    }

    #endregion

    #region POST

    /// <summary>
    /// Create a new tenant
    /// </summary>
    /// <remarks>
    /// Return the newly created tenant
    /// </remarks>
    /// <param name="model">Tenant object that needs to be created</param>
    /// <returns>Return the newly created tenant</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost]
    public TenantModel Create(TenantEditModel model)
    {
      var tennant = new TenantModel
      {
        Id = model.Id,
        Name = model.Name,
        Description = model.Description ?? string.Empty,
        Number = model.Number,
        IsEnabled = model.IsEnabled,
        ScheduleTimer = model.ScheduleTimer
      };

      var result = _tenantService.Create(tennant);
      return result.ToResponse();
    }

    #endregion

    #region PUT

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    /// <remarks>
    /// Return the updated tenant
    /// </remarks>
    /// <param name="model">Tenant object that needs to be created</param>
    /// <returns>Return the updated tenant</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPut]
    public TenantModel Update(TenantEditModel model)
    {
      var tennant = new TenantModel
      {
        Id = model.Id,
        Name = model.Name,
        Description = model.Description ?? string.Empty,
        Number = model.Number,
        IsEnabled = model.IsEnabled,
        ScheduleTimer = model.ScheduleTimer
      };

      var result = _tenantService.Update(tennant);
      return result.ToResponse();
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Delete an existing tenant
    /// </summary>
    /// <remarks>
    /// Return whether the deleting is successed or failed
    /// </remarks>
    /// <param name="tenantId">Tenant id that need to be deleted</param>
    /// <returns>Return whether the deleting is successed or failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpDelete("{tenantId:int}")]
    public bool Delete(int tenantId)
    {
      var result = _tenantService.Delete(tenantId);
      return result.ToResponse();
    }

    #endregion
  }
}
