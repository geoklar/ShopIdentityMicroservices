	using Microsoft.EntityFrameworkCore;
	using Shop.Cart.Models;
using Shop.Common.Models;
using Shop.Cart.Dtos;
namespace Shop.Cart.Models;
	public class CartContext : DbContext
	{
	    public CartContext(DbContextOptions<CartContext> options)
	        : base(options)
	    {
	    }
	    public DbSet<CartItem> CartItems { get; set; } = null!;
}