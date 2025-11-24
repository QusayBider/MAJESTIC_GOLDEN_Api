using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface IBranchRepository : IGenericRepository<Branch>
    {
        Task<Branch?> GetBranchWithDetailsAsync(int id);
        Task<IEnumerable<Branch>> GetActiveBranchesAsync();
    }
}







