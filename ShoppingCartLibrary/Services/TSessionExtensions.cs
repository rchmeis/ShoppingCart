using Microsoft.AspNetCore.Http;
using ShoppingCartLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ShoppingCartLibrary.Services
{
    public static class TSessionExtensions //En generisk SessionExtensionsklasse (bedst!) med tilføjelse af specfikke metoder under de generiske. 
    {
        private static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        private static T? Get<T>(this ISession session, string key)
        {
            string? value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }


        // Specifikke metoder: her gemmes key værdier, for at unggå fejl ved manuel indtastning
        private const string CartKey = "MyCartSessionKey";
        private const string CustomerKey = "CustomerWizardDataKey";

        public static void SetCart(this ISession session, Cart cart) => session.Set(CartKey, cart);
        public static Cart GetCart(this ISession session) => session.Get<Cart>(CartKey) ?? new Cart();

        public static void SetCustomerData(this ISession session, Customer model) => session.Set(CustomerKey, model);
        public static Customer GetCustomerData(this ISession session) => session.Get<Customer>(CustomerKey) ?? new Customer();
    }
}
