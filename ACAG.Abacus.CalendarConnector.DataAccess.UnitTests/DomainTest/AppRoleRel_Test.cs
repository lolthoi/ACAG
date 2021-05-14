using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{

    [TestClass()]
    public class AppRoleRel_Test : TestBase
    {
        private readonly IEntityRepository<AppRoleRel> _appRoleRels;
        public AppRoleRel_Test() : base()
        {
            _appRoleRels = _dbContext.GetRepository<AppRoleRel>();
        }

        [TestMethod()]
        public void Access_AppRoleRel_ReturnOkay()
        {
            var data = _appRoleRels.Query().Take(10).ToList();

            Assert.IsNotNull(data);
        }
    }
}
