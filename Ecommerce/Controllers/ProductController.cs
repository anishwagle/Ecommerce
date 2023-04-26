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
using System.Xml.Linq;
using Microsoft.AspNetCore.Hosting;

namespace Ecommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductController> _logger;


        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<ProductController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
              return _context.Products != null ? 
                          View(await _context.Products.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Products'  is null.");
        }

        // GET: Product/Details/5
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

            var selectedCategories = _context.ProductCategoryHelpers.Include(x=>x.Category).Where(x => x.ProductId == product.Id).Select(x => x.Category.Name).ToList();
            var category = _context.Categories;
            var selectedImgs = _context.ImageProductHelpers.Include(x=>x.Image).Where(x=>x.ProductId == product.Id).Select(x=>x.Image.ImagePath).ToList();
            return View(new ProductViewModel()
            {
                Id = id,
                Categories = selectedCategories,
                ImagePaths = selectedImgs,
                Description = product.Description,
                Discount = product.Discount,
                HasDiscount = product.HasDiscount,
                Name = product.Name,
                Price = product.Price,
            });

        }

        // GET: Product/Create
        public IActionResult Create()
        {
            var category = _context.Categories;
            ViewBag.Categories = new SelectList(category,"Id","Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,HasDiscount,Discount,Images,Categories")] ProductViewModel product)
        {
            if(String.IsNullOrEmpty(product.Name)|| String.IsNullOrEmpty(product.Description)|| product.Price == 0)
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
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var uniqueName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(imgPath, uniqueName);
                var file = new FileStream(filePath, FileMode.Create);

                await image.CopyToAsync(file);

                file.Close();
                var img = new Image()
                {
                    Id = Guid.NewGuid().ToString(),
                    ImagePath = $"images/{uniqueName}"
                };
                _context.Add(img);
                var imgProdHelper = new ImageProductHelper()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productModel.Id,
                    ImageId = img.Id
                };
                _context.Add(imgProdHelper);
            }

            foreach(var cat in product.Categories)
            {
                var prodCatHelper = new ProductCategoryHelper()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productModel.Id,
                    CategoryId = cat
                };
                _context.Add(prodCatHelper);
            }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: Product/Edit/5
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
            var selectedCategories = _context.ProductCategoryHelpers.Where(x=>x.ProductId == product.Id).Select(x=>x.CategoryId).ToList();
            var category = _context.Categories;
            ViewBag.Categories = new SelectList(category, "Id", "Name",selectedCategories);
            _logger.LogDebug("Aded Successfully");
            return View(new ProductViewModel()
            {
                Id = id,
                Categories=selectedCategories,
                Description=product.Description,
                Discount=product.Discount,
                HasDiscount=product.HasDiscount,
                Name = product.Name,
                Price= product.Price,
            });
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Price,HasDiscount,Discount,Images,Categories")] ProductViewModel product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(product.Name) || String.IsNullOrEmpty(product.Description) || product.Price == 0)
            {
                return View(product);
            }
            var productModel = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productModel == null) {
                return NotFound();
            }

            productModel.Name = product.Name;
            productModel.Description = product.Description;
            productModel.Price = product.Price;
            productModel.HasDiscount = product.HasDiscount;
            productModel.Discount = product.Discount;
            _context.Update(productModel);
            if(product.Images != null)
            {
                var imgs = _context.ImageProductHelpers.Where(x=>x.ProductId== product.Id);
                _context.ImageProductHelpers.RemoveRange(imgs);
                foreach (var image in product.Images)
                {
                    var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var uniqueName = $"{Guid.NewGuid()}_{image.FileName}";
                    var filePath = Path.Combine(imgPath, uniqueName);
                    var file = new FileStream(filePath, FileMode.Create);

                    await image.CopyToAsync(file);

                    file.Close();
                    var img = new Image()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ImagePath = $"images/{uniqueName}"
                    };
                    _context.Add(img);
                    var imgProdHelper = new ImageProductHelper()
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = productModel.Id,
                        ImageId = img.Id
                    };
                    _context.Add(imgProdHelper);
                }
            }
            
            var cats = _context.ProductCategoryHelpers.Where(x=>x.ProductId== product.Id);
            _context.ProductCategoryHelpers.RemoveRange(cats);
            foreach (var cat in product.Categories)
            {
                var prodCatHelper = new ProductCategoryHelper()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = productModel.Id,
                    CategoryId = cat
                };
                _context.Add(prodCatHelper);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Delete/5
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
            var selectedCategories = _context.ProductCategoryHelpers.Include(x => x.Category).Where(x => x.ProductId == product.Id).Select(x => x.Category.Name).ToList();
            var category = _context.Categories;
            var selectedImgs = _context.ImageProductHelpers.Include(x => x.Image).Where(x => x.ProductId == product.Id).Select(x => x.Image.ImagePath).ToList();
            return View(new ProductViewModel()
            {
                Id = id,
                Categories = selectedCategories,
                ImagePaths = selectedImgs,
                Description = product.Description,
                Discount = product.Discount,
                HasDiscount = product.HasDiscount,
                Name = product.Name,
                Price = product.Price,
            });
        }

        // POST: Product/Delete/5
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
                var imgs = _context.ImageProductHelpers.Where(x => x.ProductId == product.Id);
                _context.ImageProductHelpers.RemoveRange(imgs);
                var cats = _context.ProductCategoryHelpers.Where(x => x.ProductId == product.Id);
                _context.ProductCategoryHelpers.RemoveRange(cats);
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
