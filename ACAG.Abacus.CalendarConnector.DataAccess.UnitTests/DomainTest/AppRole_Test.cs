using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;


namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{
    [TestClass()]
    public class AppRole_Test : TestBase
    {
        private readonly IEntityRepository<AppRole> _appRoles;
        public AppRole_Test() : base()
        {

            _appRoles = _dbContext.GetRepository<AppRole>();
        }

        [TestMethod()]
        public void Access_AppRole_ReturnOkay()
        {
            var data = _appRoles.Query().Take(10).ToList();

            Assert.IsNotNull(data);
        }
    }
}
