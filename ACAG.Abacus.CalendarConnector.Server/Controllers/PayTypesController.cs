using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  [ApiController]
  [Route("api/Tenant/{tenantId:int}/[controller]")]
  [SwaggerTag("Pay types management")]
  [Authorize]
  public class PayTypesController : ControllerBase
  {
    private readonly IPayTypeService _payTypeService;

    public PayTypesController(IPayTypeService payTypeService)
    {
      _payTypeService = payTypeService;
    }

    #region GET

    /// <summary>
    /// Find all pay types of a tenant by search text
    /// </summary>
    /// <remarks>
    /// Return all pay types that meet the criteria
    /// </remarks>
    /// <param name="tenantId">Tenant id of the pay type</param>
    /// <param name="searchText">The criteria to filter pay types</param>
    /// <returns>Return all pay types that meet the criteria</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet()]
    public PayTypeViewModel GetAll(int tenantId, [FromQuery] string searchText)
    {
      var result = _payTypeService.GetAll(tenantId, searchText);
      return result.ToResponse();
    }

    /// <summary>
    /// Find a pay type of a tenant by pay type id
    /// </summary>
    /// <remarks>
    /// Return a single pay type
    /// </remarks>
    /// <param name="tenantId">Tenant id of the pay type</param>
    /// <param name="id">Id of the pay type to be returned</param>
    /// <returns>Return a single pay type</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("{id:int}")]
    public PayTypeModel GetById(int tenantId, int id)
    {
      var result = _payTypeService.GetById(tenantId, id);
      return result.ToResponse();
    }

    #endregion

    #region POST

    /// <summary>
    /// Create a new pay type for a tenant
    /// </summary>
    /// <remarks>
    /// Return the newly created pay type
    /// </remarks>
    /// <param name="tenantId">Tenant id of the pay type</param>
    /// <param name="model">Pay type object that needs to be created for the tenant</param>
    /// <returns>Return the newly created pay type</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost]
    public PayTypeModel Create(int tenantId, [FromBody] PayTypeEditModel model)
    {
      var payTypeModel = new PayTypeModel
      {
        TenantId = tenantId,
        Code = model.Code,
        DisplayName = model.DisplayName,
        IsAppointmentPrivate = model.IsAppointmentPrivate,
        IsAppointmentAwayState = model.IsAppointmentAwayState,
        IsEnabled = model.IsEnabled
      };

      var result = _payTypeService.Create(payTypeModel);
      return result.ToResponse();
    }

    #endregion

    #region PUT

    /// <summary>
    /// Update an existing pay type for a tenant
    /// </summary>
    /// <remarks>
    /// Return the updated pay type
    /// </remarks>
    /// <param name="tenantId">Tenant id of the pay type</param>
    /// <param name="id">Id of the pay type to be updated</param>
    /// <param name="model">Pay type object that needs to be created for the tenant</param>
    /// <returns>Return the updated pay type</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPut("{id:int}")]
    public PayTypeModel Update(int tenantId, int id, [FromBody] PayTypeEditModel model)
    {
      var payTypeModel = new PayTypeModel
      {
        Id = id,
        TenantId = tenantId,
        Code = model.Code,
        DisplayName = model.DisplayName,
        IsAppointmentPrivate = model.IsAppointmentPrivate,
        IsAppointmentAwayState = model.IsAppointmentAwayState,
        IsEnabled = model.IsEnabled
      };

      var result = _payTypeService.Update(payTypeModel);
      return result.ToResponse();
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Delete an existing pay type on a tenant
    /// </summary>
    /// <remarks>
    /// Return whether the deleting is successed or failed
    /// </remarks>
    /// <param name="tenantId">Tenant id of the pay type</param>
    /// <param name="id">Id of the pay type to be deleted</param>
    /// <returns>Return whether the deleting is successed or failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpDelete("{id:int}")]
    public bool Delete(int tenantId, int id)
    {
      var result = _payTypeService.Delete(tenantId, id);
      return result.ToResponse();
    }

    #endregion
  }
}
