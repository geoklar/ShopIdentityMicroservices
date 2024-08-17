using System.ComponentModel.DataAnnotations;
using Shop.Cart.Models;
using Shop.Common.Models;
namespace Shop.Cart.Dtos;
public class CartItemDto : CartItem
{
    public ApplicationUser User { get; set; }
    public CatalogItem Catalog { get; set; }
}