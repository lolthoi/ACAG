using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using ACAG.Abacus.CalendarConnector.Models.Abacus.V1_0;
using ACAG.AbacusUtility.Logic;

namespace ACAG.Abacus.CalendarConnector.Abacus.Controllers
{
  [RoutePrefix("api/testconnection")]
  [Authorize(Roles = "admin, user")]
  public class TestConnectionController : AuthenticatedController
  {
    [Route("")]
    public string Get([FromBody] TenantModel tenant)
    {
      try
      {
        string result = string.Empty;

        ConnectionParameters connectionParameters = new ConnectionParameters(
          tenant.AbacusSetting.ServiceServerName,
          tenant.AbacusSetting.ServicePort,
          tenant.AbacusSetting.UseSsl,
          tenant.AbacusSetting.ServiceUser,
          tenant.AbacusSetting.ServicePasswordCrypted,
          tenant.Number);

        LoginHandler loginHandlerInstance = new LoginHandler(connectionParameters);

        result = AbacusTestConnectBo.TestConnection(loginHandlerInstance);

        return  result;
      }
      catch (Exception ex)
      {
        throw new BusinessException(System.Net.HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}