using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartLibrary.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; } //bør ikke være her. Heller ikke name eler ID. Blot Product:Product
        public int Quantity { get; set; }
    }
}
