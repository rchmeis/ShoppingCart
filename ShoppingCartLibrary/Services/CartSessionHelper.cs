using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using ShoppingCartLibrary.Models;

namespace ShoppingCartLibrary.Services
{
    public static class CartSessionHelper  //denne klasse er her blot som demonstration for forskellige måder at arbejde med sessions på
    {
        private const string SessionKey = "MyCartSessionKey";
        public static Cart GetCartFromSession(HttpContext context)
        {
            string? sessionContentStr = context.Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(sessionContentStr))
            {
                return new Cart();
            }

            return JsonSerializer.Deserialize<Cart>(sessionContentStr) ?? new Cart();
        }

        public static void SetCartToSession(HttpContext context, Cart cart)
        {
            string sessionContentStr = JsonSerializer.Serialize(cart);
            context.Session.SetString(SessionKey, sessionContentStr);
        }
    }



    //Hvorfor bruges json serializer?
    //Fordi HttpContext.Session(og mange andre systemer) som udgangspunkt kun er lavet til at gemme simple ting som string og byte[] (tekst og binære data).
    //Den forstår ikke et komplekst C#-objekt som en Cart med lister og objekter indeni.
    //C# objektet gemmes som en json streng i HttpContext session (i serverens RAM) og hentes fra HttpContext.Session som en jsonstreng, derefter deserialiseres til et C# objet.
}
