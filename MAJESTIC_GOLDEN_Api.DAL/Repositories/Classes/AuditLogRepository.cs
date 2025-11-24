using System.Linq;
using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog logEntry)
        {
            await _context.AuditLogs.AddAsync(logEntry);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetRecentAsync(int take = 50)
        {
            return await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}


