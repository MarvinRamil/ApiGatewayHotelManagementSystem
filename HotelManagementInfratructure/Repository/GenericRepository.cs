using HotelManagementApplication.Interface.Repositories;
using HotelManagementInfratructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagementInfratructure.Repository
{
    public class GenericRepository<T> : IGeneric<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task Add(T item)
        {
            await _dbSet.AddAsync(item);
        }

        public virtual Task Delete(T item)
        {
            _dbSet.Remove(item);
            return Task.CompletedTask;
        }

        public virtual Task<T?> FindAsync(Func<T, bool> exp)
        {
            return Task.FromResult(_dbSet.FirstOrDefault(exp));
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual Task Update(T item)
        {
            _dbSet.Update(item);
            return Task.CompletedTask;
        }
    }
}
