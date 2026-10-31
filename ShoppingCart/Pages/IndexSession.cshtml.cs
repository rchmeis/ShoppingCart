using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Repositories;
using ShoppingCartLibrary.Services;

namespace ShoppingCart.Pages
{
    public class IndexSessionModel : PageModel
    {
        public readonly ICRUD<Product> _productRepository;
        public List<Product> allProducts = new List<Product>();
        public Cart PresentCart { get; set; } = new();
        public string ErrorMessage { get; set; }

        [BindProperty]
        public int Id { get; set; }
        public IndexSessionModel(ICRUD<Product> productrepo)
        {
            _productRepository = productrepo;
        }

       
        public void OnGet()
        {
            allProducts = _productRepository.GetAll();
            PresentCart = HttpContext.Session.GetCart() ?? new Cart();                
        }

        public IActionResult OnPostAddToCart()
        {
            Product product = _productRepository.GetById(Id);
            Cart cart = HttpContext.Session.GetCart();
            //Cart cart = CartSessionHelper.GetCartFromSession(HttpContext);
            CartItem productAlreadyInCart = cart.CartItems.FirstOrDefault(i => i.ProductId == Id);

            if(productAlreadyInCart != null)
            {
                productAlreadyInCart.Quantity++;
            }
            else
            {
                cart.CartItems.Add(new CartItem { ProductId = product.ProductId, Price = product.Price, ProductName = product.ProductName, Quantity = 1 });
            }
            HttpContext.Session.SetCart(cart);
            //CartSessionHelper.SetCartToSession(HttpContext, cart);
            return RedirectToPage();
        }

        public IActionResult OnPostRemoveFromCart()
        {
            Product product = _productRepository.GetById(Id);
            Cart cart = HttpContext.Session.GetCart();
            //var cart = CartSessionHelper.GetCartFromSession(HttpContext);
            CartItem productAlreadyInCart = cart.CartItems.FirstOrDefault(i => i.ProductId == Id);
            if (productAlreadyInCart != null)
            {
                productAlreadyInCart.Quantity--;
                if (productAlreadyInCart.Quantity <= 0)
                {
                    cart.CartItems.Remove(productAlreadyInCart);
                }
            }
            else
            {
                ErrorMessage = "Produktet er ikke i din kurv";
            }
            HttpContext.Session.SetCart(cart);
            //CartSessionHelper.SetCartToSession(HttpContext,cart);
            return RedirectToPage();
        }
    }
}
