using Microsoft.AspNetCore.Http;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Repositories;

namespace ShoppingCartLibrary.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly CartRepository _cartRepo;

        public OrderService(IOrderRepository orderRepo, CartRepository cartRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
        }

        public int CompleteCheckout(HttpContext httpContext, Cart cart, Customer customer)
        {
            if (cart == null || !cart.CartItems.Any())
            {
                throw new InvalidOperationException("Kan ikke gennemføre ordre med en tom kurv.");
            }

            // Beregn totalbeløb
            decimal totalAmount = cart.CartItems.Sum(item => item.Price * item.Quantity);

            // Gem i databasen
            int orderId = _orderRepo.CreateOrder(cart, customer, totalAmount);

            // Slet kurven fra databasen nu hvor ordren er gennemført
            _cartRepo.DeleteCart(cart.CartId);

            if (httpContext.Request.Cookies["CartCookieId"] != null)
            {
                httpContext.Response.Cookies.Delete("CartCookieId");
            }

            return orderId;
        }
    }
}