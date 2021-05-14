using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAG.Abacus.CalendarConnector.Models.Common.Toast
{
  public class ToastSettings
  {
    public ToastSettings(string _heading, string _message, string _baseClass, string _additionalClasses, string _iconClass)
    {
      Heading = _heading;
      Message = _message;
      BaseClass = _baseClass;
      AdditionalClasses = _additionalClasses;
      IconClass = _iconClass;
    }

    public string Heading { get; set; }
    public string Message { get; set; }
    public string BaseClass { get; set; }
    public string AdditionalClasses { get; set; }
    public string IconClass { get; set; }

  }
}
