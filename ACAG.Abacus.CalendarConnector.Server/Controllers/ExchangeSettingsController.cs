using System.Collections.Generic;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  // TODO: change Model to EditModel

  [ApiController]
  [Route("api/Tenant/{tenantId:int}/[controller]")]
  [SwaggerTag("Exchange settings Management")]
  [Authorize]
  public class ExchangeSettingsController : ControllerBase
  {
    private readonly IExchangeSettingService _exchangeSettingService;

    public ExchangeSettingsController(IExchangeSettingService exchangeSettingService)
    {
      _exchangeSettingService = exchangeSettingService;
    }

    #region GET

    /// <summary>
    /// Find all existing exchange settings of a tenant by search text
    /// </summary>
    /// <remarks>
    /// Return all existing exchange settings that meet the criteria
    /// </remarks>
    /// <param name="tenantId">Tenant id of the Exchange Setting</param>
    /// <param name="searchText">The criteria to filter Exchange Setting</param>
    /// <returns>Return all existing exchange settings</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet]
    public ExchangeSettingViewModel GetAll(int tenantId, [FromQuery] string searchText)
    {
      var result = _exchangeSettingService.GetAll(tenantId, searchText);
      return result.ToResponse();
    }

    /// <summary>
    /// Find an exchange setting of a tenant by exchange setting id
    /// </summary>
    /// <remarks>
    /// Return a single exchange setting
    /// </remarks>
    /// <param name="tenantId">Tenant id of the exchange setting</param>
    /// <param name="id">Id of the exchange setting</param>
    /// <returns>Return a single exchange setting</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("{id:int}")]
    public ExchangeSettingModel GetById(int tenantId, int id)
    {
      var result = _exchangeSettingService.GetById(tenantId, id);
      return result.ToResponse();
    }

    /// <summary>
    /// Find all existing exchange version
    /// </summary>
    /// <remarks>
    /// Return all existing exchange version
    /// </remarks>
    /// <returns>Return all existing exchange version</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("/api/[controller]/ExchangeVersions")]
    public List<ExchangeVersionModel> ExchangeVersions()
    {
      var result = _exchangeSettingService.GetAllExchangeVersions();
      return result;
    }

    /// <summary>
    /// Find all existing login types
    /// </summary>
    /// <remarks>
    /// Return all existing login types
    /// </remarks>
    /// <returns>Return all existing login types</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("/api/[controller]/LoginTypes")]
    public List<ExchangeLoginTypeModel> LoginTypes()
    {
      var result = _exchangeSettingService.GetExchangeLoginTypes();
      return result;
    }

    #endregion

    #region POST

    /// <summary>
    /// Create a new exchange setting for a tenant
    /// </summary>
    /// <remarks>
    /// Return the newly created exchange setting
    /// </remarks>
    /// <param name="tenantId">Tenant id of the exchange setting</param>
    /// <param name="model">Exchange setting object that needs to be created for the tenant</param>
    /// <returns>Return the newly created exchange setting</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost()]
    public ExchangeSettingModel Create(int tenantId, [FromBody] ExchangeSettingEditModel model)
    {
      var exchangeSetting = new ExchangeSettingModel
      {
        Name = model.Name,
        AzureClientId = model.AzureClientId,
        AzureClientSecret = model.AzureClientSecret,
        AzureTenant = model.AzureTenant,
        Description = model.Description,
        EmailAddress = string.IsNullOrEmpty(model.EmailAddress) ? null : model.EmailAddress.Trim().ToLower(),
        ExchangeUrl = model.ExchangeUrl,
        ExchangeVersionModel = model.ExchangeVersionModel == null ? null : new ExchangeVersionModel()
        {
          Id = model.ExchangeVersionModel.Id,
          Name = model.ExchangeVersionModel.Name
        },
        ExchangeLoginTypeModel = model.ExchangeLoginTypeModel == null ? null : new ExchangeLoginTypeModel()
        {
          Id = model.ExchangeLoginTypeModel.Id,
          Name = model.ExchangeLoginTypeModel.Name,
          Type = model.ExchangeLoginTypeModel.Type
        },
        HealthStatus = model.HealthStatus,
        IsEnabled = model.IsEnabled,
        LoginType = model.ExchangeLoginTypeModel.Id,
        ServiceUser = model.ServiceUser,
        ServiceUserPassword = model.ServiceUserPassword,
        TenantId = tenantId
      };

      var result = _exchangeSettingService.Create(exchangeSetting);
      return result.ToResponse();
    }

    #endregion

    #region PUT

    /// <summary>
    /// Check connection of the provided tenant and exchange setting information
    /// </summary>
    /// <remarks>
    /// Return whether the checking is successed or failed
    /// </remarks>
    /// <param name="tenantId">Tenant id of the exchange setting</param>
    /// <param name="model">Tenant and exchange setting information that needs to be checked</param>
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
    public string CheckConnection(int tenantId, [FromBody] ExchangeSettingEditModel model)
    {
      var exchangeSetting = new ExchangeSettingModel
      {
        Id = model.Id,
        Name = model.Name,
        AzureClientId = model.AzureClientId,
        AzureClientSecret = model.AzureClientSecret,
        AzureTenant = model.AzureTenant,
        Description = model.Description,
        EmailAddress = string.IsNullOrEmpty(model.EmailAddress) ? null : model.EmailAddress.Trim().ToLower(),
        ExchangeUrl = model.ExchangeUrl,
        ExchangeVersionModel = model.ExchangeVersionModel == null ? null : new ExchangeVersionModel()
        {
          Id = model.ExchangeVersionModel.Id,
          Name = model.ExchangeVersionModel.Name
        },
        ExchangeLoginTypeModel = model.ExchangeLoginTypeModel == null ? null : new ExchangeLoginTypeModel()
        {
          Id = model.ExchangeLoginTypeModel.Id,
          Name = model.ExchangeLoginTypeModel.Name,
          Type = model.ExchangeLoginTypeModel.Type
        },
        HealthStatus = model.HealthStatus,
        IsEnabled = model.IsEnabled,
        LoginType = model.ExchangeLoginTypeModel.Id,
        ServiceUser = model.ServiceUser,
        ServiceUserPassword = model.ServiceUserPassword,
        TenantId = tenantId,
        ExchangeVersion = model.ExchangeVersion
      };

      var result = _exchangeSettingService.CheckConnection(exchangeSetting);
      return result.ToResponse();
    }

    /// <summary>
    /// Update an existing exchange setting for a tenant
    /// </summary>
    /// <remarks>
    /// Return the updated exchange setting
    /// </remarks>
    /// <param name="tenantId">Tenant id of the exchange setting</param>
    /// <param name="id">Id of the exchange setting</param>
    /// <param name="model">Exchange setting object that needs to be created for the tenant</param>
    /// <returns>Return the updated exchange setting</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPut("{id:int}")]
    public ExchangeSettingModel Update(int tenantId, int id, [FromBody] ExchangeSettingEditModel model)
    {
      var exchangeSetting = new ExchangeSettingModel
      {
        Id = model.Id,
        Name = model.Name,
        AzureClientId = model.AzureClientId,
        AzureClientSecret = model.AzureClientSecret,
        AzureTenant = model.AzureTenant,
        Description = model.Description,
        EmailAddress = string.IsNullOrEmpty(model.EmailAddress) ? null : model.EmailAddress.Trim().ToLower(),
        ExchangeUrl = model.ExchangeUrl,
        ExchangeVersionModel = model.ExchangeVersionModel == null ? null : new ExchangeVersionModel()
        {
          Id = model.ExchangeVersionModel.Id,
          Name = model.ExchangeVersionModel.Name
        },
        ExchangeLoginTypeModel = model.ExchangeLoginTypeModel == null ? null : new ExchangeLoginTypeModel()
        {
          Id = model.ExchangeLoginTypeModel.Id,
          Name = model.ExchangeLoginTypeModel.Name,
          Type = model.ExchangeLoginTypeModel.Type
        },
        HealthStatus = model.HealthStatus,
        IsEnabled = model.IsEnabled,
        LoginType = model.ExchangeLoginTypeModel.Id,
        ServiceUser = model.ServiceUser,
        ServiceUserPassword = model.ServiceUserPassword,
        TenantId = tenantId
      };

      var result = _exchangeSettingService.Update(exchangeSetting);
      return result.ToResponse();
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Delete an existing exchange setting on a tenant
    /// </summary>
    /// <remarks>
    /// Return whether the deleting is successed or failed
    /// </remarks>
    /// <param name="tenantId">Tenant id of the exchange setting</param>
    /// <param name="id">Id of the exchange setting</param>
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
      var result = _exchangeSettingService.Delete(tenantId, id);
      return result.ToResponse();
    }

    #endregion
  }
}
