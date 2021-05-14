using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ACAG.Abacus.CalendarConnector.DataAccess.UnitTests.DomainTest
{

  [TestClass()]
  public class Tenant_Test : TestBase
  {
    private readonly IEntityRepository<Tenant> _tenants;
    public Tenant_Test() : base()
    {

      _tenants = _dbContext.GetRepository<Tenant>();
    }

    [TestMethod()]
    public void Access_Tenant_ReturnOkay()
    {
      var data = _tenants.Query().Take(10).ToList();

      Assert.IsNotNull(data);
    }

    [TestMethod()]
    public void Add_Tenant_ReturnOkay()
    {
      #region Init data

      var tenant = new Tenant
      {
        Name = "Nam",
        Description = "Description",
        Number = 1,
        ScheduleTimer = 1000,
        IsEnabled = true,
        CreatedBy = 1,
        CreatedDate = DateTime.UtcNow
      };

      try
      {
        _tenants.Add(tenant);
        _dbContext.Save();
      }
      catch (Exception ex)
      {
        Assert.Fail("Add fail");
        return;
      }

      #endregion

      #region Get and check data

      var actualTenant = _tenants.Query().FirstOrDefault(t => t.Id == tenant.Id);

      Assert.IsNotNull(actualTenant);
      Assert.AreEqual(tenant.Id, actualTenant.Id);
      Assert.AreEqual(tenant.Name, actualTenant.Name);
      Assert.AreEqual(tenant.Description, actualTenant.Description);
      Assert.AreEqual(tenant.Number, actualTenant.Number);
      Assert.AreEqual(tenant.ScheduleTimer, actualTenant.ScheduleTimer);
      Assert.AreEqual(tenant.IsEnabled, actualTenant.IsEnabled);

      #endregion

      #region Clean data

      if (actualTenant != null)
      {
        _tenants.Delete(actualTenant);
        _dbContext.Save();
      }

      #endregion
    }
  }
}
