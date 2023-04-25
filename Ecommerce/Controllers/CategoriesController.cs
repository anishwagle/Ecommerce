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
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Categories.Include(c => c.Image);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Categories/Details/5
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

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Color, Size, Image")] CategoryViewModel category)
        {
            if (String.IsNullOrEmpty(category.Name) || String.IsNullOrEmpty(category.Color) || String.IsNullOrEmpty(category.Size) || category.Image == null)
            {
                return View(category);
            }
            else
            {
                var categoryModel = new Category()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = category.Name,
                    Color = category.Color,
                    Size= category.Size,
                };
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Color", "Size");
                var uniqueName = $"{Guid.NewGuid()}_{category.Image.FileName}";
                var filePath = Path.Combine(imgPath, uniqueName);
                var file = new FileStream(filePath, FileMode.Create);
               
                await category.Image.CopyToAsync(file);
                file.Close();
                var img = new Image()
                {
                    Id = Guid.NewGuid().ToString(),
                    ImagePath = $"Images/{uniqueName}"

                };
                _context.Add(img);
                categoryModel.ImageId = img.Id;
                _context.Add(categoryModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }

        // GET: Categories/Edit/5
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
            return View(new CategoryViewModel { Id= category.Id,Name= category.Name, Color=category.Color, Size=category.Size});
           
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Color,Size,Image")] CategoryViewModel category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(category.Name) || String.IsNullOrEmpty(category.Color) || String.IsNullOrEmpty(category.Size))
            {
                return View(category);
            }
            var categoryModel = await _context.Categories.FindAsync(id);
            if (categoryModel == null)
            {
                return NotFound();

            }
            categoryModel.Name = category.Name;
            categoryModel.Color = category.Color;
            categoryModel.Size = category.Size;
            if(category.Image != null)
            {
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                var uniqueName = $"{Guid.NewGuid()}_{category.Image.FileName}";
                var filePath = Path.Combine(imgPath, uniqueName);
                var file = new FileStream(filePath, FileMode.Create);

                await category.Image.CopyToAsync(file);
                file.Close();
                var img = new Image()
                {
                    Id = Guid.NewGuid().ToString(),
                    ImagePath = $"Images/{uniqueName}"
                };
                _context.Add(img);
                categoryModel.ImageId = img.Id;
            }
            _context.Update(categoryModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Categories/Delete/5
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

        // POST: Categories/Delete/5
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
