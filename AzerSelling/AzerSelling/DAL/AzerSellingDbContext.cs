using AzerSelling.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AzerSelling.DAL;
public class AzerSellingDbContext: IdentityDbContext
{
	public AzerSellingDbContext(DbContextOptions<AzerSellingDbContext> options) :base(options) { }

	public DbSet<Category> Categories { get; set; }
	public DbSet<Company> Companies { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<ProductImage> ProductImages { get; set; }
	public DbSet<AppUser> AppUsers { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }
	public DbSet<Order> Orders { get; set; }
}