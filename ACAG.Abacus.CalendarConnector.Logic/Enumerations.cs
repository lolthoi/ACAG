using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAG.Abacus.CalendarConnector.Logic
{
  public enum EnumAppointmentState
  {
    Error = -1,
    New = 0,
    Snchronised = 1,
    NeedsInsert = 2,
    NeedsUpdate = 3
  }
}
