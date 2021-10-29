using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Cinema.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Comments = new HashSet<Comment>();
            Votes = new HashSet<Vote>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? BannedOnDate { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
    }
}
