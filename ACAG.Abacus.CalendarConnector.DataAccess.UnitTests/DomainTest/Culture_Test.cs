using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{

    [TestClass()]
    public class Culture_Test : TestBase
    {
        private readonly IEntityRepository<Culture> _cultures;
        public Culture_Test() : base()
        {
            _cultures = _dbContext.GetRepository<Culture>();
        }

        [TestMethod()]
        public void Access_Culture_ReturnOkay()
        {
            var data = _cultures.Query().Take(10).ToList();

            Assert.IsNotNull(data);
        }
    }
}