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
    public class CarouselController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CarouselController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Carousel
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Carousels.Include(c => c.Image);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Carousel/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Carousels == null)
            {
                return NotFound();
            }

            var carousel = await _context.Carousels
                .Include(c => c.Image)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carousel == null)
            {
                return NotFound();
            }

            return View(carousel);
        }

        // GET: Carousel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Carousel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Heading,SubHeading,ButtonUrl,Image")] CarouselViewModel carousel)
        {
            if(String.IsNullOrEmpty(carousel.Heading)|| String.IsNullOrEmpty(carousel.SubHeading)|| String.IsNullOrEmpty(carousel.ButtonUrl) || carousel.Image == null)
            {
                return View(carousel);
            }
            else
            {
                var carouselModel = new Carousel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Heading = carousel.Heading,
                    SubHeading = carousel.SubHeading,
                    ButtonUrl = carousel.ButtonUrl,
                };

                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath,"images");
                var uniqueName = $"{Guid.NewGuid()}_{carousel.Image.FileName}";
                var filePath = Path.Combine(imgPath, uniqueName);
                var file = new FileStream(filePath,FileMode.Create);

                await carousel.Image.CopyToAsync(file);

                file.Close();
                var img = new Image() {
                    Id = Guid.NewGuid().ToString(),
                    ImagePath = $"images/{uniqueName}"
                };
                _context.Add(img);
                carouselModel.ImageId = img.Id;
                _context.Add(carouselModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

            }
        }

        // GET: Carousel/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Carousels == null)
            {
                return NotFound();
            }

            var carousel = await _context.Carousels.FindAsync(id);
            if (carousel == null)
            {
                return NotFound();
            }
            return View(new CarouselViewModel { ButtonUrl = carousel.ButtonUrl,Heading=carousel.Heading,SubHeading=carousel.SubHeading,Id=carousel.Id});
        }

        // POST: Carousel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Heading,SubHeading,ButtonUrl,Image")] CarouselViewModel carousel)
        {
            if (id != carousel.Id)
            {
                return NotFound();
            }

            if (String.IsNullOrEmpty(carousel.Heading) || String.IsNullOrEmpty(carousel.SubHeading) || String.IsNullOrEmpty(carousel.ButtonUrl) )
            {
                return View(carousel);
            }

            var carouselModel =  await _context.Carousels.FindAsync(id);
            carouselModel.SubHeading = carousel.SubHeading;
            carouselModel.Heading= carousel.Heading;
            carouselModel.ButtonUrl= carousel.ButtonUrl;
           
            if(carousel.Image!= null)
            {
                var imgPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                var uniqueName = $"{Guid.NewGuid()}_{carousel.Image.FileName}";
                var filePath = Path.Combine(imgPath, uniqueName);
                var file = new FileStream(filePath, FileMode.Create);

                await carousel.Image.CopyToAsync(file);

                file.Close();
                var img = new Image()
                {
                    Id = Guid.NewGuid().ToString(),
                    ImagePath = $"images/{uniqueName}"
                };
                _context.Add(img);
                carouselModel.ImageId = img.Id;
            }
            _context.Update(carouselModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Carousel/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Carousels == null)
            {
                return NotFound();
            }

            var carousel = await _context.Carousels
                .Include(c => c.Image)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carousel == null)
            {
                return NotFound();
            }

            return View(carousel);
        }

        // POST: Carousel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Carousels == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Carousels'  is null.");
            }
            var carousel = await _context.Carousels.FindAsync(id);
            if (carousel != null)
            {
                _context.Carousels.Remove(carousel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarouselExists(string id)
        {
          return (_context.Carousels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
