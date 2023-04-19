using Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class CarouselViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CarouselViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var carousels = _context.Carousels.Include(x=>x.Image);
            return View(carousels);
        }
    }
}
