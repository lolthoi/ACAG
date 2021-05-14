using System;
using System.Collections.Generic;
using System.Linq;
using ACAG.Abacus.CalendarConnector.DataAccess.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ACAG.Abacus.CalendarConnector.DataAccess
{
  public class CalendarConnectorDataInitializer
  {
    public static void Initialize(IServiceProvider serviceProvider)
    {
      using (var scope = serviceProvider.CreateScope())
      {
        var context = scope.ServiceProvider.GetService<IUnitOfWork<CalendarConnectorContext>>();

        InitializeRethinkBaseData(context);
      }
    }

    private static void InitializeRethinkBaseData(IUnitOfWork<CalendarConnectorContext> context)
    {
      var createdDate = DateTime.UtcNow;

      var cultures = context.GetRepository<Culture>();
      if (!cultures.Query().Any())
      {
        var data = new List<Culture>
        {
          new Culture { Code = "en", DisplayName = "English", IsEnabled = true },
          new Culture { Code = "de", DisplayName = "German", IsEnabled = true },
          new Culture { Code = "fr", DisplayName = "France", IsEnabled = false },
          new Culture { Code = "it", DisplayName = "Italy", IsEnabled = false },
          new Culture { Code = "vi", DisplayName = "Vietnam", IsEnabled = false },
          new Culture { Code = "tr", DisplayName = "Transalation", IsEnabled = true }
        };
        cultures.InsertRange(data);
      }

      var appRoles = context.GetRepository<AppRole>();
      if (!appRoles.Query().Any())
      {
        var data = new List<AppRole>
        {
          new AppRole { Code = "Administrator", IsAdministrator = true, IsEnabled = true, CreatedBy = 1, CreatedDate = createdDate },
          new AppRole { Code = "User", IsAdministrator = false, IsEnabled = true, CreatedBy = 1, CreatedDate = createdDate },
          new AppRole { Code = "SysAdmin", IsAdministrator = true, IsEnabled = true, CreatedBy = 1, CreatedDate = createdDate}
        };
        appRoles.InsertRange(data);
      }

      var users = context.GetRepository<User>();
      if (!users.Query().Any())
      {
        var defaultCulture = cultures.Query(t => t.Code == "en").Single();

        var data = new List<User>
        {
          new User
          {
            CultureId = defaultCulture.Id,
            FirstName = "admin",
            LastName = "admin",
            Email = "develop@all-consulting.ch",
            Password = "7CD4A46E7B0F31EFBFBD1F2B5E7969EFBFBD4EEFBFBDEFBFBDEFBFBD6A6E15EFBFBDEFBFBD3DEFBFBD30D69E44EFBFBD4425", //123456
            SaltPassword ="84b32f39-a6d5-4d5a-908c-538fea22b3d9",
            IsEnabled = true,
            CreatedBy = -1,
            CreatedDate = createdDate
          },
          new User
          {
            CultureId = defaultCulture.Id,
            FirstName = "sysadmin",
            LastName = "sysadmin",
            Email = "sysadmin@all-consulting.ch",
            Password = "EFBFBDEFBFBD6955CFA1EFBFBD280438EFBFBDEFBFBD79EFBFBDEFBFBDC5B3EFBFBD2CEFBFBD3C4CEFBFBD5EEFBFBDEFBFBDEFBFBD7EEFBFBDEFBFBD", //123456
            SaltPassword ="84b32f39-a6d5-4d5a-908c-538fea22b3d9",
            IsEnabled = true,
            CreatedBy = -1,
            CreatedDate = createdDate
          }
        };
        users.InsertRange(data);
      }

      var appRoleRels = context.GetRepository<AppRoleRel>();
      if (!appRoleRels.Query().Any())
      {
        //Add admin role
        var adminRole = appRoles.Query(t => t.IsAdministrator && t.Code == "Administrator").First();
        var userAdmin = users.Query(t => t.Email == "develop@all-consulting.ch" && t.CreatedBy == -1).First();
        //Add sysadmin role
        var sysAdminRole = appRoles.Query(t => t.IsAdministrator && t.Code == "SysAdmin").First();
        var userSysAdmin = users.Query(t => t.Email == "sysadmin@all-consulting.ch").First();
        var data = new List<AppRoleRel>
        {
          new AppRoleRel { AppRoleId = adminRole.Id, UserId = userAdmin.Id , CreatedBy = 1, CreatedDate = createdDate },
          new AppRoleRel { AppRoleId = sysAdminRole.Id, UserId = userSysAdmin.Id, CreatedBy = 1, CreatedDate = createdDate}
        };
        appRoleRels.InsertRange(data);
      }
    }
  }
}

