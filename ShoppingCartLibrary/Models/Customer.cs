using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartLibrary.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
