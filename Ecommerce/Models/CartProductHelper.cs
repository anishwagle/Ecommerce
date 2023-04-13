namespace Ecommerce.Models
{
    public class CartProductHelper
    {
        public string Id { get; set; }
        public string CartId { get; set; } 
        public Cart Cart { get; set; }
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}
