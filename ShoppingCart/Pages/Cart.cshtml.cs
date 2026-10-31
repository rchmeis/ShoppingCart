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
        }

        public IActionResult OnPostRemoveItem()
        {
            _cartService.DecreaseOrRemoveItem(CartItemId);

            return RedirectToPage();
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null && int.TryParse(claim.Value, out int id) ? id : null;
        }
    }
}