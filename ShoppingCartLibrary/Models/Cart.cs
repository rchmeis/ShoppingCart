using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartLibrary.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int? UserId { get; set; }
        public string? AnonymousId { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal TotalPrice => CartItems.Sum(p => p.Price * p.Quantity);
    }
}
