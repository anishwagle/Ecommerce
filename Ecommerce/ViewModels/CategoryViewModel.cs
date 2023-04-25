namespace Ecommerce.ViewModels
{
    public class CategoryViewModel
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public IFormFile? Image { get; set; }
    }
}
