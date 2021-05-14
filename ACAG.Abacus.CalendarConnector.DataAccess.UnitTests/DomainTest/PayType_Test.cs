using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{

    [TestClass()]
    public class PayType_Test : TestBase
    {
        private readonly IEntityRepository<PayType> _payTypes;
        public PayType_Test() : base()
        {
            _payTypes = _dbContext.GetRepository<PayType>();
        }

        [TestMethod()]
        public void Access_PayType_ReturnOkay()
        {
            var data = _payTypes.Query().Take(10).ToList();

            Assert.IsNotNull(data);
        }
    }
}