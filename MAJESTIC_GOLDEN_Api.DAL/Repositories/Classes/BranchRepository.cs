using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class BranchRepository : GenericRepository<Branch>, IBranchRepository
    {
        public BranchRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Branch?> GetBranchWithDetailsAsync(int id)
        {
            return await context.Branches
                .Include(b => b.Users)
                    .ThenInclude(u => u.PatientProfile) 
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Branch>> GetActiveBranchesAsync()
        {
            return await context.Branches
                .Where(b => b.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}



