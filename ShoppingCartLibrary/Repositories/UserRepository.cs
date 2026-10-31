using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using ShoppingCartLibrary.Database;
using ShoppingCartLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartLibrary.Repositories
{
    public class UserRepository:IUserRepository
    {
        private readonly DbConnection _db;

        public UserRepository(DbConnection db)
        {
            _db = db;
        }

        public User Create(string username, string password)
        {
            using SqlConnection connection=_db.DbConnect();
            string query = """
                INSERT INTO [1_users](Username, PasswordHash) 
                OUTPUT INSERTED.*
                VALUES (@username, @passwordhash)
                """;
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@passwordhash", password);
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                User DBuser = new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString()
                };
                return DBuser;
            }
            return null;
        }

        public bool UsernameExists(string username)
        {
            using SqlConnection connection = _db.DbConnect();
            string query = """
                SELECT COUNT(1)
                FROM [1_users]
                WHERE Username = @username
                """;
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            int result = (int)cmd.ExecuteScalar(); //returns 0 if it does not exist, 1 if it does because we used COUNT(1) in SELECT.
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public User GetByUsername(string username)
        {
            using SqlConnection connection = _db.DbConnect();
            string query = """
                SELECT * FROM [1_users] WHERE Username=@username
                """;
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@username", username);
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                User user = new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString()
                };
                return user;
            }
            return null;
        }

        // Henter bruger + kundedata via JOIN, så servicen slipper for SQL
        public UserWithCustomerDto GetUserWithCustomerByUsername(string username)
        {
            using SqlConnection conn = _db.DbConnect();
            string query = @"
                SELECT u.Id, u.Username, u.PasswordHash, c.FirstName 
                FROM [1_users] u
                LEFT JOIN [1_customers] c ON u.Id = c.UserId
                WHERE u.Username = @Username";

            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Username", username);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UserWithCustomerDto
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString(),
                    FirstName = reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : "Bruger"
                };
            }

            return null; // Bruger ikke fundet
        }
    }
}
