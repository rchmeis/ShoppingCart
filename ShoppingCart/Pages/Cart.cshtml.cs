using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Services;
using System.Security.Claims;

namespace ShoppingCart.Pages
{
    public class CartModel : PageModel
    {
        private readonly CartService _cartService;

        public CartModel(CartService cartService)
        {
            _cartService = cartService;
        }

        public Cart Cart { get; set; } = new();

        [BindProperty]
        public int CartItemId { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public void OnGet()
        {
            int? userId = GetCurrentUserId();
            Cart = _cartService.GetOrCreateCart(HttpContext, userId);
            Response.Cookies.Append("CartItems", Cart.CartItems.Sum(ci => ci.Quantity).ToString());
        }

        public IActionResult OnPostRemoveItem()
        {
            _cartService.DecreaseOrRemoveItem(CartItemId);
            Response.Cookies.Append("CartItems", (Cart.CartItems.Sum(ci => ci.Quantity)-1).ToString());

            return RedirectToPage();
        }
        public IActionResult OnPostAddToCart()
        {
            int? userId = GetCurrentUserId();
            Cart cart = _cartService.GetOrCreateCart(HttpContext, userId);

            _cartService.AddItemToCart(cart.CartId, CartItemId, 1);
            Response.Cookies.Append("ItemCount", (cart.CartItems.Sum(ci => ci.Quantity) + 1).ToString());
            return RedirectToPage();
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out int id) ? id : null;
        }
    }
}