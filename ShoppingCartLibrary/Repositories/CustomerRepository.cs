using Microsoft.Data.SqlClient; // Sørg for at du bruger Microsoft.Data.SqlClient eller System.Data.SqlClient
using System;
using System.Collections.Generic;
using ShoppingCartLibrary.Database;
using ShoppingCartLibrary.Models;

namespace ShoppingCartLibrary.Repositories
{
    public class CustomerRepository
    {
        private readonly DbConnection _dbConnection; // Erstat med din faktiske database-klasse/interface

        public CustomerRepository(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // CREATE
        public int Create(Customer customer)
        {
            string query = """
                INSERT INTO [dbo].[1_customers] (UserId, FirstName, LastName, Email, PhoneNumber, Address)
                OUTPUT INSERTED.Id
                VALUES (@UserId, @FirstName, @LastName, @Email, @PhoneNumber, @Address);
                """;

            using SqlConnection connection = _dbConnection.DbConnect();
            using SqlCommand cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@UserId", customer.UserId);
            cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
            cmd.Parameters.AddWithValue("@LastName", customer.LastName);
            cmd.Parameters.AddWithValue("@Email", customer.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)customer.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", customer.Address);
            
            // ExecuteScalar henter det nye Id fra OUTPUT INSERTED.Id
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public Customer? GetById(int id)
        {
            string query = """
                SELECT Id, UserId, FirstName, LastName, Email, PhoneNumber, Address 
                FROM [dbo].[1_customers] 
                WHERE Id = @Id;
                """;

            using SqlConnection connection = _dbConnection.DbConnect();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return MapToCustomer(reader);
            }
            return null;
        }

        public Customer? GetByUserId(int userId)
        {
            string query = """
                SELECT Id, UserId, FirstName, LastName, Email, PhoneNumber, Address 
                FROM [dbo].[1_customers] 
                WHERE UserId = @UserId;
                """;

            using SqlConnection connection = _dbConnection.DbConnect();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapToCustomer(reader);
            }
            return null;
        }

        public IEnumerable<Customer> GetAll()
        {
            var customers = new List<Customer>();
            string query = """
                SELECT Id, UserId, FirstName, LastName, Email, PhoneNumber, Address 
                FROM [dbo].[1_customers];
                """;

            using SqlConnection connection = _dbConnection.DbConnect();
            using SqlCommand cmd = new SqlCommand(query, connection);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                customers.Add(MapToCustomer(reader));
            }
            return customers;
        }

   
        public bool Update(Customer customer)
        {
            string query = """
                UPDATE [dbo].[1_customers]
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    PhoneNumber = @PhoneNumber,
                    Address = @Address
                WHERE Id = @Id;
                """;

            using SqlConnection connection = _dbConnection.DbConnect();
            using SqlCommand cmd = new SqlCommand(query, connection);

            cmd.Parameters.AddWithValue("@Id", customer.Id);
            cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
            cmd.Parameters.AddWithValue("@LastName", customer.LastName);
            cmd.Parameters.AddWithValue("@Email", customer.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", (object?)customer.PhoneNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", customer.Address);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }


        public bool Delete(int id)
        {
            string query = """
                DELETE FROM [dbo].[1_customers] 
                WHERE Id = @Id;
                """;

            using SqlConnection connection = _dbConnection.DbConnect();
            using SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

       
        private Customer MapToCustomer(SqlDataReader reader)
        {
            return new Customer
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                Address = reader.GetString(reader.GetOrdinal("Address"))
            };
        }
    }
}