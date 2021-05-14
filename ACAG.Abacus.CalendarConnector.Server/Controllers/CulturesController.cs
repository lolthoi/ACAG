using System.Collections.Generic;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [SwaggerTag("Cultures management")]
  [Authorize]
  public class CulturesController : ControllerBase
  {
    private readonly ICultureService _cultureService;

    public CulturesController(ICultureService cultureService)
    {
      _cultureService = cultureService;
    }

    #region GET

    /// <summary>
    /// Find all cultures that satisfy logged-in role
    /// </summary>
    /// <remarks>
    /// Return all cultures that satisfy logged-in role
    /// </remarks>
    /// <returns>Return all cultures that satisfy logged-in role</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet]
    public List<LanguageModel> GetAll()
    {
      var result = _cultureService.GetAll();
      return result.ToResponse();
    }

    #endregion
  }
}
