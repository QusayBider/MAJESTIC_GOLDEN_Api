using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface ILaboratoryRepository : IGenericRepository<Laboratory>
    {
        Task<Laboratory?> GetByIdWithUserAsync(int id);
        Task<Laboratory?> GetByUserIdAsync(string userId);
        Task<IEnumerable<Laboratory>> GetAllWithUsersAsync();
    }
}




