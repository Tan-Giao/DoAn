using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Assignment.Data.Entities
{
    public class Category
    {
        [Key]
        public required int CategoryId { get; set; }
        public required string CategoryName { get; set; }
    }
}
