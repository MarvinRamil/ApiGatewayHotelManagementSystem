using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementApplication.Interface.Repositories
{
    public interface IGeneric<T> where T : class
    {
        Task<T?> GetById(int id);
        Task<List<T>> GetAllAsync();
        Task Add(T item);
        Task Update(T item);
        Task Delete(T item);
        Task<T?> FindAsync(Func<T, bool> exp);
    }
}
