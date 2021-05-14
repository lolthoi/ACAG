using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ACAG.Abacus.CalendarConnector.Server.Controllers
{
  /// <summary>
  /// Class controller
  /// </summary>
  [ApiController]
  [Route("api/[controller]")]
  [SwaggerTag("User management")]
  [Authorize]
  public class UsersController : ControllerBase
  {
    private readonly IUserService _userService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userService"></param>
    public UsersController(IUserService userService)
    {
      _userService = userService;
    }

    #region GET

    /// <summary>
    /// Find all users by search text
    /// </summary>
    /// <remarks>
    /// Return all users that meet the criteria
    /// </remarks>
    /// <param name="searchText">The criteria to filter users</param>
    /// <returns>Return all Users that meet the criteria</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet]
    public UserViewModel GetAll([FromQuery] string searchText)
    {
      var result = _userService.GetAll(searchText);
      return result.ToResponse();
    }


    /// <summary>
    /// Find a User by its id
    /// </summary>
    /// <remarks>
    /// Return a single User
    /// </remarks>
    /// <param name="userId">User id of the pay type</param>
    /// <returns>Return a single user</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpGet("{userId:int}")]
    public UserModel GetById(int userId)
    {
      var result = _userService.GetById(userId);
      return result.ToResponse();
    }

    #endregion

    #region POST

    /// <summary>
    /// Create a new User
    /// </summary>
    /// <remarks>
    /// Return the newly created User
    /// </remarks>
    /// <param name="model">User object that needs to be created</param>
    /// <returns>Return the newly created User</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPost]
    public UserModel Create([FromBody] UserModel model)
    {
      var result = _userService.Create(model, "123456");
      return result.ToResponse();
    }

    #endregion

    #region PUT

    /// <summary>
    /// Update an existing User
    /// </summary>
    /// <remarks>
    /// Return the updated User
    /// </remarks>
    /// <param name="model">User object that needs to be created</param>
    /// <returns>Return the updated user</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpPut]
    public UserModel Update([FromBody] UserModel model)
    {
      var result = _userService.Update(model);
      return result.ToResponse();
    }

    #endregion

    #region DELETE

    /// <summary>
    /// Delete an existing user
    /// </summary>
    /// <remarks>
    /// Return whether the deleting is successed or failed
    /// </remarks>
    /// <param name="userId">User id that need to be deleted</param>
    /// <returns>Return whether the deleting is successed or failed</returns>
    /// <response code="200">Successful operation</response>
    /// <response code="204">Successful operation, no content is returned</response>
    /// <response code="400">Invalid Id supplied</response>
    /// <response code="401">User is unauthorized</response>
    /// <response code="403">User is not authenticated</response>
    /// <response code="404">Request is inaccessible</response>
    /// <response code="409">Data is conflicted</response>
    /// <response code="500">Server side error</response>
    [HttpDelete("{userId:int}")]
    public bool Delete(int userId)
    {
      var result = _userService.Delete(userId);
      return result.ToResponse();
    }

    #endregion

  }

}
