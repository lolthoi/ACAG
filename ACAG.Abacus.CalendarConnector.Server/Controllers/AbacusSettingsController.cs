using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  [ApiController]
  [Route("api/Tenant/{tenantId:int}/[controller]")]
  [SwaggerTag("Abacus settings Management")]
  [Authorize]
  public class AbacusSettingsController : ControllerBase
  {
    private readonly IAbacusSettingService _abacusSettingService;

    public AbacusSettingsController(IAbacusSettingService abacusSettingService)
    {
      _abacusSettingService = abacusSettingService;
    }

    #region GET

    /// <summary>
    /// Find an abacus setting of a tenant by abacus setting id
    /// </summary>
    /// <remarks>
    /// Return a single abacus setting
    /// </remarks>
    /// <param name="tenantId">Tenant id of the abacus setting</param>
    /// <param name="id">Id of the abacus setting</param>
    /// <returns>Return a single abacus setting</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("{id:int}")]
    public AbacusSettingModel GetById(int tenantId, int id)
    {
      var result = _abacusSettingService.GetById(tenantId, id);
      return result.ToResponse();
    }

    /// <summary>
    /// Find an abacus setting by its tenant
    /// </summary>
    /// <remarks>
    /// Return a single abacus setting
    /// </remarks>
    /// <param name="tenantId">Tenant id of the abacus setting</param>
    /// <returns>Return a single abacus setting</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet()]
    public AbacusSettingModel GetByTenant(int tenantId)
    {
      var result = _abacusSettingService.GetByTenant(tenantId);
      return result.ToResponse();
    }

    #endregion

    #region POST

    /// <summary>
    /// Create a new abacus setting for a tenant
    /// </summary>
    /// <remarks>
    /// Return the newly created abacus setting
    /// </remarks>
    /// <param name="tenantId">Tenant id of the abacus setting</param>
    /// <param name="model">Abacus setting object that needs to be created for the tenant</param>
    /// <returns>Return the newly created abacus setting</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost()]
    public AbacusSettingModel Create(int tenantId, [FromBody] AbacusSettingEditModel model)
    {
      var abacusSettingModel = new AbacusSettingModel
      {
        Name = model.Name,
        ServiceUrl = model.ServiceUrl,
        ServicePort = model.ServicePort,
        ServiceUseSsl = model.ServiceUseSsl,
        ServiceUser = model.ServiceUser,
        ServiceUserPassword = model.ServiceUserPassword,
        TenantId = tenantId
      };

      var result = _abacusSettingService.Create(abacusSettingModel);
      return result.ToResponse();
    }

    /// <summary>
    /// Check connection of the provided tenant and abacus setting information
    /// </summary>
    /// <remarks>
    /// Return whether the checking is successed or failed
    /// </remarks>
    /// <param name="tenantId">Tenant id of the abacus setting</param>
    /// <param name="model">Tenant and abacus setting information that needs to be checked</param>
    /// <returns>Return whether the checking is successed or failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost("Connection")]
    public bool CheckConnection(int tenantId, [FromBody] AbacusSettingConnectionModel model)
    {
      var tenant = new TenantModel()
      {
        Id = tenantId,
        Name = model.TenantName,
        Description = model.TenantDescription,
        Number = model.TenantNumber
      };

      var abacusSetting = new AbacusSettingModel()
      {
        Name = model.AbacusSettingName,
        Description = model.AbacusSettingDescription,
        ServiceUrl = model.AbacusSettingServiceUrl,
        ServicePort = model.AbacusSettingServicePort,
        ServiceUseSsl = model.AbacusSettingServiceUseSsl,
        ServiceUser = model.AbacusSettingServiceUser,
        ServiceUserPassword = model.AbacusSettingServiceUserPassword
      };

      var result = _abacusSettingService.CheckConnection(tenant, abacusSetting);
      return result.ToResponse();
    }

    #endregion

    #region PUT

    /// <summary>
    /// Update an existing abacus setting for a tenant
    /// </summary>
    /// <remarks>
    /// Return the updated abacus setting
    /// </remarks>
    /// <param name="tenantId">Tenant id of the abacus setting</param>
    /// <param name="id">Id of the abacus setting</param>
    /// <param name="model">Abacus setting object that needs to be created for the tenant</param>
    /// <returns>Return the updated abacus setting</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPut("{id:int}")]
    public AbacusSettingModel Update(int tenantId, int id, [FromBody] AbacusSettingEditModel model)
    {
      var abacusSettingModel = new AbacusSettingModel
      {
        Id = id,
        Name = model.Name,
        ServiceUrl = model.ServiceUrl,
        ServicePort = model.ServicePort,
        ServiceUseSsl = model.ServiceUseSsl,
        ServiceUser = model.ServiceUser,
        ServiceUserPassword = model.ServiceUserPassword,
        TenantId = tenantId
      };

      var result = _abacusSettingService.Update(abacusSettingModel);
      return result.ToResponse();
    }

    #endregion

  }
}
