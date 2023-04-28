namespace Ecommerce.ViewModels
{
    public class ProductViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } = 0;
        public bool HasDiscount { get; set; }
        public decimal Discount { get; set; }
        public List<IFormFile>? Images { get; set; }
        public List<string>? Categories { get; set; }
        public List<string>? ImagePaths { get; set; }
        public int? Quantity { get; set; }
    }
}
