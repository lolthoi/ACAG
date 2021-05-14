using System;
using System.Collections.Generic;
using System.Linq;

using ACAG.Abacus.CalendarConnector.Abacus.Models.V1_0;
using ACAG.Abacus.CalendarConnector.Models;
using ACAG.AbacusUtility.Logic;

namespace ACAG.Abacus.CalendarConnector.Abacus
{
  public class ReportAccessor
  {
    public static List<_ACAG_AbacusCalendarConnector> GetReportData(TenantModel tenant, AbacusSettingModel abacusSetting)
    {
      List<_ACAG_AbacusCalendarConnector> result;

      ConnectionParameters connectionParameters = new ConnectionParameters(
        abacusSetting.ServiceServerName,
        abacusSetting.ServicePort,
        abacusSetting.UseSsl,
        abacusSetting.ServiceUser,
        abacusSetting.ServicePasswordCrypted,
        tenant.Number);
       
      LoginHandler loginHandlerInstance = new LoginHandler(connectionParameters);

      AbaReportAccessorNew abaReportAccessorNew = new AbaReportAccessorNew(loginHandlerInstance);
      result = abaReportAccessorNew.GetListFromMatrixReport<_ACAG_AbacusCalendarConnector>(
        "_ACAG_AbacusCalendarConnector",
        string.Empty,
        string.Empty,
        false).ToList();

      return result;
    }
  }
}
