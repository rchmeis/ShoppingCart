using ShoppingCartLibrary.Models;

namespace ShoppingCartLibrary.Repositories
{
    public interface IOrderRepository
    {
        int CreateOrder(Cart cart, Customer customer, decimal totalAmount);
    }
}