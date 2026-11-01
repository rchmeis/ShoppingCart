using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ShoppingCart.Pages.Account
{
    public class MyProfileModel : PageModel
    {
        public string Name { get; set; }
        public void OnGet()
        {
            Name = User.Identity.Name;
        }
    }
}
