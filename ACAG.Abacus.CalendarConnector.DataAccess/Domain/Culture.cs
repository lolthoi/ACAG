using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACAG.Abacus.CalendarConnector.DataAccess.Domain
{
    public class Culture
    {

        public Culture()
        {
            Users = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public bool IsEnabled { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
