using System.ComponentModel.DataAnnotations;

namespace ABCRetailManagement.Models
{
    public class Product
    {
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the product name.")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the product category.")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the product price.")]
        [Range(0.01, 9999999, ErrorMessage = "Price must be greater than zero.")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Please enter the stock quantity.")]
        [Range(0, 999999, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        public string? ImageName { get; set; }
    }
}