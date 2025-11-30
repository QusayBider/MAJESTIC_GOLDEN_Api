using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class LaboratoryRepository : GenericRepository<Laboratory>, ILaboratoryRepository
    {
        public LaboratoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Laboratory?> GetByIdWithUserAsync(int id)
        {
            return await context.Laboratories
                .Include(l => l.User)
                .Include(l => l.Requests)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Laboratory?> GetByUserIdAsync(string userId)
        {
            return await context.Laboratories
                .Include(l => l.User)
                .Include(l => l.Requests)
                .FirstOrDefaultAsync(l => l.UserId == userId);
        }

        public async Task<IEnumerable<Laboratory>> GetAllWithUsersAsync()
        {
            return await context.Laboratories
                .Include(l => l.User)
                .Include(l => l.Requests)
                .ToListAsync();
        }
    }
}






