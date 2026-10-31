using Microsoft.Data.SqlClient;
using ShoppingCartLibrary.Database;
using ShoppingCartLibrary.Models;

namespace ShoppingCartLibrary.Repositories
{
    public class CartRepository
    {
        private readonly DbConnection _db;

        public CartRepository(DbConnection db)
        {
            _db = db;
        }

        public Cart GetCartById(int cartId)
        {
            using SqlConnection conn = _db.DbConnect();
            string query = @"
                SELECT c.Id, c.UserId, c.AnonymousId, 
                       cp.Id AS CartProductId, cp.ProductId, p.Name, p.Price, cp.Quantity
                FROM [1_cart] c
                LEFT JOIN [1_cart_products] cp ON c.Id = cp.CartId
                LEFT JOIN [1_products] p ON cp.ProductId = p.Id
                WHERE c.Id = @CartId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CartId", cartId);
            return ParseCartFromReader(cmd);
        }

        public Cart GetCartByUserId(int userId)
        {
            using SqlConnection conn = _db.DbConnect();
            string query = @"
                SELECT c.Id, c.UserId, c.AnonymousId, 
                       cp.Id AS CartProductId, cp.ProductId, p.Name, p.Price, cp.Quantity
                FROM [1_cart] c
                LEFT JOIN [1_cart_products] cp ON c.Id = cp.CartId
                LEFT JOIN [1_products] p ON cp.ProductId = p.Id
                WHERE c.UserId = @UserId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            return ParseCartFromReader(cmd);
        }

        public Cart GetCartByAnonymousId(string anonymousId)
        {
            using SqlConnection conn = _db.DbConnect();
            string query = @"
                SELECT c.Id, c.UserId, c.AnonymousId, 
                       cp.Id AS CartProductId, cp.ProductId, p.Name, p.Price, cp.Quantity
                FROM [1_cart] c
                LEFT JOIN [1_cart_products] cp ON c.Id = cp.CartId
                LEFT JOIN [1_products] p ON cp.ProductId = p.Id
                WHERE c.AnonymousId = @AnonymousId";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@AnonymousId", anonymousId);
            return ParseCartFromReader(cmd);
        }

        public Cart CreateCart(int? userId, string? anonymousId)
        {
            using SqlConnection conn = _db.DbConnect();
            string query = "INSERT INTO [1_cart] (UserId, AnonymousId) OUTPUT INSERTED.ID VALUES (@UserId, @AnonymousId)";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AnonymousId", (object?)anonymousId ?? DBNull.Value);

            int cartId = Convert.ToInt32(cmd.ExecuteScalar());
            return new Cart { CartId = cartId, UserId = userId, AnonymousId = anonymousId };
        }

        public void AddItem(int cartId, int productId, int quantity = 1)
        {
            using SqlConnection conn = _db.DbConnect();
            
            //følgende query laver en update + insert handling i én query.
            //Den sætter først målet (den tabel vi vil opdatere/indsætte i
            //derefter bruges using (select... til at putte cartid og productid in i en midlertidig tabel som source
            //ON er match betingelsen. ie, findes denne kombination allerede i vores target tabel=
            //WHEN MATCHED THEN fortæller hvad vi gør hvis den findes (her target.quantity+@quantity). dvs en update
            //WHEN NOT MATCHED THEN indsættes blot cartid, productid og quantity, dvs som en insert
            string query = """
                        
                MERGE INTO [1_cart_products] AS target  
                USING (SELECT @CartId AS CartId, @ProductId AS ProductId) AS source
                ON target.CartId = source.CartId AND target.ProductId = source.ProductId
                WHEN MATCHED THEN
                    UPDATE SET Quantity = target.Quantity + @Quantity
                WHEN NOT MATCHED THEN
                    INSERT (CartId, ProductId, Quantity) VALUES (@CartId, @ProductId, @Quantity);
                
                """;

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CartId", cartId);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            cmd.Parameters.AddWithValue("@Quantity", quantity);
            cmd.ExecuteNonQuery();
        }

        public void DecreaseOrRemoveItem(int cartItemId)
        {
            using SqlConnection conn = _db.DbConnect();

            // Denne query tjekker først mængden:
            // - Hvis Quantity > 1: Opdaterer den og trækker 1 fra.
            // - Hvis Quantity <= 1: Sletter den rækken helt fra tabellen.
            string query = @"
                IF EXISTS (SELECT 1 FROM [1_cart_products] WHERE Id = @CartItemId AND Quantity > 1)
                BEGIN
                    UPDATE [1_cart_products] 
                    SET Quantity = Quantity - 1 
                    WHERE Id = @CartItemId
                END
                ELSE
                BEGIN
                    DELETE FROM [1_cart_products] 
                    WHERE Id = @CartItemId
                END";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CartItemId", cartItemId);
            cmd.ExecuteNonQuery();
        }
        public void UpdateCartOwner(int cartId, int userId)
        {
            using SqlConnection conn = _db.DbConnect();
            string query = "UPDATE [1_cart] SET UserId = @UserId, AnonymousId = NULL WHERE Id = @CartId";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@CartId", cartId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteCart(int cartId)
        {
            using SqlConnection conn = _db.DbConnect();
            string query = "DELETE FROM [1_cart] WHERE Id = @CartId";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CartId", cartId);
            cmd.ExecuteNonQuery();
        }

        private Cart ParseCartFromReader(SqlCommand cmd)
        {
            Cart cart = null;
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (cart == null)
                {
                    cart = new Cart
                    {
                        CartId = Convert.ToInt32(reader["Id"]),
                        UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : null,
                        AnonymousId = reader["AnonymousId"] != DBNull.Value ? reader["AnonymousId"].ToString() : null
                    };
                }

                if (reader["ProductId"] != DBNull.Value)
                {
                    cart.CartItems.Add(new CartItem
                    {
                        Id = Convert.ToInt32(reader["CartProductId"]),
                        ProductId = Convert.ToInt32(reader["ProductId"]),
                        ProductName = reader["Name"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Quantity = Convert.ToInt32(reader["Quantity"])
                    });
                }
            }
            return cart;
        }
    }
}