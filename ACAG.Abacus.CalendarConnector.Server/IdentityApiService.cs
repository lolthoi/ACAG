using System;
using System.Linq;
using System.Security.Claims;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;
using Microsoft.AspNetCore.Http;

namespace ACAG.Abacus.CalendarConnector.Server
{
  public class IdentityApiService : IIdentityService
  {
    public IHttpContextAccessor _contextAccessor;
    private CacheProvider _cache;

    public IdentityApiService(IHttpContextAccessor context,
      CacheProvider cache)
    {
      _cache = cache;
      _contextAccessor = context;
    }

    public LoginUserModel GetCurrentUser()
    {
      var userClaims = _contextAccessor.HttpContext.User.Claims;
      var userIdString = userClaims.Where(c => c.Type == ClaimTypes.Sid)
                   .Select(c => c.Value).SingleOrDefault();
      var tokenRoleName = userClaims.Where(t => t.Type == ClaimTypes.Role).Select(t => t.Value).SingleOrDefault();

      var isValidIdString = Int32.TryParse(userIdString, out int userId);

      var rs = new LoginUserModel
      {
        User = null
      };

      var user = _cache.Users.Get(userId);

      if (user == null)
        return rs;

      var tenant = _cache.Users.GetTenant(userId);
      var loginUser = new LoginUserModel.LoginUser
      {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        TenantId = tenant == null ? null : tenant.Id,
        CultureId = user.CultureId,
        Role = (Roles)Enum.Parse(typeof(Roles), tokenRoleName.ToString().ToUpper())
      };

      rs.User = loginUser;

      return rs;
    }
  }
}
