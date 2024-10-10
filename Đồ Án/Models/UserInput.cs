using Assignment.Data.Entities;
namespace Assignment.Models
{
    public class UserInput : Account
    {
        public string? EmailOrUserName { get; set; }
        public required string Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool RememberMe { get; set; }
    }

}
