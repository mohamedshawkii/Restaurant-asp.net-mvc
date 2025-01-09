
using Restaurant.Models.Identity;

namespace Restaurant.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? UserId { get; set; }
        public BaseUserModel? User { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
