using MAJESTIC_GOLDEN_Api.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdNoTrackingAsync(int id);
        Task<int> AddAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync(bool withTracking = false);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, bool withTracking = false);
        Task<int> RemoveAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }

}
