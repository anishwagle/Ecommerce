using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models
{
    public class Order
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        public OrderStatus Status { get; set; }
        public string BillingAddressId { get; set; }
        public Address BillingAddress { get; set; }
        public List<OrderItem> Items { get; set;}
    }

    public enum OrderStatus
    {
        Pending,
        Apporved,
        Cancel,
        Complete
    }
}
