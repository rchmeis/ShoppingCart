using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Connections;
using ShoppingCartLibrary.Database;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Repositories;
using ShoppingCartLibrary.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ShoppingCart
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".MyWebshop.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true; //requiret due to gdpr. This indicates the the cookie is necessary for the app to function correctly
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
             .AddCookie(options =>
             {
                 options.LoginPath = "/Account/SignIn";     // Hvor sendes brugere hen, hvis de ikke er logget ind?
                 options.LogoutPath = "/Account/Logout";   // Sti til logud
                 options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Hvor længe cookien gælder

                 //nedenstående er automatisk sat ved brug af autencificeringscookies. Men tilføjet så de kan ses.
                 options.Cookie.HttpOnly = true; // Blokerer JavaScript i at læse cookien (Standard er true, men godt at kende)
                 //options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Kræver HTTPS i produktion (brug Always hvis du kun kører HTTPS)
                 options.Cookie.SameSite = SameSiteMode.Lax; // Beskytter mod CSRF-angreb, men tillader navigation
             });

            //Registrering af database og repositories (data access layer)
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<DbConnection>();
            builder.Services.AddScoped<ICRUD<Product>, ProductRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<CustomerRepository>();
            builder.Services.AddScoped<CartRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            //registerering af services (Business Logic Layer)
            builder.Services.AddScoped<CustomerService>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<AuthenticationService>();
            builder.Services.AddScoped<OrderService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
