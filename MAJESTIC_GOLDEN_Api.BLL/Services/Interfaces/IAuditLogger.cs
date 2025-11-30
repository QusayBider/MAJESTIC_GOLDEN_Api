using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IAuditLogger
    {
        Task LogAsync(
            string action,
            string entityName,
            string entityId,
            string? userId = null,
            string? userName = null,
            object? oldValues = null,
            object? newValues = null,
            string? ipAddress = null);
    }
}





