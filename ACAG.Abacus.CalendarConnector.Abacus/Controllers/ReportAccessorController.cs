using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;
using ACAG.AbacusUtility.Logic;

namespace ACAG.Abacus.CalendarConnector.Abacus.Controllers
{
  [RoutePrefix("api/reportaccessor")]
  [Authorize(Roles = "admin, user")]
  public class ReportAccessorController : AuthenticatedController
  {
    [Route("")]
    public IEnumerable<_ACAG_AbacusCalendarConnector> Get([FromBody] TenantModel tenant)
    {
      try
      {
        var result = new List<_ACAG_AbacusCalendarConnector>();

        ConnectionParameters connectionParameters = new ConnectionParameters(
          tenant.AbacusSetting.ServiceServerName,
          tenant.AbacusSetting.ServicePort,
          tenant.AbacusSetting.UseSsl,
          tenant.AbacusSetting.ServiceUser,
          tenant.AbacusSetting.ServicePasswordCrypted,
          tenant.Number);

        LoginHandler loginHandlerInstance = new LoginHandler(connectionParameters);

        AbaReportAccessorNew abaReportAccessorNew = new AbaReportAccessorNew(loginHandlerInstance);

        List<_ACAG_AbacusCalendarConnector> list = abaReportAccessorNew.GetListFromMatrixReport<_ACAG_AbacusCalendarConnector>(
          "_ACAG_AbacusCalendarConnector",
          string.Empty,
          string.Empty,
          false).ToList();

        return  list;
      }
      catch (Exception ex)
      {
        throw new BusinessException(System.Net.HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}