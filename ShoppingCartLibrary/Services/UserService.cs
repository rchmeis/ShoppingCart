using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartLibrary.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _hasher;
        private readonly CustomerService _customerService;
        public UserService(IUserRepository userRepository, IPasswordHasher<User> hasher, CustomerService customerService)
        {
            _userRepository = userRepository;
            _hasher = hasher;
            _customerService = customerService;
        }
        public User Register(string username, string password, Customer customer)
        {
            if (UsernameExists(username))
            {
                return null;
            }
            string hashedPassword = _hasher.HashPassword(null!, password);
            User user = Create(username, hashedPassword);
            if (user == null)
            {
                return null;
            }
            Customer cust = new Customer
            {
                UserId = user.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber
            };
            _customerService.Create(cust);
            return user;
        }
        public User Create(string username, string password)
        {
            return _userRepository.Create(username, password);
        }
        public bool UsernameExists(string username)
        {
            return _userRepository.UsernameExists(username);
        }
        public User GetByUsername(string username)
        {
            return _userRepository.GetByUsername(username);
        }
        public UserWithCustomerDto GetUserWithCustomerByUsername(string username)
        {
            return _userRepository.GetUserWithCustomerByUsername(username);
        }

    }
}
