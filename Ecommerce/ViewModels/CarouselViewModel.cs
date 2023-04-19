namespace Ecommerce.ViewModels
{
    public class CarouselViewModel
    {
        public string Id { get; set; }
        public string Heading { get; set; }
        public string SubHeading { get; set; }
        public string ButtonUrl { get; set; }
        public IFormFile? Image { get; set; }
    }
}
