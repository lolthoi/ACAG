using System.Collections.Generic;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  [ApiController]
  [Route("api/Tenant/{tenantId:int}/[controller]")]
  [SwaggerTag("Log diaries management")]
  [Authorize]
  public class LogDiariesController : ControllerBase
  {
    private readonly ILogDiaryService _logDiaryService;

    public LogDiariesController(ILogDiaryService logDiaryService)
    {
      _logDiaryService = logDiaryService;
    }

    #region GET

    /// <summary>
    /// Find all log diaries by its tenant
    /// </summary>
    /// <remarks>
    /// Return all log diaries of the provided tenant
    /// </remarks>
    /// <param name="tenantId">Tenant id of the log diaries</param>
    /// <returns>Return all log diaries of the provided tenant</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet()]
    public List<LogDiaryModel> GetByTenant(int tenantId)
    {
      var result = _logDiaryService.GetByTenant(tenantId);
      return result.ToResponse();
    }

    #endregion

    #region POST

    /// <summary>
    /// Create a new log diary for a tenant
    /// </summary>
    /// <remarks>
    /// Return the newly created log diary
    /// </remarks>
    /// <param name="tenantId">Tenant id of the log diaries</param>
    /// <param name="model">Log diary object that needs to be created for the tenant</param>
    /// <returns>Return the newly created log diary</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost]
    public LogDiaryModel Create(int tenantId, [FromBody] LogDiaryEditModel model)
    {
      var logDiaryModel = new LogDiaryModel
      {
        TenantId = tenantId,
        DateTime = model.DateTime,
        Data = model.Data,
        Error = model.Error
      };

      var result = _logDiaryService.Create(logDiaryModel);
      return result.ToResponse();
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Delete a list of existing log diaries on a tenant
    /// </summary>
    /// <remarks>
    /// Return whether the deleting is successed or failed
    /// </remarks>
    /// <param name="tenantId">Tenant id of the log diary</param>
    /// <param name="ids">List id of log diaries to be deleted</param>
    /// <returns>Return whether the deleting is successed or failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpDelete("Ids")]
    public bool DisableRange(int tenantId, [FromBody] List<int> ids)
    {
      var result = _logDiaryService.DisableRange(tenantId, ids);
      return result.ToResponse();
    }

    /// <summary>
    /// Delete all existing log diaries on a tenant
    /// </summary>
    /// <remarks>
    /// Return whether the deleting is successed or failed
    /// </remarks>
    /// <param name="tenantId">Tenant id of the log diaries</param>
    /// <returns>Return whether the deleting is successed or failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpDelete()]
    public bool DeleteAll(int tenantId)
    {
      var result = _logDiaryService.DeleteAll(tenantId);
      return result.ToResponse();
    }

    #endregion
  }
}
