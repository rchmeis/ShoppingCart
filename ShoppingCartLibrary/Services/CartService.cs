using Microsoft.AspNetCore.Http;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Repositories;

namespace ShoppingCartLibrary.Services
{
    public class CartService
    {
        private readonly CartRepository _cartRepo;
        private const string CookieName = "CartCookieId";
 
        public CartService(CartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        public Cart GetOrCreateCart(HttpContext httpContext, int? userId = null)
        {
            Cart cart = null;

            //henter kurv hvis den findes (fra gæst eller bruger)
            if (userId.HasValue)
            {
                cart = _cartRepo.GetCartByUserId(userId.Value);
            }
            else
            {
                string anonymousId = httpContext.Request.Cookies[CookieName];
                if (!string.IsNullOrEmpty(anonymousId))
                {
                    cart = _cartRepo.GetCartByAnonymousId(anonymousId);
                }
            }

            // Hvis der stadig ikke findes en kurv, opret en ny
            if (cart == null)
            {
                string anonymousId = null;

                if (!userId.HasValue)
                {
                    anonymousId = Guid.NewGuid().ToString();
                    httpContext.Response.Cookies.Append(CookieName, anonymousId, new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(30),
                        HttpOnly = true,
                        IsEssential = true
                    });
                }

                cart = _cartRepo.CreateCart(userId, anonymousId);
            }

            return cart;
        }

        public void AddItemToCart(int cartId, int productId, int quantity = 1)
        {
            _cartRepo.AddItem(cartId, productId, quantity);
        }

        public void DecreaseOrRemoveItem(int cartItemId)
        {
            _cartRepo.DecreaseOrRemoveItem(cartItemId);
        }

        // Merge-logik ved login
        public void MergeAnonymousCartToUser(HttpContext httpContext, int userId)
        {
            // 1. Tjek om der overhovedet er en gæste-cookie
            string anonymousId = httpContext.Request.Cookies[CookieName];
            if (string.IsNullOrEmpty(anonymousId)) return;

            // 2. Hent gæstekurven fra databasen
            Cart anonymousCart = _cartRepo.GetCartByAnonymousId(anonymousId);
            if (anonymousCart == null || !anonymousCart.CartItems.Any())
            {
                // Hvis gæstekurven er tom eller ikke findes, slet blot cookien
                httpContext.Response.Cookies.Delete(CookieName);
                return;
            }

            // 3. Tjek om brugeren ALLEREDE har en kurv liggende i databasen
            Cart existingUserCart = _cartRepo.GetCartByUserId(userId);

            if (existingUserCart == null)
            {
                // ----------------------------------------------------
                // SCENARIO A: DET SMARTE VALG (Ingen eksisterende kurv)
                // Brugeren har ingen kurv i forvejen. Vi opdaterer bare 
                // ejeren på gæstekurven direkte i databasen!
                // ----------------------------------------------------
                _cartRepo.UpdateCartOwner(anonymousCart.CartId, userId);
            }
            else
            {
                // ----------------------------------------------------
                // SCENARIO B: KOLLISION (Brugeren har en kurv i forvejen)
                // Vi flytter varerne over i brugerens eksisterende kurv 
                // og sletter gæstekurven.
                // ----------------------------------------------------
                foreach (CartItem item in anonymousCart.CartItems)
                {
                    _cartRepo.AddItem(existingUserCart.CartId, item.ProductId, item.Quantity);
                }
                _cartRepo.DeleteCart(anonymousCart.CartId);
            }

            // 4. Slet altid gæste-cookien i browseren efter endt login/merge
            httpContext.Response.Cookies.Delete(CookieName, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true
            });
        }
    }
}