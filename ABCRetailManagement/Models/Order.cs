using System.ComponentModel.DataAnnotations;

namespace ABCRetailManagement.Models
{
    public class Order
    {
        public string OrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a customer.")]
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a product.")]
        public string ProductId { get; set; } = string.Empty;

        [Range(1, 999999, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}