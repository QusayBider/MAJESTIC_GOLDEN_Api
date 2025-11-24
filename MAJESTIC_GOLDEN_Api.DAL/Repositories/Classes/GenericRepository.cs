using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseModel
    {
        protected readonly ApplicationDbContext context;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<int> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await context.Set<T>().AddAsync(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool withTracking = false)
        {
            if (withTracking)
                return await context.Set<T>().ToListAsync();

            return await context.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id) => await context.Set<T>().FindAsync(id);

        public async Task<T?> GetByIdNoTrackingAsync(int id)
        {
            var entity = await context.Set<T>().FindAsync(id);
            if (entity != null)
                context.Entry(entity).State = EntityState.Detached; 

            return entity;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool withTracking = false)
        {
            if (withTracking)
                return await context.Set<T>().Where(predicate).ToListAsync();

            return await context.Set<T>().AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<int> RemoveAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            context.Set<T>().Update(entity);
            return await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await context.Set<T>().CountAsync();

            return await context.Set<T>().CountAsync(predicate);
        }
    }

}
