using ShoppingCartLibrary.Models;
using ShoppingCartLibrary.Repositories;
using System.Collections.Generic;
// using ShoppingCartLibrary.Models;
// using ShoppingCartLibrary.Repositories;

namespace ShoppingCartLibrary.Services
{
    public class CustomerService
    {
        private readonly CustomerRepository _customerRepo;

        public CustomerService(CustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public int Create(Customer customer)
        {
            return _customerRepo.Create(customer);
        }

        public Customer? GetById(int id)
        {
            return _customerRepo.GetById(id);
        }

        public Customer? GetByUserId(int userId)
        {
            return _customerRepo.GetByUserId(userId);
        }

        public IEnumerable<Customer> GetAll()
        {
            return _customerRepo.GetAll();
        }

        public bool Update(Customer customer)
        {
            return _customerRepo.Update(customer);
        }

        public bool Delete(int id)
        {
            return _customerRepo.Delete(id);
        }
    }
}