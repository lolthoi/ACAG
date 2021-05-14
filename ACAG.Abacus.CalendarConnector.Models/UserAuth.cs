using System;

namespace ACAG.Abacus.CalendarConnector.Models
{
    public class UserAuth
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ExperiedOn { get; set; }        
    }
}
