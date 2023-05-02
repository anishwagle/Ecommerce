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

namespace Ecommerce.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shop
        public async Task<IActionResult> Index([FromQuery] string searchText, [FromQuery] string categoryId)
        {
            searchText ??= "";
            var productViewModel = new List<ProductViewModel>();
            if (string.IsNullOrEmpty(categoryId))
            {
                var products = _context.Products.Where(x => x.Name.ToLower().StartsWith(searchText.ToLower())).ToList();

                products.ForEach(product =>
                {
                    var categories = _context.ProductCategoryHelpers.Include(x => x.Category).Where(x => x.ProductId == product.Id).Select(x => x.Category.Name).ToList();
                    var img = _context.ImageProductHelpers.Include(x => x.Image).Where(x => x.ProductId == product.Id).Select(x => x.Image.ImagePath).ToList();

                    productViewModel.Add(new ProductViewModel
                    {
                        Name = product.Name,
                        Id = product.Id,
                        Description = product.Description,
                        Discount = product.Discount,
                        HasDiscount = product.HasDiscount,
                        Price = product.Price,
                        Categories = categories,
                        ImagePaths = img
                    });

                });
            }
            else
            {
                var catProduct = _context.ProductCategoryHelpers.Include(x => x.Product).Where(x => x.CategoryId == categoryId && x.Product.Name.ToLower().StartsWith(searchText.ToLower())).ToList();
                catProduct.ForEach(cat =>
                {
                    var categories = _context.ProductCategoryHelpers.Include(x => x.Category).Where(x => x.ProductId == cat.ProductId).Select(x => x.Category.Name).ToList();
                    var img = _context.ImageProductHelpers.Include(x => x.Image).Where(x => x.ProductId == cat.ProductId).Select(x => x.Image.ImagePath).ToList();

                    productViewModel.Add(new ProductViewModel
                    {
                        Name = cat.Product.Name,
                        Id = cat.Product.Id,
                        Description = cat.Product.Description,
                        Discount = cat.Product.Discount,
                        HasDiscount = cat.Product.HasDiscount,
                        Price = cat.Product.Price,
                        Categories = categories,
                        ImagePaths = img
                    });

                });
            }
              return View(productViewModel);
        }

        // GET: Shop/Details/5
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
                Quantity=0
            });
        }

    }
}
