using Assignment.Data.Entities;

namespace Assignment.Models
{
    public class Product
    {
        public int ProductId { get; }
        public required string ProductName { get; set; }
        public required int ProductQuantity { get; set; }
        public string? ProductDescription { get; set; }
        public required double ProductPrice { get; set; }
        public required string ProductImage { get; set; }

        #region Specific product details
        public string? ProductScreen { get; set; }
        public string? ProductPlatform { get; set; }
        public string? ProductCamera { get; set; }
        public string? ProductChip { get; set; }
        public string? ProductRam { get; set; }
        public string? ProductStorage { get; set; }
        public string? ProductBattery { get; set; }
        public string? ProductColor { get; set; }
        #endregion

        public required virtual  ProductBrand { get; set; }
    }
}
