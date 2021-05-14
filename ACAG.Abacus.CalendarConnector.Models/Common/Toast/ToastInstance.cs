using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAG.Abacus.CalendarConnector.Models.Common.Toast
{
  public class ToastInstance
  {
    public Guid Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public ToastSettings ToastSettings { get; set; }
  }
}
