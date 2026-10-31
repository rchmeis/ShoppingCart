using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingCartLibrary.Services;

namespace ShoppingCart.Pages.Account
{
    public class SignInModel : PageModel
    {
        private readonly AuthenticationService _authService;

        public SignInModel(AuthenticationService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
           
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                bool success = await _authService.SignInAsync(HttpContext, Username, Password);

                if (!success)
                {
                    ErrorMessage = "Ugyldigt brugernavn eller adgangskode.";
                    return Page();
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = "Fejl under login - kontakt systemadmin. " + ex.Message;
            }


            return RedirectToPage("/Index");
        }
    }
}