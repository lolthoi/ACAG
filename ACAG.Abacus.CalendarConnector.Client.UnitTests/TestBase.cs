using ACAG.Abacus.CalendarConnector.DataAccess;
using ACAG.Abacus.CalendarConnector.DataAccess.Caches;
using ACAG.Abacus.CalendarConnector.Logic.Services;
using ACAG.Abacus.CalendarConnector.WebApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ACAG.Abacus.CalendarConnector.WebApp.UnitTests
{
  public abstract class TestBase
    {
        protected IUnitOfWork<CalendarConnectorContext> _dbContext;
        protected CacheProvider _cacheProvider;
        protected ITenantService _tenantService;
        protected IUserService _userService;
        protected IRoleService _roleService;
        private IServiceScope _scope;
        public TestBase()
        {
            var startup = Startup.Instance;

            _scope = startup.ServiceProvider.CreateScope();

            _tenantService = _scope.ServiceProvider.GetService<ITenantService>();
            _userService = _scope.ServiceProvider.GetService<IUserService>();
            _roleService = _scope.ServiceProvider.GetService<IRoleService>();

            _dbContext = _scope.ServiceProvider.GetService<IUnitOfWork<CalendarConnectorContext>>();
        }
    }
}
