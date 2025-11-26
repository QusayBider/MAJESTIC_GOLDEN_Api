using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog logEntry);
        Task<IEnumerable<AuditLog>> GetRecentAsync(int take = 50);
    }
}




