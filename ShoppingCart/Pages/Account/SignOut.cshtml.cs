using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingCartLibrary.Services;

namespace ShoppingCart.Pages.Account
{
    public class SignOutModel : PageModel
    {
        private readonly AuthenticationService _authService;

        public SignOutModel(AuthenticationService authService)
        {
            _authService = authService;
        }

        // Håndterer POST (anbefalet for sikkerhed mod CSRF ved logud)
        public async Task<IActionResult> OnPostAsync()
        {
            Response.Cookies.Delete("ItemCount");
            await _authService.LogoutAsync(HttpContext);
            return RedirectToPage("/Index");
        }

        // Håndterer GET, hvis der logges ud via et rent link i stedet for en formular-knap
        public async Task<IActionResult> OnGetAsync()
        {
            await _authService.LogoutAsync(HttpContext);
            return RedirectToPage("/Index");
        }
    }
}