using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Assignment.Data.Entities
{
    public class Account : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? Gender { get; set; }
        public int Role { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }

    }
}
