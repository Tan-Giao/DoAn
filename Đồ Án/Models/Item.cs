using Assignment.Data.Entities;

namespace Assignment.Models
{
    public class Item
    {
        public int? Amount { get; set; }
        public Product? Products { get; set; }
        public Account? Accounts { get; set; }
    }
}
