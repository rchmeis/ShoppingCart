using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Services;
using System.Security.Claims;

namespace ShoppingCart.Pages.Checkout
{
    public class Step1Model : PageModel
    {
        private readonly CartService _cartService;
        private readonly CustomerService _customerService;

        public Step1Model(CartService cartService, CustomerService customerService)
        {
            _cartService = cartService;
            _customerService = customerService;
        }

        [BindProperty]
        public Customer customer { get; set; } = new();

        public Cart Cart { get; set; } = new();

        public void OnGet()
        {
            int? userId = GetCurrentUserId();
            
            Cart = _cartService.GetOrCreateCart(HttpContext, userId);

     
            if (userId.HasValue)
            {
                var existingCustomer = _customerService.GetByUserId(userId.Value);
                if (existingCustomer != null)
                {
                    customer = existingCustomer;
                }
            }
            else { customer = HttpContext.Session.GetCustomerData(); }

        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int? userId = GetCurrentUserId();
            Cart = _cartService.GetOrCreateCart(HttpContext, userId);

           
            HttpContext.Session.SetCustomerData(customer);

            return RedirectToPage("Summary");
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