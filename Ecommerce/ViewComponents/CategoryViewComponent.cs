using Ecommerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public CategoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var carousels = _context.Categories.Include(x=>x.Image);
            return View(carousels);
        }
    }
}
