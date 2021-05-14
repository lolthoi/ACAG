using System.Linq;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface IRoleService
  {
    ResultModel<AppRoleViewModel> GetAll();
  }

  //public class RoleService : IRoleService
  //{
  //  private readonly ILogicService<RoleService> _logicService;

  //  public RoleService(ILogicService<RoleService> logicService)
  //  {
  //    _logicService = logicService;
  //  }

  //  public ResultModel<AppRoleViewModel> GetAll()
  //  {
  //    var result = _logicService
  //      .Start()
  //      .ThenValidate(() => null)
  //      .ThenImplement(() =>
  //      {
  //        var roles = _logicService.Cache.AppRoles.GetValues()
  //        .Select(ar => new AppRoleModel
  //        {
  //          Id = ar.Id,
  //          Code = ar.Code,
  //          IsAdministrator = ar.IsAdministrator,
  //        })
  //        .ToList();

  //        var appRoleViewModel = new AppRoleViewModel
  //        {
  //          Roles = roles
  //        };

  //        return appRoleViewModel;
  //      });
  //    return result;
  //  }
  //}

  #region Get roles by role
  public class RoleService : IRoleService
  {
    private readonly IAuthenLogicService<RoleService> _logicService;

    public RoleService(IAuthenLogicService<RoleService> logicService)
    {
      _logicService = logicService;
    }

    public ResultModel<AppRoleViewModel> GetAll()
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenValidate(() => { return null; })
        .ThenImplement(currentUser =>
        {
          var rolesQuery = _logicService.Cache.AppRoles.GetValues()
          .Where(r => r.Code.ToUpper() != Roles.SYSADMIN.ToString());

          if (currentUser.Role.ToString() == Roles.ADMINISTRATOR.ToString())
          {
            rolesQuery = rolesQuery.Where(r => r.Code.ToUpper() != Roles.ADMINISTRATOR.ToString());
          }
          else if (currentUser.Role.ToString() == Roles.USER.ToString())
          {
            rolesQuery = rolesQuery.Where(r => (r.Code.ToUpper() != Roles.ADMINISTRATOR.ToString())
                                    && (r.Code.ToUpper() != Roles.USER.ToString()));
          }

          var roles = rolesQuery.Select(ar => new AppRoleModel
          {
            Id = ar.Id,
            Code = ar.Code,
            IsAdministrator = ar.IsAdministrator,
          })
          .ToList();

          var appRoleViewModel = new AppRoleViewModel
          {
            Roles = roles
          };

          return appRoleViewModel;
        });

      return result;
    }
  }
  #endregion
}
