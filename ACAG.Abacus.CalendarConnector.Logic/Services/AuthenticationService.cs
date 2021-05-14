using System;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Common;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface IAuthenticationService
  {
    ResultModel<UserAuthModel> GetByEmail(string email);

    ResultModel<UserAuthModel> Login(string email, string password);

    ResultModel<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel model);
    ResultModel<ResetPasswordModel> ResetPassword(ResetPasswordModel resetPasswordModel);

    ResultModel<bool> CheckCodeForResetPassword(string code);
  }

  public class AuthenticationService : IAuthenticationService
  {
    private readonly IUnitOfWork<CalendarConnectorContext> _dbContext;

    private readonly IEntityRepository<User> _users;
    private readonly IEntityRepository<TenantUserRel> _tenantUserRels;
    private readonly IEntityRepository<AppRole> _approles;
    private readonly IEntityRepository<AppRoleRel> _appRoleRels;
    private readonly IEntityRepository<Culture> _cultures;

    private readonly ILogicService<AuthenticationService> _logicService;


    public AuthenticationService(
      ILogicService<AuthenticationService> logicService)
    {
      _logicService = logicService;

      _dbContext = _logicService.DbContext;

      _users = _dbContext.GetRepository<User>();
      _tenantUserRels = _dbContext.GetRepository<TenantUserRel>();
      _approles = _dbContext.GetRepository<AppRole>();
      _appRoleRels = _dbContext.GetRepository<AppRoleRel>();
      _cultures = _dbContext.GetRepository<Culture>();
    }

    public ResultModel<ForgotPasswordModel> ForgotPassword(ForgotPasswordModel model)
    {
      User user = null;
      LanguageModel language = null;
      DateTime currentNow = DateTime.UtcNow;
      var result = _logicService
        .Start()
        .ThenValidate(() =>
        {
          if (model == null)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
          }

          if (string.IsNullOrEmpty(model.Email) || !ValidationHelper.IsValidEmail(model.Email))
          {

            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS);
          }

          user = _users
          .Query(t => t.Email == model.Email)
          .SingleOrDefault();

          if (user == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_THE_ENTERED_EMAIL_DOES_NOT_EXIST);
          }

          if (!user.IsEnabled)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_THIS_USER_DOES_NOT_EXIST_OR_IS_DEACTIVATED);
          }

          if ( user.ExpireTime != null)
          {
            var expireTime = user.ExpireTime.Value.AddMinutes(Contains.TimeSendMail);
            int data = DateTime.Compare(expireTime, currentNow);
            if (data > 0)
            {
              return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_SENDING_EMAIL_FOR_FORGOT_PASSWORD_CAN_TAKE_MORE_THAN_10_MINUTES);
            }
          }

          #region Not allow user forgot password.

          //var role = _logicService.Cache.Users.GetRole(user.Id);

          //if (!role.IsAdministrator)
          //{
          //  var tenant = _logicService.Cache.Users.GetTenant(data.User.Id);
          //  if (tenant == null || !tenant.IsEnabled)
          //  {
          //    return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_THIS_USER_DOES_NOT_EXIST_OR_IS_DEACTIVATED);
          //  }

          //  data.User.TenantId = tenant.Id;
          //}
          #endregion

          return null;
        })
        .ThenImplement(() =>
        {
          string idLinkResetPassword = user.Id + "_Guid-" + Guid.NewGuid().ToString();
          language = _cultures.Query(c => c.IsEnabled && c.Id == user.CultureId)
            .Select(t => new LanguageModel()
            {
              Code = t.Code
            })
          .FirstOrDefault();
          user.ResetCode = idLinkResetPassword;
          user.ExpireTime = currentNow;
          _users.Edit(user);
          _dbContext.Save();

          return new ForgotPasswordModel()
          {
            Code = idLinkResetPassword,
            Email = user.Email,
            Url = model.Url,
            IsSuccess = true,
            LanguageCode = language == null ? "en-US" : language.Code
          };
        });
      return result;
    }

    public ResultModel<UserAuthModel> GetByEmail(string email)
    {
      var result = _logicService
        .Start()
        .ThenValidate(() =>
        {
          if (string.IsNullOrEmpty(email) || !ValidationHelper.IsValidEmail(email))
          {

            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS);
          }

          var user = _users
          .Query(t => t.IsEnabled && t.Email == email)
          .SingleOrDefault();

          if (user == null)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS);
          }

          #region Not allow user deactive login.

          //var role = _logicService.Cache.Users.GetRole(user.Id);

          //if (!role.IsAdministrator)
          //{
          //  var tenant = _logicService.Cache.Users.GetTenant(data.User.Id);
          //  if (tenant == null || !tenant.IsEnabled)
          //  {
          //    return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_THIS_USER_DOES_NOT_EXIST_OR_IS_DEACTIVATED);
          //  }

          //  data.User.TenantId = tenant.Id;
          //}
          #endregion

          return null;
        })
        .ThenImplement(() =>
        {
          var user = _users
          .Query(t => t.IsEnabled && t.Email == email)
          .Select(t => new UserAuthModel
          {
            Id = t.Id,
            Email = t.Email,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Password = t.Password,
            PasswordSalt = t.SaltPassword,
            CultureId = t.CultureId
          })
          .SingleOrDefault();

          if (user == null)
          {
            return null;
          }

          var role = _logicService.Cache.Users.GetRole(user.Id);
          user.Role = role == null ? string.Empty : role.Code;

          return user;
        });

      return result;
    }

    public ResultModel<UserAuthModel> Login(string email, string password)
    {
      UserAuthModel user = null;

      var result = _logicService
        .Start()
        .ThenValidate(() =>
        {
          if (!ValidationHelper.IsValidEmail(email))
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_THE_EMAIL_FIELD_IS_NOT_A_VALID_EMAIL_ADDRESS);
          }

          var data = _users
            .Query(t => t.Email.Equals(email))
            .Select(t => new
            {
              User = new UserAuthModel
              {
                Id = t.Id,
                Email = t.Email,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Password = t.Password,
                CultureId = t.CultureId
              },
              SaltPassword = t.SaltPassword,
              IsEnabled = t.IsEnabled
            })
            .FirstOrDefault();

          if (data == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_CAN_NOT_IDENTITY_USER);
          }

          if (!data.IsEnabled)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_THIS_USER_DOES_NOT_EXIST_OR_IS_DEACTIVATED);
          }

          var encryptedPassword = Utils.EncryptedPassword(password, data.SaltPassword);
          if (data.User.Password != encryptedPassword)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_CAN_NOT_IDENTITY_USER);
          }

          var role = _logicService.Cache.Users.GetRole(data.User.Id);
          data.User.Role = role.Code;

          var tenantId = _tenantUserRels.Query(t => t.UserId == data.User.Id).Select(t => t.TenantId).FirstOrDefault();
          data.User.TenantId = tenantId == 0 ? data.User.TenantId : tenantId;

          #region Not allow user deactive login.

          //if (!role.IsAdministrator)
          //{
          //  var tenant = _logicService.Cache.Users.GetTenant(data.User.Id);
          //  if (tenant == null || !tenant.IsEnabled)
          //  {
          //    return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_THIS_USER_DOES_NOT_EXIST_OR_IS_DEACTIVATED);
          //  }

          //  data.User.TenantId = tenant.Id;
          //}
          #endregion

          user = data.User;
          return null;
        })
        .ThenImplement(() =>
        {
          return user;
        });

      return result;
    }

    public ResultModel<ResetPasswordModel> ResetPassword(ResetPasswordModel resetPasswordModel)
    {
      User user = null;

      var result = _logicService
        .Start()
        .ThenValidate(() =>
        {
          if (resetPasswordModel == null)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
          }

          if (resetPasswordModel.Code == null)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_CAN_NOT_IDENTITY_USER);
          }

          if (string.IsNullOrEmpty(resetPasswordModel.Password))
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PASSWORD_IS_REQUIRED);
          }

          if (resetPasswordModel.Password.Length < 6)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PASSWORD_IS_REQUIRED_MIN_LENGHT_IS_6_MAX_LENGHT_IS_100);
          }
          if (resetPasswordModel.Password.Length > 100)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PASSWORD_IS_REQUIRED_MIN_LENGHT_IS_6_MAX_LENGHT_IS_100);
          }

          if (string.IsNullOrEmpty(resetPasswordModel.ConfirmPassword))
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, "");
          }

          if (resetPasswordModel.ConfirmPassword != resetPasswordModel.Password)
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_THE_PASSWORD_AND_CONFIRMATION_PASSWORD_DO_NOT_MATCH);
          }

          string userId = resetPasswordModel.Code.Split('_')[0];

          if (int.TryParse(userId, out int id))
          {
            user = _users.Query(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
              return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_CAN_NOT_IDENTITY_USER);
            }

            if (!user.IsEnabled)
            {
              return new ErrorModel(ErrorType.CONFLICTED, LangKey.MSG_CAN_NOT_IDENTITY_USER);
            }
            if (user.ResetCode == null)
            {
              return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_CAN_NOT_IDENTITY_USER);
            }

            if (user.ResetCode != resetPasswordModel.Code)
            {
              return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_CAN_NOT_IDENTITY_USER);
            }

            var current = DateTime.UtcNow;
            var expiredAdd = user.ExpireTime.Value.AddMinutes(Contains.TimeSendMail);
            int data = DateTime.Compare(current, expiredAdd);
            if (data > 0)
            {
              return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_CAN_NOT_IDENTITY_USER);
            }
          }
          else
          {
            return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.RESET_PASSWORD_THE_RESET_CODE_IS_NOT_VALID);
          }

          return null;
        })
        .ThenImplement(() =>
        {

          var salt = Guid.NewGuid().ToString();
          user.Password = Utils.EncryptedPassword(resetPasswordModel.Password, salt);
          user.ResetCode = null;
          user.ExpireTime = null;
          user.SaltPassword = salt;
          user.ModifiedBy = user.Id;
          user.ModifiedDate = DateTime.UtcNow;
          _dbContext.Save();

          return new ResetPasswordModel()
          {
            Status = true
          };
        });
      return result;
    }

    public ResultModel<bool> CheckCodeForResetPassword(string code)
    {
      var result = _logicService
        .Start()
        .ThenValidate(() =>
        {
          return null;
        })
        .ThenImplement(() =>
        {
          if (string.IsNullOrEmpty(code))
          {
            return false;
          }
          string userId = code.Split('_')[0];
          if (int.TryParse(userId, out int id))
          {
            var user = _users.Query(x => x.Id == id).FirstOrDefault();
            if (user == null && user.ResetCode == null && user.ResetCode != code && user.ExpireTime == null)
            {
              return false;
            }

            var current = DateTime.UtcNow;
            var expiredAdd = user.ExpireTime.Value.AddMinutes(Contains.TimeSendMail);
            int data = DateTime.Compare(expiredAdd, current);
            if (data > 0)
            {
              return true;
            }
            return false;
          }
          else
          {
            return false;
          }
        });
      return result;
    }
  }
}
