using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.Abacus.CalendarConnector.Models.Common;

namespace ACAG.Abacus.CalendarConnector.Logic.Services
{
  public interface ICultureService
  {
    ResultModel<List<LanguageModel>> GetAll();
  }

  public class CultureService : ICultureService
  {
    private readonly IAuthenLogicService<CultureService> _logicService;
    private readonly CacheProvider _cache;

    public CultureService(IAuthenLogicService<CultureService> logicService, CacheProvider cache)
    {
      _logicService = logicService;
      _cache = cache;
    }

    public ResultModel<List<LanguageModel>> GetAll()
    {
      var result = _logicService
        .Start()
        .ThenAuthorize(Roles.SYSADMIN, Roles.ADMINISTRATOR, Roles.USER)
        .ThenValidate(() => { return null; })
        .ThenImplement(currentUser =>
        {
          var includeTranslation = currentUser.Role == Roles.SYSADMIN || currentUser.Role == Roles.ADMINISTRATOR;
          
          var languages = _cache.Cultures.GetValues()
          .Where(t => t.IsEnabled && (includeTranslation ? true : t.DisplayName != LangConfig.CULT_TRANSLATION))
          .Select(t => new LanguageModel
          {
            Id = t.Id,
            Code = t.Code,
            DisplayName = t.DisplayName
          })
          .OrderBy(t => t.DisplayName)
          .ToList();

          return languages;
        });

      return result;
    }
  }
}
