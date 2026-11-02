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
            Cart = _cartService.GetOrCreateCart(HttpContext, userId);     //kurv SKAL hentes fra databasen.  
            customer = HttpContext.Session.GetCustomerData();             //kundeinfo SKAL hentes fra session! da de kan have valgt anden addresse end den der er gemt i databasen(på deres profil)
                     
            // --- SESSION-KODE (Udkommet til demonstration af sessions) ---
            // Cart = HttpContext.Session.GetCart();
        }

        public IActionResult OnPostConfirm()
        {
            int? userId = GetCurrentUserId();
            var cart = _cartService.GetOrCreateCart(HttpContext, userId);

            customer = HttpContext.Session.GetCustomerData() ?? new();
            if (customer== null) //hvis sessionen udløb ell. lign
            {
                return RedirectToPage("Step1");
            }

            // --- SESSION-KODE (Udkommet til demonstration af sessioncart) ---
            //var cartSession = HttpContext.Session.GetCart();

            try
            {
                // Gemmer ordren i databasen og rydder kurven samt gæstecookie hvis gæst!
                _orderService.CompleteCheckout(HttpContext, cart, customer);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Der opstod en fejl under oprettelse af ordren: " + ex.Message);
                return Page();
            }

            // Ryd sessionen !
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