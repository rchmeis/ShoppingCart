using Microsoft.AspNetCore.Http;
using ShoppingCartLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ShoppingCartLibrary.Services
{
    public static class SessionExtensions  //et eksempel på en ikke generisk SessionExtensions: 
    {
        // --- 1. INDKØBSKURV (Cart) ---
        //private const string CartKey = "MyCartSessionKey";

        //public static Cart GetCart(this ISession session)
        //{
        //    string? json = session.GetString(CartKey);
        //    return string.IsNullOrEmpty(json) ? new Cart() : JsonSerializer.Deserialize<Cart>(json)!;
        //}

        //public static void SetCart(this ISession session, Cart cart)
        //{
        //    session.SetString(CartKey, JsonSerializer.Serialize(cart));
        //}


        //// --- 2. MULTI-STEP CHECKOUT FORMULAR ---
        //private const string CheckoutKey = "CheckoutWizardDataKey";

        //public static CheckoutModel GetCheckoutData(this ISession session)
        //{
        //    string? json = session.GetString(CheckoutKey);
        //    return string.IsNullOrEmpty(json) ? new CheckoutModel() : JsonSerializer.Deserialize<CheckoutModel>(json)!;
        //}

        //public static void SetCheckoutData(this ISession session, CheckoutModel model)
        //{
        //    session.SetString(CheckoutKey, JsonSerializer.Serialize(model));
        //}


        // --- 3. MIDLERTIDIGE BRUGER-PRÆFERENCER ---
        //private const string PrefsKey = "UserPreferencesKey";

        //public static UserPreferences GetPreferences(this ISession session)
        //{
        //    string? json = session.GetString(PrefsKey);
        //    return string.IsNullOrEmpty(json) ? new UserPreferences() : JsonSerializer.Deserialize<UserPreferences>(json)!;
        //}

        //public static void SetPreferences(this ISession session, UserPreferences prefs)
        //{
        //    session.SetString(PrefsKey, JsonSerializer.Serialize(prefs));
        //}
    }

}
