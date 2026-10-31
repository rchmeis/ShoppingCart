using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ShoppingCartLibrary.Models;
using System.Net;
using System.Security.Claims;

namespace ShoppingCartLibrary.Services
{
    public class AuthenticationService
    {
        private readonly UserService _userService;
        private readonly CartService _cartService;
        private readonly IPasswordHasher<User> _passwordHasher;      

        // Vi injicerer UserRepository i stedet for DbConnection
        public AuthenticationService(UserService userService, CartService cartService, IPasswordHasher<User> passwordHasher, CustomerService customerService)
        {
            _userService = userService;
            _cartService = cartService;
            _passwordHasher = passwordHasher;        
        }

        public async Task<bool> SignInAsync(HttpContext context, string username, string password)
        {                       
            UserWithCustomerDto user = _userService.GetUserWithCustomerByUsername(username);
            if (user == null|| _passwordHasher.VerifyHashedPassword(null!, user.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return false;
            }
            _cartService.MergeAnonymousCartToUser(context, user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.FirstName),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return true;           
        }


        public async Task LogoutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}