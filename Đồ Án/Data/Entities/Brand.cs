using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Assignment.Data.Entities
{
    public class Brand
    {
        [Key]
        public required int BrandId { get; set; }
        public required string BrandName { get; set; }
    }
}
