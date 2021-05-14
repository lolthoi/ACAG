using System.Linq;
using ACAG.Abacus.CalendarConnector.Logic.Common;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface IAppSettingService
  {
    ResultModel<AppFooterModel> GetAll();
  }
  public class AppSettingService : IAppSettingService
  {
    private readonly ILogicService<AppSettingService> _logicService;
    public AppSettingService(ILogicService<AppSettingService> logicService)
    {
      _logicService = logicService;
    }

    public ResultModel<AppFooterModel> GetAll()
    {
      var result = _logicService
        .Start()
        .ThenValidate(() => null)
        .ThenImplement(() =>
        {
          var footerSetting = _logicService.Cache
          .AppSettings
          .GetValues()
          .FirstOrDefault(x => x.Status == true && x.Id == Utils.FOOTER);
          if (footerSetting != null)
          {
            var footerSettingModel = Utils.StringToObject<AppFooterModel>(footerSetting.Value);

            return footerSettingModel;
          }
          return null;
        });
      return result;
    }
  }
}
