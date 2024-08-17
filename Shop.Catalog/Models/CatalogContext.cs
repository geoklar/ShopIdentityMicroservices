	using Microsoft.EntityFrameworkCore;
	using Shop.Catalog.Models;
using Shop.Common.Models;
namespace Shop.Catalog.Models;
	public class CatalogContext : DbContext
	{
	    public CatalogContext(DbContextOptions<CatalogContext> options)
	        : base(options)
	    {
	    }
	    public DbSet<CatalogItem> CatalogItems { get; set; } = null!;
	}
