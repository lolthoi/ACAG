using System;
using System.Linq;
using System.Text.RegularExpressions;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches.Data;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Common;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{

  public interface IUserService
  {
    ResultModel<UserViewModel> GetAll(string searchText);
    ResultModel<UserViewModel> GetTenantUsers(int tenantId, string searchUser);
    ResultModel<UserModel> GetById(int id);
    ResultModel<UserModel> Update(UserModel userModel);
    ResultModel<UserModel> Create(UserModel userModel, string password);
    ResultModel<bool> Delete(int id);
    ResultModel<bool> ChangePassword(string oldPassword, string newPassword);
  }

  public class UserService : IUserService
  {
    private readonly IAuthenLogicService<UserService> _logicService;
    private readonly IUnitOfWork<CalendarConnectorContext> _dbContext;

    private readonly IEntityRepository<User> _users;
    private readonly IEntityRepository<TenantUserRel> _tenantUserRel;
    private readonly IEntityRepository<Tenant> _tenants;
    private readonly IEntityRepository<AppRoleRel> _appRoleRels;
    private readonly IEntityRepository<AppRole> _appRoles;

    public UserService(
        IAuthenLogicService<UserService> logicService)
    {
      _logicService = logicService;
      _dbContext = logicService.DbContext;

      _users = _dbContext.GetRepository<User>();
      _tenantUserRel = _dbContext.GetRepository<TenantUserRel>();
      _tenants = _dbContext.GetRepository<Tenant>();
      _appRoleRels = _dbContext.GetRepository<AppRoleRel>();
      _appRoles = _dbContext.GetRepository<AppRole>();
    }

    public ResultModel<bool> ChangePassword(string oldPassword, string newPassword)
    {
      User user = null;

      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenValidate(currentUser =>
        {
          if (!ValidationHelper.IsValidPasswordLength(newPassword))
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PASSWORD_IS_REQUIRED_MIN_LENGHT_IS_6_MAX_LENGHT_IS_100);
          }

          user = _users.Query(x => x.Id == currentUser.Id).FirstOrDefault();
          if (user == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_CAN_NOT_IDENTITY_USER);
          }

          if (!user.IsEnabled)
          {
            return new ErrorModel(ErrorType.CONFLICTED, LangKey.MSG_CAN_NOT_IDENTITY_USER);
          }

          var encryptedPassword = Utils.EncryptedPassword(oldPassword, user.SaltPassword);
          if (user.Password != encryptedPassword)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_OLD_PASSWORD_NOT_MATCHED);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var encryptedPassword = Utils.EncryptedPassword(newPassword, user.SaltPassword);
          user.Password = encryptedPassword;
          user.ModifiedBy = currentUser.Id;
          user.ModifiedDate = DateTime.UtcNow;

          _dbContext.Save();

          return true;

        });
      return result;
    }

    public ResultModel<UserModel> Create(UserModel userModel, string password)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR)
        .ThenValidate(currentUser =>
        {
          var error = ValidateUser(userModel);
          if (error != null)
          {
            return error;
          }

          var email = userModel.Email.Trim();

          var hasSameEmail = _logicService.Cache
            .Users
            .GetValues()
            .Any(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

          if (hasSameEmail)
          {
            return new ErrorModel(ErrorType.DUPLICATED, LangKey.MSG_THE_ENTERED_EMAIL_ALREADY_EXISTS);
          }

          if (userModel.AppRole.IsAdministrator)
          {
            if (userModel.Tenant != null)
              if (userModel.Tenant.Id != 0)
                return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.IF_THE_USER_IS_AN_ADMINISTRATOR_THE_TENANT_MUST_BE_EMPTY);
          }
          else
          {
            var invalidTenant = ValidateTenant(userModel.Tenant);
            if (invalidTenant != null)
            {
              return invalidTenant;
            }
          }

          var invalidRole = ValidateRole(userModel, currentUser);
          if (invalidRole != null)
          {
            return invalidRole;
          }

          var invalidLanguage = ValidateLanguage(userModel.Language);
          if (invalidLanguage != null)
          {
            return invalidLanguage;
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var salt = Guid.NewGuid().ToString();
          var now = DateTime.UtcNow;

          var user = new User()
          {
            Email = userModel.Email.Trim().ToLower(),
            FirstName = userModel.FirstName.Trim(),
            LastName = userModel.LastName.Trim(),
            Comment = userModel.Comment,
            CreatedBy = currentUser.Id,
            CreatedDate = now,
            IsEnabled = userModel.IsEnabled,
            Password = Utils.EncryptedPassword(password, salt),
            SaltPassword = salt,
            CultureId = userModel.Language.Id
          };

          AppRoleRel appRoleRel = new AppRoleRel()
          {
            User = user,
            AppRoleId = userModel.AppRole.Id,
            CreatedBy = currentUser.Id,
            CreatedDate = now
          };
          _users.Add(user);

          _appRoleRels.Add(appRoleRel);

          if (!userModel.AppRole.IsAdministrator && userModel.Tenant != null)
          {
            TenantUserRel tenantUserRel = new TenantUserRel()
            {
              User = user,
              TenantId = userModel.Tenant.Id,
              CreatedBy = currentUser.Id,
              CreatedDate = now
            };
            _tenantUserRel.Add(tenantUserRel);
          }

          int result = _dbContext.Save();

          userModel.Id = user.Id;

          _logicService.Cache.Users.Clear();
          return userModel;

        });
      return result;
    }

    public ResultModel<bool> Delete(int id)
    {
      User user = null;
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR)
        .ThenValidate(currentUser =>
        {
          user = _users.Query(x => x.Id == id).FirstOrDefault();
          if (user == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          }

          //DELETE YOUR SELF
          if (currentUser.Id == id)
          {
            return new ErrorModel(ErrorType.CONFLICTED, LangKey.MSG_CANT_DELETE_YOURSELF);
          }
          //DELETE USER WITH ROLE SYS-ADMIN
          var isDeleteSysAdmin = _logicService.Cache.Users.GetRole(id).Code.ToString().ToUpper() == Roles.SYSADMIN.ToString() ? true : false;
          if (isDeleteSysAdmin)
          {
            return new ErrorModel(ErrorType.CONFLICTED, LangKey.MSG_CANT_DELETE_SYSADMIN);
          }
          //DELETE USER WITH ROLE ADMIN
          var isDeleteAdminWithEqualOrLowerRoles = _logicService.Cache.Users.GetRole(id).Code.ToString().ToUpper() == Roles.ADMINISTRATOR.ToString() &&
                              !string.Equals(currentUser.Role.ToString(), Roles.SYSADMIN.ToString())
                              ? true : false;
          if (isDeleteAdminWithEqualOrLowerRoles)
          {
            return new ErrorModel(ErrorType.CONFLICTED, LangKey.MSG_CANT_DELETE_ROLE_WITH_EQUAL_OR_LOWER_ROLE);
          }
          return null;
        })
        .ThenImplement(() =>
        {
          var tenantUserRels = _tenantUserRel.Query(x => x.UserId == id).ToList();
          if (tenantUserRels != null || tenantUserRels.Count > 0)
          {
            _tenantUserRel.DeleteRange(tenantUserRels);
          }

          var appRoleRels = _appRoleRels.Query(x => x.UserId == id).ToList();
          if (tenantUserRels != null || tenantUserRels.Count > 0)
          {
            _appRoleRels.DeleteRange(appRoleRels);
          }

          _users.Delete(user);
          int result = _dbContext.Save();
          _logicService.Cache.Users.Remove(id);
          return true;

        });
      return result;
    }

    public ResultModel<UserViewModel> GetAll(string searchText)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR,Roles.SYSADMIN)
        .ThenValidate(currentUser => null)
        .ThenImplement(currentUser =>
        {
          var userQuery = _logicService.Cache
          .Users
          .GetValues()
          .AsEnumerable();

          if (!string.IsNullOrWhiteSpace(searchText))
          {
            userQuery = userQuery.Where(t => 
                   t.FirstName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                || t.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                || t.Email.Contains(searchText, StringComparison.OrdinalIgnoreCase));
          }

          var users = userQuery
            .OrderBy(t => t.Email)
            .Select(t =>
            {
              var role = _logicService.Cache.Users.GetRole(t.Id);
              var tenant = _logicService.Cache.Users.GetTenant(t.Id);

              return new UserModel
              {
                Id = t.Id,
                Email = t.Email,
                FirstName = t.FirstName,
                LastName = t.LastName,
                IsEnabled = t.IsEnabled,
                RoleName = role == null ? string.Empty : role.Code,
                TenantName = tenant == null ? string.Empty : tenant.Name,
                Comment = t.Comment
              };
            })
            .OrderBy(t => t.FirstName)
            .ToList();

          //Admin roles unable to see user sysadmin
          users = currentUser.Role == Roles.ADMINISTRATOR ? users.Where(t => t.RoleName.ToUpper() != Roles.SYSADMIN.ToString()).ToList() : users;

          var userViewModel = new UserViewModel
          {
            Users = users
          };

          return userViewModel;
        });
      return result;
    }

    public ResultModel<UserModel> GetById(int id)
    {
      UserMD user = null;
      bool isShowPasswordButton = false;
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenAuthorizeUser(id)
        .ThenValidate(currentUser =>
        {
          user = _logicService.Cache.Users.Get(id);
          if (user == null)
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          return null;
        })
        .ThenImplement(currentUser =>
        {
          var appRole = _logicService.Cache.Users.GetRole(user.Id);
          var tenant = _logicService.Cache.Users.GetTenant(user.Id);
          var language = _logicService.Cache.Cultures.GetValues().FirstOrDefault(l => l.Id == user.CultureId);       
          DateTime currentNow = DateTime.UtcNow;

          if (user.IsEnabled)
          {
            isShowPasswordButton = true;
          }        

          var data = new UserModel
          {
            Id = user.Id,
            Email = user.Email,
            Comment = user.Comment,
            CultureId = user.CultureId,
            IsEnabled = user.IsEnabled,
            FirstName = user.FirstName,
            LastName = user.LastName,
            AppRole = appRole == null ? null : new AppRoleModel()
            {
              Id = appRole.Id,
              Code = appRole.Code,
              IsAdministrator = appRole.IsAdministrator
            },
            Tenant = tenant == null ? null : new TenantModelForUser()
            {
              Id = tenant.Id,
              Name = tenant.Name
            },
            Language = language == null ? null : new LanguageModel()
            {
              Id = language.Id,
              DisplayName = language.DisplayName
            },
            IsShowSendPasswordButton = isShowPasswordButton
          };
          if (currentUser.Id == data.Id)
          {
            data.CanDelete = false;
          }

          //IF CURRENT USER IS ADMIN => CANT DELETE USER ADMIN ROLE ( BUTTON DELETE DISABLE)
          if ( currentUser.Role == Models.Common.Roles.ADMINISTRATOR
          && _logicService.Cache.Users.GetRole(id).Code.ToString().ToUpper() == Models.Common.Roles.ADMINISTRATOR.ToString())
          {
            data.CanDelete = false;
          }
          //CURRENT USER IS SYSADMIN => CANT DELETE SYSADMIN ROLE ( BUTTON DELETE DISABLE)
          if (currentUser.Role == Models.Common.Roles.SYSADMIN 
          && _logicService.Cache.Users.GetRole(id).Code.ToString().ToUpper() == Models.Common.Roles.SYSADMIN.ToString())
          {
            data.CanDelete = false;
          }
          return data;
        });
      return result;
    }

    public ResultModel<UserViewModel> GetTenantUsers(int tenantId, string searchUser)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(currentUser => null)
        .ThenImplement(currentUser =>
        {
          var query = _logicService.Cache
            .Tenants
            .GetUsers(tenantId)
            .AsEnumerable();

          if (!string.IsNullOrWhiteSpace(searchUser))
          {
            query = query.Where(
                   t => t.FirstName.Contains(searchUser, StringComparison.OrdinalIgnoreCase)
                || t.LastName.Contains(searchUser, StringComparison.OrdinalIgnoreCase)
                || t.Email.Contains(searchUser, StringComparison.OrdinalIgnoreCase));
          }

          var record = query
          .OrderBy(x => x.FirstName)
          .Select(u => new UserModel
          {
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsEnabled = u.IsEnabled
          })
            .ToList();

          UserViewModel userViewModel = new UserViewModel() { Users = record };
          return userViewModel;
        });

      return result;
    }

    public ResultModel<UserModel> Update(UserModel userModel)
    {
      User user = null;
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR)
        .ThenAuthorizeUser(userModel?.Id)
        .ThenValidate(currentUser =>
        {
          var error = ValidateUser(userModel);
          if (error != null)
          {
            return error;
          }

          if (userModel.Id <= 0)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.ID_DOES_NOT_MATCH);
          }
          if (userModel.AppRole.IsAdministrator)
          {
            if (userModel.Tenant != null)
              if (userModel.Tenant.Id != 0)
                return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.IF_THE_USER_IS_AN_ADMINISTRATOR_THE_TENANT_MUST_BE_EMPTY);           
          }
          else
          {
            var invalidTenant = ValidateTenant(userModel.Tenant);
            if (invalidTenant != null)
            {
              return invalidTenant;
            }
          }

          var canBeDeactiveError = CanBeDeactive(userModel, currentUser);
          if(canBeDeactiveError != null)
          {
            return canBeDeactiveError;
          }

          var invalidRole = ValidateRole(userModel, currentUser);
          if (invalidRole != null)
          {
            return invalidRole;
          }

          var invalidLanguage = ValidateLanguage(userModel.Language);
          if (invalidLanguage != null)
          {
            return invalidLanguage;
          }

          user = _users.Query(x => x.Id == userModel.Id).FirstOrDefault();
          if (user == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID_);
          }

          if (user.Email != userModel.Email)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, "");
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          user.IsEnabled = userModel.IsEnabled;
          user.FirstName = userModel.FirstName;
          user.LastName = userModel.LastName;
          user.Comment = userModel.Comment;
          user.ModifiedBy = currentUser.Id;
          user.ModifiedDate = DateTime.UtcNow;
          user.CultureId = userModel.Language.Id;
          userModel.Email = user.Email;
          _users.Edit(user);

          var appRoles = _appRoleRels.Query(a => a.UserId == user.Id).FirstOrDefault();
          if (appRoles != null)
          {
            appRoles.AppRoleId = userModel.AppRole.Id;
            _appRoleRels.Edit(appRoles);
          }
          else
          {
            _appRoleRels.Add(new AppRoleRel
            {
              UserId = user.Id,
              AppRoleId = userModel.AppRole.Id
            });
          }

          if (!userModel.AppRole.IsAdministrator && userModel.Tenant != null && userModel.Tenant.Id > 0)
          {
            var tenanlUserRel = _tenantUserRel.Query(t => t.UserId == user.Id).FirstOrDefault();
            if (tenanlUserRel != null)
            {
              tenanlUserRel.TenantId = userModel.Tenant.Id;
              tenanlUserRel.ModifiedBy = currentUser.Id;
              tenanlUserRel.ModifiedDate = DateTime.UtcNow;
              _tenantUserRel.Edit(tenanlUserRel);
            }
            else
            {
              Tenant tenant = _tenants.Query(x => x.Id == userModel.Tenant.Id).FirstOrDefault();
              if (tenant != null)
              {
                tenanlUserRel = new TenantUserRel()
                {
                  Tenant = tenant,
                  User = user,
                  CreatedBy = currentUser.Id,
                  CreatedDate = DateTime.Now
                };
                _tenantUserRel.Add(tenanlUserRel);
              }
            }
          }
          else
          {
            var tenanlUserRel = _tenantUserRel.Query(tenanlUserRels => tenanlUserRels.UserId == user.Id).FirstOrDefault();
            if (tenanlUserRel != null)
            {
              _tenantUserRel.Delete(tenanlUserRel);
            }
          }

          int result = _dbContext.Save();
          _logicService.Cache.Users.Clear();
          userModel.Id = user.Id;
          return userModel;
        });
      return result;
    }

    private ErrorModel ValidateUser(UserModel userModel)
    {
      #region UserModel

      if (userModel == null)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
      }

      #endregion

      #region FirstName

      if (string.IsNullOrEmpty(userModel.FirstName))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_FIRST_NAME_IS_REQUIRED);
      }
      if (userModel.FirstName.Length < 3)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MIN_LENGTH_OF_FIRST_NAME_IS_3);
      }
      if (userModel.FirstName.Length > 50)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_FIRST_NAME_IS_50);
      }

      #endregion

      #region LastName

      if (string.IsNullOrEmpty(userModel.LastName))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_LAST_NAME_IS_REQUIRED);
      }
      if (userModel.LastName.Length < 3)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MIN_LENGTH_OF_LAST_NAME_IS_3);
      }
      if (userModel.LastName.Length > 50)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_LAST_NAME_IS_50);
      }

      #endregion

      #region Email

      if (string.IsNullOrEmpty(userModel.Email))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_EMAIL_IS_REQUIRED);
      }
      if (userModel.Email.Length > 75)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_MAX_LENGTH_OF_EMAIL_IS_75);
      }
      if (ValidationHelper.IsValidEmail(userModel.Email) == false)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS);
      }

      #endregion

      return null;
    }

    private ErrorModel ValidateTenant(TenantModelForUser tenantModel)
    {
      if (tenantModel == null)
      {
        return null;
      }
      else
      {
        if (tenantModel.Id < 0)
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
        }

        if (tenantModel.Id == 0)
        {
          return null;
        }

        var tenant = _logicService.Cache.Tenants.Get(tenantModel.Id);
        if (tenant == null)
        {
          return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
        }

        return null;
      }
    }

    private ErrorModel ValidateRole(UserModel userModel, LoginUserModel.LoginUser currentUser)
    {
      if(userModel.AppRole == null)
      {
        return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
      }
      else
      {
        if (userModel.AppRole.Id <= 0)
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
        }

        AppRoleMD appRole;
        if(userModel.Id != 0)
        {
          appRole = _logicService.Cache.Users.GetRole(userModel.Id);
        }
        else
        {
          appRole = _logicService.Cache.AppRoles.Get(userModel.AppRole.Id);
        }

        if (appRole == null)
        {
          return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
        }

        // Current user can change himself
        if(userModel.Id != currentUser.Id)
        {
          if (currentUser.Role == Roles.SYSADMIN)
          {
            if(appRole.Code.ToUpper() == Roles.SYSADMIN.ToString())
            {
              return new ErrorModel(ErrorType.NO_ROLE, LangKey.MSG_USER_NOT_IN_ROLE);
            }
          }
          else if (currentUser.Role == Roles.ADMINISTRATOR)
          {
            if (appRole.Code.ToUpper() == Roles.SYSADMIN.ToString() 
              || appRole.Code.ToUpper() == Roles.ADMINISTRATOR.ToString())
            {
              return new ErrorModel(ErrorType.NO_ROLE, LangKey.MSG_USER_NOT_IN_ROLE);
            }
          }
        }

        return null;
      }
    }

    private ErrorModel ValidateLanguage(LanguageModel languageModel)
    {
      if(languageModel == null)
      {
        return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
      }
      else
      {
        if (languageModel.Id <= 0)
        {
          return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_NO_RECORD_FOUND_WITH_ID);
        }

        var language = _logicService.Cache.Cultures.GetValues()
                        .Where(l => l.Id == languageModel.Id && l.IsEnabled)
                        .FirstOrDefault();

        if (language == null)
        {
          return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_TENANT_NOT_EXIST_OR_DISABLED);
        }

        return null;
      }
    }

    private ErrorModel CanBeDeactive(UserModel appUser, LoginUserModel.LoginUser currentUser)
    {
      // Current user can not deactivate himself
      if(appUser.Id == currentUser.Id
        && !appUser.IsEnabled)
      {
        return new ErrorModel(ErrorType.NO_ROLE, LangKey.MSG_USER_NOT_IN_ROLE);
      }

      return null;
    }
  }
}
