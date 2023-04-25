using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ViewComponents
{
    public class NavCategoryViewComponent: ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public NavCategoryViewComponent(ApplicationDbContext context)
        {
            _context = context;
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var category = _context.Categories.Include(x => x.Image);
            return View(category);
        }
    
    }
}
