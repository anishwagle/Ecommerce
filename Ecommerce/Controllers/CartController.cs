﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public CartController(ApplicationDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Carts.Include(c => c.Product).Include(c => c.User).ToList();
            var response = new List<CartViewModel>();
            foreach(var item in applicationDbContext)
            {
                response.Add(new CartViewModel {
                Id= item.Id,
                ProductId =item.ProductId,
                Quantity = item.Quantity,
                Product =  new ProductViewModel
                {
                    Name = item.Product.Name,
                    Price = item.Product.HasDiscount? item.Product.Price - item.Product.Price*item.Product.Discount : item.Product.Price,
                    Quantity = item.Quantity,
                }
                });
            }
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([Bind("Id","Quantity")] ProductViewModel product)
        {
            if(product == null)
            {
                return NotFound();
            }
            product.Quantity ??= 0;
            var userId = _userManager.GetUserId(User);
            var cartProd = _context.Carts.Where(x=>x.UserId== userId && x.ProductId == product.Id).FirstOrDefault();
            if(cartProd != null)
            {
                cartProd.Quantity += (int)product.Quantity;
                _context.Update(cartProd);
            }
            else
            {
                var cart = new Cart()
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = product.Id,
                    UserId = userId,
                    Quantity = (int)product.Quantity,
                };
                _context.Carts.Add(cart);
            }
            _context.SaveChanges();
            return RedirectToAction("Details","Shop",new { id = product.Id});
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart([Bind("Id", "Quantity")] ProductViewModel product)
        {
            if (product == null)
            {
                return NotFound();
            }
            product.Quantity ??= 0;
            var userId = _userManager.GetUserId(User);
            var cartProd = _context.Carts.Where(x => x.UserId == userId && x.ProductId == product.Id).FirstOrDefault();
            if (cartProd != null)
            {
                cartProd.Quantity = (int)product.Quantity;
                _context.Update(cartProd);
            }
            
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromCart(string id)
        {
            var userId = _userManager.GetUserId(User);
            var cartProd = _context.Carts.Where(x => x.UserId == userId && x.ProductId == id).FirstOrDefault();
            if (cartProd != null)
            {
                _context.Carts.Remove(cartProd);
            }
            
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
