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
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            return _context.Products != null ?
                        View(await _context.Products.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var category = _context.Categories;
            ViewBag.Categories = new SelectList(category, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,HasDiscount,Discount, Images, Categories")] ProductViewModel product)
        {
            if (String.IsNullOrEmpty(product.Name)||String.IsNullOrEmpty(product.Description)||product.Price == 0)
            {
              return View(product);
            }
            var productModel = new Product()
            {
                Id = Guid.NewGuid().ToString(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                HasDiscount = product.HasDiscount,
                Discount = product.Discount,
            };
            _context.Add(productModel);
            foreach(var image in product.Images)
            {
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
                var uniqueName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(imgPath, uniqueName);
                var file = new FileStream(filePath, FileMode.Create);

                await image.CopyToAsync(file);
                file.Close();
                var img = new Image()
                {
                    Id = Guid.NewGuid().ToString(),
                    ImagePath = $"Images/{uniqueName}"
                };
                _context.Add(img);
                var imgProdtHelper = new ImageProductHelper()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productModel.Id,
                    ImageId= img.Id,
                };
                _context.Add(imgProdtHelper);

            }
            foreach(var cat in product.Categories)
            {
                var prodCatHelper = new ProductCategoryHelper()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productModel.Id,
                    CategoryId=cat,


                };
                _context.Add(prodCatHelper);

            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Price,HasDiscount,Discount")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(string id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
