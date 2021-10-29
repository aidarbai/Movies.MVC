using System.Collections.Generic;
using System.ComponentModel;

namespace Cinema.BLL.DTOs.User
{
    public class ApplicationUserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        [DisplayName("Firstname")]
        public string FirstName { get; set; }
        [DisplayName("Lastname")]
        public string LastName { get; set; }
        public List<string> Roles { get; set; }
        public bool IsBanned { get; set; }
    }
}
