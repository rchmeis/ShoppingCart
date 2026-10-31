using DotNetEnv;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
 
namespace ShoppingCartLibrary.Database
{
    public class DbConnection
    {      
        public SqlConnection DbConnect()
        {
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Fejl: 'DefaultConnection' blev ikke fundet i miljøvariablerne eller .env-filen!");
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
