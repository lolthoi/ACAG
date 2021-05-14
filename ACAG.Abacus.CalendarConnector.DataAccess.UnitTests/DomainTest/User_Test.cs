using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{

    [TestClass()]
    public class User_Test : TestBase
    {
        private readonly IEntityRepository<User> _users;
        public User_Test() : base()
        {

            _users = _dbContext.GetRepository<User>();
        }

        [TestMethod()]
        public void Access_User_ReturnOkay()
        {
            var data = _users.Query().ToPaginatedList(1, 10);//.Take(10).ToList();

            Assert.IsNotNull(data);
        }
        
        [TestMethod()]
        public void Add_User_ReturnOkay()
        {
            #region Init data

            var user = new User
            {
                FirstName = "Nam",
                LastName = "Nguyen",
                Email = "test@nam.nam",
                CultureId = 1,
                Password = "notencrypt",
                SaltPassword = "salt",
                Comment = "comment",
                IsEnabled = true,
                CreatedBy = 1,
                CreatedDate = DateTime.UtcNow,
            };

            _users.Add(user);
            _dbContext.Save();

            #endregion

            #region Get and check

            var actualUser = _users.Query().FirstOrDefault(t => t.Id == user.Id);

            Assert.IsNotNull(actualUser);
            Assert.AreEqual(user.Id, actualUser.Id);
            //Assert.AreEqual(user.UserName, actualUser.UserName);
            Assert.AreEqual(user.CultureId, actualUser.CultureId);
            Assert.AreEqual(user.Password, actualUser.Password);
            Assert.AreEqual(user.SaltPassword, actualUser.SaltPassword);
            Assert.AreEqual(user.Comment, actualUser.Comment);
            Assert.AreEqual(user.IsEnabled, actualUser.IsEnabled);

            #endregion

            #region Clean data

            if (actualUser != null)
            {
                _users.Delete(actualUser);
                _dbContext.Save();
            }

            #endregion

        }

        private DbContextOptionsBuilder ChangeDatabaseNameInConnectionString<TContext>()
            where TContext : DbContext
        {
            var connectionString = "Data Source=PC-025\\SQLEXPRESS;Initial Catalog=CalendarConnector;User ID=sa;Password=sa12345";

            var contextOptionsBuilder = new DbContextOptionsBuilder<TContext>();

            contextOptionsBuilder.UseSqlServer(connectionString);

            return contextOptionsBuilder;
        }
    }
}
