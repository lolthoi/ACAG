using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACAG.Abacus.CalendarConnector.Models
{
    public class AppRoleModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsEnabled { get; set; }
    }
}
