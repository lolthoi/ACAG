using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.Logic.Services.Base;
using Microsoft.Extensions.DependencyInjection;

namespace ACAG.Abacus.CalendarConnector.Logic.Common
{
  public static class ServiceExtensions
  {
    public static IServiceCollection AddCustomizedService<T>(this IServiceCollection services)
      where T : class, IIdentityService
    {
      services.AddSingleton<CacheProvider>();
      services.AddScoped(typeof(ILogicService<>), typeof(LogicService<>));
      services.AddScoped(typeof(IAuthenLogicService<>), typeof(AuthenLogicService<>));
      services.AddScoped<IIdentityService, T>();

      return services;
    }
  }
}
