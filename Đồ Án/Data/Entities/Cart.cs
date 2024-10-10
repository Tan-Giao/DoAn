using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment.Data.Entities
{
    [PrimaryKey("AccountId", "ProductId")]
    public class Cart
    {
        [ForeignKey("AccountId")]
        public Guid AccountId { get; set; }

        [ForeignKey("ProductId")]
        public int ProductId { get; set; }

        public required virtual Account? RefAccount { get; set; }

        public required virtual Product? RefProduct { get; set; }
    }

}