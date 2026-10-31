using Microsoft.Data.SqlClient;
using ShoppingCartLibrary.Database;
using ShoppingCartLibrary.Models;

namespace ShoppingCartLibrary.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbConnection _db;

        public OrderRepository(DbConnection db)
        {
            _db = db;
        }

        public int CreateOrder(Cart cart, Customer customer, decimal totalAmount)
        {
            using SqlConnection conn = _db.DbConnect();           

            // Start en transaktion, så både ordre og ordrelinjer oprettes samlet
            using SqlTransaction transaction = conn.BeginTransaction();

            try
            {
                // 1. Indsæt hovedordre
                string orderQuery = @"
                    INSERT INTO [1_orders] (UserId, OrderDate, TotalAmount, CustomerEmail, CustomerName, ShippingAddress)
                    OUTPUT INSERTED.ID
                    VALUES (@UserId, GETDATE(), @TotalAmount, @CustomerEmail, @CustomerName, @ShippingAddress)";

                int orderId;
                using (SqlCommand cmd = new SqlCommand(orderQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@UserId", (object?)cart.UserId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    cmd.Parameters.AddWithValue("@CustomerEmail", customer.Email);
                    cmd.Parameters.AddWithValue("@CustomerName", $"{customer.FirstName} {customer.LastName}");
                    cmd.Parameters.AddWithValue("@ShippingAddress", $"{customer.Address}, {customer.PhoneNumber}"); // Evt. udbyg med postnummer/by hvis du ønsker det

                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 2. Indsæt ordrelinjer (med historisk pris fra CartItem)
                string itemQuery = @"
                    INSERT INTO [1_order_items] (OrderId, ProductId, Quantity, UnitPrice)
                    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice)";

                foreach (var cartItem in cart.CartItems)
                {
                    using (SqlCommand cmd = new SqlCommand(itemQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@ProductId", cartItem.ProductId);
                        cmd.Parameters.AddWithValue("@Quantity", cartItem.Quantity);
                        cmd.Parameters.AddWithValue("@UnitPrice", cartItem.Price); // Gemmer prisen på købstidspunktet!
                        cmd.ExecuteNonQuery();
                    }
                }

                // Bekræft transaktionen
                transaction.Commit();
                return orderId;
            }
            catch
            {
                // Rollback ved fejl
                transaction.Rollback();
                throw;
            }
        }
    }
}