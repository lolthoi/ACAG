using System;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface IPayTypeService
  {
    ResultModel<PayTypeViewModel> GetAll(int tenantId, string searchText);
    ResultModel<PayTypeModel> GetById(int tenantId, int id);
    ResultModel<PayTypeModel> Create(PayTypeModel model);
    ResultModel<PayTypeModel> Update(PayTypeModel model);
    ResultModel<bool> Delete(int tenantId, int id);
    bool CheckConnection(PayTypeModel model);
  }

  public class PayTypeService : IPayTypeService
  {

    #region Constructor

    private readonly IAuthenLogicService<PayTypeService> _logicService;
    private readonly IUnitOfWork<CalendarConnectorContext> _dbContext;

    private readonly IEntityRepository<PayType> _payTypesRepository;
    public PayTypeService(IAuthenLogicService<PayTypeService> logicService)
    {
      _logicService = logicService;
      _dbContext = logicService.DbContext;
      _payTypesRepository = _dbContext.GetRepository<PayType>();
    }

    #endregion Constructor

    public bool CheckConnection(PayTypeModel model)
    {
      throw new NotImplementedException();
    }

    public ResultModel<PayTypeModel> Create(PayTypeModel model)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        .ThenAuthorizeTenant(model?.TenantId)
        .ThenValidate(currentUser =>
        {
          var error = ValidPaytypeModel(model);
          if (error != null)
          {
            return error;
          }

          var exists = _logicService.Cache.Tenants.GetPayTypes(model.TenantId).Where(x => x.Code == model.Code && x.TenantId == model.TenantId).Any();
          if (exists)
          {
            return new ErrorModel(ErrorType.CONFLICTED, LangKey.MSG_CODE_USED_FOR_ANOTHER_PAYTYPE);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var paytypeCreateModel = new PayType()
          {
            TenantId = model.TenantId,
            Code = model.Code,
            DisplayName = model.DisplayName,
            IsAppointmentAwayState = model.IsAppointmentAwayState,
            IsAppointmentPrivate = model.IsAppointmentPrivate,
            IsEnabled = model.IsEnabled,
            CreatedBy = currentUser.Id,
            CreatedDate = DateTime.UtcNow,
          };

          _payTypesRepository.Add(paytypeCreateModel);
          _dbContext.Save();
          _logicService.Cache.Tenants.Clear();

          model.Id = paytypeCreateModel.Id;
          return model;
        });
      return result;
    }

    public ResultModel<bool> Delete(int tenantId, int id)
    {
      PayType payTypeItem = null;
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        //.ThenAuthorizeTenant(tenantId, TenantManager.PAY_TYPE, id)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(currentUser =>
        {
          payTypeItem = _payTypesRepository.Query(x => x.Id == id && x.TenantId == tenantId).SingleOrDefault();
          if (payTypeItem == null)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_PAYTYPE_DOESNT_EXIST);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          _payTypesRepository.Delete(payTypeItem);
          int result = _dbContext.Save();
          _logicService.Cache.Tenants.Clear();

          return true;
        });
      return result;
    }

    public ResultModel<PayTypeViewModel> GetAll(int tenantId, string searchText)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(null)
        .ThenImplement(currentUser =>
        {
          var query = _logicService.Cache.Tenants.GetPayTypes(tenantId).Where(x => x.TenantId == tenantId);

          if (!String.IsNullOrEmpty(searchText))
          {
            int searchCode = 0;
            var isSearchTextNumber = Int32.TryParse(searchText, out searchCode);

            if (isSearchTextNumber)
            {
              query = query.Where(x => x.Code.ToString().Contains(searchText) || x.DisplayName.ToLower().Contains(searchText.ToLower()));
            }
            else
            {
              query = query.Where(x => x.DisplayName.ToLower().Contains(searchText.ToLower()));
            }
          }

          var payTypes = query
            .OrderBy(y => y.Code)
            .Select(x => new PayTypeModel
            {
              Id = x.Id,
              Code = x.Code,
              DisplayName = x.DisplayName,
              IsAppointmentAwayState = x.IsAppointmentAwayState,
              IsAppointmentPrivate = x.IsAppointmentPrivate,
              IsEnabled = x.IsEnabled
            })
            .ToList();

          var result = new PayTypeViewModel()
          {
            PayTypeModels = payTypes
          };
          return result;
        });
      return result;
    }

    public ResultModel<PayTypeModel> GetById(int tenantId, int id)
    {
      PayType paytype = null;
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        //.ThenAuthorizeTenant(tenantId, TenantManager.PAY_TYPE, id)
        .ThenAuthorizeTenant(tenantId)
        .ThenValidate(currentUser =>
        {
          var isExistPaytype = _logicService.Cache.Tenants.GetPayTypes(tenantId).Where(x => x.Id == id && x.TenantId == tenantId).Any();
          if (!isExistPaytype)
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_PAYTYPE_DOESNT_EXIST);

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var query = _logicService.Cache.Tenants.GetPayTypes(tenantId).Where(x => x.Id == id);

          var payTypes = query.Select(x => new PayTypeModel
          {
            Id = x.Id,
            Code = x.Code,
            DisplayName = x.DisplayName,
            IsAppointmentAwayState = x.IsAppointmentAwayState,
            IsAppointmentPrivate = x.IsAppointmentPrivate,
            IsEnabled = x.IsEnabled
          }).FirstOrDefault();

          return payTypes;
        });
      return result;
    }

    public ResultModel<PayTypeModel> Update(PayTypeModel model)
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.ADMINISTRATOR, Roles.USER, Roles.SYSADMIN)
        .ThenAuthorizeTenant(model?.TenantId)
        .ThenValidate(currentUser =>
        {
          var error = ValidPaytypeModel(model);
          if (error != null)
          {
            return error;
          }

          var isExistPayTypeItem = _logicService.Cache.Tenants.GetPayTypes(model.TenantId).Where(x => x.Id == model.Id).Any();
          if (!isExistPayTypeItem)
          {
            return new ErrorModel(ErrorType.NOT_EXIST, LangKey.MSG_PAYTYPE_DOESNT_EXIST);
          }

          var isExist = _logicService.Cache.Tenants.GetPayTypes(model.TenantId).Where(x => x.Code == model.Code && x.TenantId == model.TenantId).FirstOrDefault();
          if (isExist != null && isExist.Id != model.Id)
          {
            return new ErrorModel(ErrorType.CONFLICTED, LangKey.MSG_THIS_PAYTYPE_CODE_IS_USED_FOR_ANOTHER_PAYTYPE);
          }

          return null;
        })
        .ThenImplement(currentUser =>
        {
          var payTypeItem = _payTypesRepository.Query(x => x.Id == model.Id).FirstOrDefault();

          payTypeItem.Code = model.Code;
          payTypeItem.DisplayName = model.DisplayName;
          payTypeItem.IsAppointmentAwayState = model.IsAppointmentAwayState;
          payTypeItem.IsAppointmentPrivate = model.IsAppointmentPrivate;
          payTypeItem.IsEnabled = model.IsEnabled;
          payTypeItem.ModifiedBy = currentUser.Id;
          payTypeItem.ModifiedDate = DateTime.UtcNow;

          _payTypesRepository.Edit(payTypeItem);
          int result = _dbContext.Save();
          _logicService.Cache.Tenants.Clear();

          return model;
        });

      return result;
    }

    #region Private Method

    private ErrorModel ValidPaytypeModel(PayTypeModel model)
    {
      #region PaytypeModel

      if (model == null)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PLEASE_FILL_IN_THE_REQUIRED_FIELDS);
      }

      #endregion PaytypeModel

      #region Display Name

      if (string.IsNullOrEmpty(model.DisplayName))
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_PAYTYPE_DISPLAY_NAME_IS_REQUIRED);
      }
      if (model.DisplayName.Length < 1)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MIN_LENGTH_OF_DISPLAY_NAME_IS_1);
      }
      if (model.DisplayName.Length > 250)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MAX_LENGTH_OF_DISPLAY_NAME_IS_250);
      }

      #endregion Display Name

      #region Code

      if (model.Code < 1 || model.Code > 99999)
      {
        return new ErrorModel(ErrorType.BAD_REQUEST, LangKey.MSG_CODE_NUMBER_RANGE_IS_FROM_1_TO_99999);
      }

      #endregion Code

      return null;
    }

    #endregion Private Method
  }
}
