using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Cart.Dtos;
using Shop.Cart.Models;
using Shop.Common.Clients;
using Shop.Common.Models;

namespace Shop.Cart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly CartContext _context;
        private readonly IHttpShopClient<CatalogItem> _httpContextAccessor;
        private readonly IHttpShopClient<ApplicationUser> _httpContextAccessorUser;

        public CartController(CartContext context,IHttpShopClient<CatalogItem> httpContextAccessor, IHttpShopClient<ApplicationUser> httpContextAccessorUser)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessorUser = httpContextAccessorUser;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCartItemDto()
        {
            var catalogItems = await _httpContextAccessor.GetItemsAsync("https://localhost:7078", "/api/Catalog");
            return await _context.CartItems.Select(x => new CartItemDto {
                Id = x.Id,
                Catalog = catalogItems.FirstOrDefault(c => c.Id == x.CatalogId),
                CatalogId = x.CatalogId,
                UserId = x.UserId,
                User = null,
                Quantity = x.Quantity,
                AcquiredDate = x.AcquiredDate
            }).ToListAsync();
        }

        // GET: api/Cart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItemDto>> GetCartItemDto(long id)
        {
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
        public async Task<IActionResult> PutCartItemDto(long id, CartItem cartItemDto)
        {
            if (id != cartItemDto.Id)
            {
                return BadRequest();
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
        public async Task<ActionResult<CartItem>> PostCartItemDto(CartItem cartItemDto)
        {
            _context.CartItems.Add(cartItemDto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCartItemDto", new { id = cartItemDto.Id }, cartItemDto);
        }

        // DELETE: api/Cart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItemDto(long id)
        {
            var cartItemDto = await _context.CartItems.FindAsync(id);
            if (cartItemDto == null)
            {
                return NotFound();
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
