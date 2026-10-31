using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Services;
using System.Security.Claims;

namespace ShoppingCart.Pages.Checkout
{
    public class SummaryModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly CustomerService _customerService;
        private readonly OrderService _orderService;

        public SummaryModel(CartService cartService, CustomerService customerService, OrderService orderService)
        {
            _cartService = cartService;
            _customerService = customerService;
            _orderService = orderService;
        }

        public Customer customer { get; set; } = new();
        public Cart Cart { get; set; } = new();

        public void OnGet()
        {
            int? userId = GetCurrentUserId();
           
            Cart = _cartService.GetOrCreateCart(HttpContext, userId);
           
            if (userId.HasValue)
            {
                customer = _customerService.GetByUserId(userId.Value) ?? new();
            }
            else
            {
                customer = HttpContext.Session.GetCustomerData();
            }            
            // --- SESSION-KODE (Udkommet til demonstration af sessions) ---
            // Cart = HttpContext.Session.GetCart();
        }

        public IActionResult OnPostConfirm()
        {
            int? userId = GetCurrentUserId();
            var cart = _cartService.GetOrCreateCart(HttpContext, userId);

            // Hent kundedata (enten via db hvis logget ind, eller fra formular/session)
            if (userId.HasValue)
            {
                customer = _customerService.GetByUserId(userId.Value) ?? new();
            }
            else { customer = HttpContext.Session.GetCustomerData()??new(); }
                        
            // --- SESSION-KODE (Udkommet til demonstration af sessioncart) ---
            //var cartSession = HttpContext.Session.GetCart();

            try
            {
                // 1. Gem ordren i databasen og ryd kurven samt gæstecookie hvis gæst
                _orderService.CompleteCheckout(HttpContext, cart, customer);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Der opstod en fejl under oprettelse af ordren: " + ex.Message);
                return Page();
            }

            // 2. Ryd sessionen hvis den blev brugt
            // HttpContext.Session.Remove("MyCartSessionKey");
            HttpContext.Session.Remove("CustomerWizardDataKey");
         
            return RedirectToPage("/Checkout/Confirmation");
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id))
            {
                return id;
            }
            return null;
        }
    }
}