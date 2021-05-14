using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using ACAG.Abacus.CalendarConnector.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{

    [TestClass()]
    public class TenantUserRel_Test : TestBase
    {
        private readonly IEntityRepository<TenantUserRel> _tenantUserRels;
        private readonly IEntityRepository<Tenant> _tenants;
        private readonly IEntityRepository<User> _users;

        public TenantUserRel_Test() : base()
        {

            _tenantUserRels = _dbContext.GetRepository<TenantUserRel>();
            _tenants = _dbContext.GetRepository<Tenant>();
            _users = _dbContext.GetRepository<User>();
        }

        [TestMethod()]
        public void Access_TenantUserRel_ReturnOkay()
        {
            var data = _tenantUserRels.Query().Take(10).ToList();

            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void Add_TenantUserRel_ReturnOkay()
        {
            #region Insert data

            var user = new User
            {
                FirstName = "Nam",
                LastName = "Last Name",
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

            var tenant = new Tenant
            {
                Name = "Nam",
                Description = "Description",
                Number = 1,
                ScheduleTimer = 1000,
                IsEnabled = true,
                CreatedBy = 1,
                CreatedDate = DateTime.UtcNow,
            };
            _tenants.Add(tenant);

            var tenantUserRel = new TenantUserRel
            {
                Tenant = tenant,
                User = user,
                CreatedBy = 1,
                CreatedDate = DateTime.UtcNow
            };

            _tenantUserRels.Add(tenantUserRel);

            _dbContext.Save();

            #endregion

            #region Get and check data

            var actualUser = _users.Query().FirstOrDefault(t => t.Id == user.Id);

            Assert.IsNotNull(actualUser);

            var actualTenantUserRel = actualUser.TenantUserRels.FirstOrDefault();
            Assert.IsNotNull(actualTenantUserRel);

            var actualTenant = actualTenantUserRel == null ? null : actualTenantUserRel.Tenant;
            Assert.IsNotNull(actualTenant);

            #endregion

            #region Clean data

            if (actualTenantUserRel != null)
                _tenantUserRels.Delete(actualTenantUserRel);

            if (actualTenant != null)
                _tenants.Delete(actualTenant);

            if (actualUser != null)
                _users.Delete(actualUser);

            _dbContext.Save();

            #endregion

        }
    }
}
