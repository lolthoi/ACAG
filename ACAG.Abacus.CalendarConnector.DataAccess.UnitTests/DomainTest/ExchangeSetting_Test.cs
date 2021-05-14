using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{

    [TestClass()]
    public class ExchangeSetting_Test : TestBase
    {
        private readonly IEntityRepository<ExchangeSetting> _exchangeSettings;
        public ExchangeSetting_Test() : base()
        {
            _exchangeSettings = _dbContext.GetRepository<ExchangeSetting>();
        }

        [TestMethod()]
        public void Access_ExchangeSetting_ReturnOkay()
        {
            var data = _exchangeSettings.Query().Take(10).ToList();

            Assert.IsNotNull(data);
        }
    }
}