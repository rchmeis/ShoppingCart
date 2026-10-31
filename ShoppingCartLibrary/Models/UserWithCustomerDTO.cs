using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartLibrary.Models
{
    public class UserWithCustomerDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
    }
}
