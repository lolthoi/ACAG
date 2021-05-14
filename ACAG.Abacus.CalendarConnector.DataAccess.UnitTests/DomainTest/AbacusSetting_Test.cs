using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{

    [TestClass()]
    public class AbacusSetting_Test : TestBase
    {
        private readonly IEntityRepository<AbacusSetting> _abacusSettings;
        public AbacusSetting_Test() : base()
        {

            _abacusSettings = _dbContext.GetRepository<AbacusSetting>();
        }

        [TestMethod()]
        public void Access_AbacusSetting_ReturnOkay()
        {
            var data = _abacusSettings.Query().Take(10).ToList();

            Assert.IsNotNull(data);
        }
    }
}
