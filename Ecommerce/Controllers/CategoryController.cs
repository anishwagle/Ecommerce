using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Hosting;

namespace Ecommerce.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public CategoryController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Categories.Include(c => c.Image);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Image)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image")] CategoryViewModel category)
        {
            if (String.IsNullOrEmpty(category.Name)|| category.Image == null)
            {
                return View(category);
            }
            var categoryModel = new Category()
            {
                Id = Guid.NewGuid().ToString(),
                Name = category.Name,
            };
            var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            var uniqueName = $"{Guid.NewGuid()}_{category.Image.FileName}";
            var filePath = Path.Combine(imgPath, uniqueName);
            var file = new FileStream(filePath, FileMode.Create);

            await category.Image.CopyToAsync(file);

            file.Close();
            var img = new Image()
            {
                Id = Guid.NewGuid().ToString(),
                ImagePath = $"images/{uniqueName}"
            };
            _context.Add(img);
            categoryModel.ImageId = img.Id;
            _context.Add(categoryModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(new CategoryViewModel { Id = category.Id,Name=category.Name});
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Image")] CategoryViewModel category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }
            if (String.IsNullOrEmpty(category.Name) )
            {
                return View(category);
            }
            var categorylModel = await _context.Categories.FindAsync(id);
            if (categorylModel == null)
            {
                return NotFound();
            }
            categorylModel.Name = category.Name;

            if (category.Image != null)
            {
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var uniqueName = $"{Guid.NewGuid()}_{category.Image.FileName}";
                var filePath = Path.Combine(imgPath, uniqueName);
                var file = new FileStream(filePath, FileMode.Create);

                await category.Image.CopyToAsync(file);

                file.Close();
                var img = new Image()
                {
                    Id = Guid.NewGuid().ToString(),
                    ImagePath = $"images/{uniqueName}"
                };
                _context.Add(img);
                categorylModel.ImageId = img.Id;
            }
            _context.Update(categorylModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.Image)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(string id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
