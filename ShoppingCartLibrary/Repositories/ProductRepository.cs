using Microsoft.Data.SqlClient;
using ShoppingCartLibrary.Models;
using System;
using System.Collections.Generic;
using ShoppingCartLibrary.Database;
using System.Text;

namespace ShoppingCartLibrary.Repositories
{
    public class ProductRepository : ICRUD<Product>
    {
        private readonly DbConnection _db;
        public ProductRepository(DbConnection db)
        {
            _db = db;
        }
        public Product Create(Product item)
        {
            throw new NotImplementedException();
        }
        public Product Update(Product item, string id)
        {
            throw new NotImplementedException();
        }

        Product ICRUD<Product>.Delete(string id)
        {
            throw new NotImplementedException();
        }

        List<Product> ICRUD<Product>.GetAll()
        {
            List<Product> products = new List<Product>();

            try
            {
                using SqlConnection connection = _db.DbConnect();
                string query = """
                SELECT * FROM [1_products];
                """;
                using SqlCommand cmd = new SqlCommand(query, connection);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        ProductId = Convert.ToInt32(reader["Id"]),
                        ProductName = reader["name"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                    products.Add(product);
                }
                return products;
            }
            catch(Exception ex)
            {
                throw new Exception("Fejl under læsning fra databasen", ex);
            }
        }

        Product ICRUD<Product>.GetById(int id)
        {
            try
            {
                using SqlConnection connection = _db.DbConnect();
                string query = """
                SELECT * FROM [1_products] WHERE Id = @id;
                """;
                using SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Product product = new Product
                    {
                        ProductId = Convert.ToInt32(reader["Id"]),
                        ProductName = reader["name"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                    return product;
                }
                return null;
            }
            catch(Exception ex)
            {
                throw new Exception("Fejl ved læsning af database, getById");
            }
        }
    }
}
