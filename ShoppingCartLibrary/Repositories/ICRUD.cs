using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartLibrary.Repositories
{
    public interface ICRUD<T>
    {
        public T Create(T item);
        public T Delete(string id);
        public T Update(T item, string id);
        public List<T> GetAll();
        public T GetById(int id);
    }
}
