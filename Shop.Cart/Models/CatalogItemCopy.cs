	using System.ComponentModel.DataAnnotations.Schema;
	using Shop.Common.Models;
	namespace Shop.Cart.Models;
	[Table("CatalogItem")]
	public class CatalogItemCopy : CatalogItem
	{
	    public long CatalogId { get; set; }
}