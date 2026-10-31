using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Services;

namespace ShoppingCart.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly UserService _userService;

        public SignUpModel(UserService userService)
        {
            _userService = userService;
        }

      
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

       
        [BindProperty]
        public Customer Customer { get; set; } = new();

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
          
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                User newUser = _userService.Register(Username, Password, Customer);
                if (newUser == null)
                {
                    ErrorMessage = "Brugernavnet er optaget.";
                    return Page();
                }
            }
            catch(Exception ex)
            {
                ErrorMessage = "Oprettelsen fejlede - kontakt admin";
                return Page();
            }
           
            return RedirectToPage("/Account/SignIn");
        }
    }
}