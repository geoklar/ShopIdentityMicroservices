using System.ComponentModel.DataAnnotations;
using Shop.Cart.Dtos;
using Shop.Cart.Models;
using Shop.Common.Models;
namespace Shop.Cart.Extensions
    {
        public static class Extensions
        {
            public static CartItemDto AsDto(this CartItem item, string name, string description, decimal price, DateTime createdDate, ApplicationUser user)
            {
                return new CartItemDto 
                { 
                    Id = item.Id,
                    CatalogId = item.CatalogId,
                    Catalog = new CatalogItem 
                    {
                        Id = item.CatalogId,
                        Name = name,
                        Description = description,
                        Price = price,
                        CreatedDate = createdDate
                    },
                    Quantity = item.Quantity,
                    AcquiredDate = item.AcquiredDate,
                    UserId = user == null ? default(Guid) : user.Id,
                    User = user
                };
            }
        }
    }