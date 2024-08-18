using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Cart.Dtos;
using Shop.Cart.Extensions;
using Shop.Cart.Models;
using Shop.Common.Clients;
using Shop.Common.Models;

namespace Shop.Cart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private const string AdminRole = "Admin";
        private readonly CartContext _context;
        private readonly IHttpShopClient<CatalogItem> _httpContextAccessor;
        private readonly IHttpShopClient<ApplicationUser> _httpContextAccessorUser;

        public CartController(CartContext context,IHttpShopClient<CatalogItem> httpContextAccessor, IHttpShopClient<ApplicationUser> httpContextAccessorUser)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            // _httpContextAccessorUser = httpContextAccessorUser;
        }

        // GET: api/Cart
        [HttpGet]
        [Authorize(Policies.Read)]
        public async Task<ActionResult<IEnumerable<CartItemDto>>>GetCartItems(Guid userId)
        {
            var currentUserId = User.FindFirstValue("sub");
            if (userId != Guid.Parse(currentUserId) && !User.IsInRole(AdminRole))
            {
                return Unauthorized();
            }

            var catalogItemEntities = await _httpContextAccessor.GetItemsAsync("https://localhost:7078", "/api/Catalog");
            var cartItemEntities = await _context.CartItems.Where(item => item.UserId == userId).ToListAsync();
            var itemIds = cartItemEntities.Select(item => item.CatalogId);
            // var user = await _httpContextAccessorUser.GetItemAsync("https://localhost:7078", $"/api/user/{userId}");
            var inventoryItemDtos = cartItemEntities.Select(cartItem =>
            {
                var catalogItem = catalogItemEntities.Single(catalogItem => catalogItem.Id == cartItem.CatalogId);
                return cartItem.AsDto(catalogItem.Name, catalogItem.Description, catalogItem.Price, catalogItem.CreatedDate, new ApplicationUser { Id = userId});
            });

            return Ok(inventoryItemDtos);
        }

        // GET: api/Cart/5
        [HttpGet("{id}")]
        [Authorize(Policies.Read)]
        public async Task<ActionResult<CartItemDto>> GetCartItemDto(long id)
        {
            if (id == default(long))
            {
                return BadRequest();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            var catalogItem = await _httpContextAccessor.GetItemAsync("https://localhost:7078", $"/api/Catalog/{cartItem.CatalogId}");
            if (cartItem == null)
            {
                return NotFound();
            }

            return new CartItemDto {
                Id = cartItem.Id,
                Catalog = catalogItem,
                CatalogId = cartItem.CatalogId,
                UserId = cartItem.UserId,
                User = null,
                Quantity = cartItem.Quantity,
                AcquiredDate = cartItem.AcquiredDate
            };
        }

        // PUT: api/Cart/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policies.Write)]
        public async Task<IActionResult> PutCartItemDto(long id, CartItem cartItemDto)
        {
            if (id != cartItemDto.Id)
            {
                return BadRequest();
            }

            var currentUserId = User.FindFirstValue("sub");
            if (cartItemDto.UserId != Guid.Parse(currentUserId) && !User.IsInRole(AdminRole))
            {
                return Unauthorized();
            }

            _context.Entry(cartItemDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cart
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Policies.Write)]
        public async Task<ActionResult<CartItem>> PostCartItemDto(CartItem cartItemDto)
        {
            var currentUserId = User.FindFirstValue("sub");
            if (cartItemDto.UserId != Guid.Parse(currentUserId) && !User.IsInRole(AdminRole))
            {
                return Unauthorized();
            }
            _context.CartItems.Add(cartItemDto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCartItemDto", new { id = cartItemDto.Id }, cartItemDto);
        }

        // DELETE: api/Cart/5
        [HttpDelete("{id}")]
        [Authorize(Policies.Write)]
        public async Task<IActionResult> DeleteCartItemDto(long id)
        {
            var currentUserId = User.FindFirstValue("sub");
            var cartItemDto = await _context.CartItems.FindAsync(id);
            if (cartItemDto == null)
            {
                return NotFound();
            }

            if (cartItemDto.UserId != Guid.Parse(currentUserId) && !User.IsInRole(AdminRole))
            {
                return Unauthorized();
            }

            _context.CartItems.Remove(cartItemDto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartItemExists(long id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }
    }
}
