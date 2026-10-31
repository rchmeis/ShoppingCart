using ShoppingCartLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartLibrary.Repositories
{
    public interface IUserRepository
    {
        public User Create(string username, string password);
        public bool UsernameExists(string username);
        public User GetByUsername(string username);
        public UserWithCustomerDto GetUserWithCustomerByUsername(string username);
    }
}
