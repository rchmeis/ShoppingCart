using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Repositories;
using ShoppingCartLibrary.Services;
using System.Security.Claims;

namespace ShoppingCart.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICRUD<Product> _productRepo;
        private readonly CartService _cartService;

        public List<Product> Products { get; set; } = new();
        public Cart Cart { get; set; } = new();

        [BindProperty]
        public int ProductId { get; set; }

        public IndexModel(ICRUD<Product> productRepo, CartService cartService)
        {
            _productRepo = productRepo;
            _cartService = cartService;
        }

        public void OnGet()
        {
            Products = _productRepo.GetAll();
            int? userId = GetCurrentUserId();
            Cart = _cartService.GetOrCreateCart(HttpContext, userId);

            Response.Cookies.Append("ItemCount", (Cart.CartItems.Sum(ci=>ci.Quantity).ToString())); //session cookie
        }

        public IActionResult OnPostAddToCart()
        {
            int? userId = GetCurrentUserId();
            Cart cart = _cartService.GetOrCreateCart(HttpContext, userId);

            _cartService.AddItemToCart(cart.CartId, ProductId, 1);
            Response.Cookies.Append("ItemCount", (cart.CartItems.Sum(ci=>ci.Quantity)+1).ToString());
            return RedirectToPage();
        }

        private int? GetCurrentUserId()
        {
            var isAuthenticated = User.Identity?.IsAuthenticated;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            // Debug: Tjek i Output-vinduet i Visual Studio når du er "logget ud"
            System.Diagnostics.Debug.WriteLine($"IsAuthenticated: {isAuthenticated}, Claim Value: {userIdClaim?.Value}");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id))
            {
                return id;
            }
            return null;
        }
        //private int? GetCurrentUserId()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id))
        //    {
        //        return id;
        //    }
        //    return null;
        //}
    }
}