using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Models
{
    public class Cart
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

    }
}
