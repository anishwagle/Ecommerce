using Ecommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Carousel>Carousels { get; set; }
        public DbSet<Cart>Carts { get; set; }
        public DbSet<CartProductHelper>CartProductHelpers { get; set; }
        public DbSet<Category>Categories { get; set; }
        public DbSet<Image>Images { get; set; }
        public DbSet<ImageProductHelper>ImageProductHelpers { get; set; }
        public DbSet<Product>Products { get; set; }
        public DbSet<ProductCategoryHelper>ProductCategoryHelpers { get; set; }
    }
}