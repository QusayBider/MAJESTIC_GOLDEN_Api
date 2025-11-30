using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class AuditLogger : IAuditLogger
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };

        public AuditLogger(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task LogAsync(
            string action,
            string entityName,
            string entityId,
            string? userId = null,
            string? userName = null,
            object? oldValues = null,
            object? newValues = null,
            string? ipAddress = null)
        {
            var logEntry = new AuditLog
            {
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                UserId = userId,
                UserName = userName,
                IpAddress = ipAddress,
                OldValues = SerializeOrDefault(oldValues),
                NewValues = SerializeOrDefault(newValues),
                Timestamp = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(logEntry);
        }

        private string? SerializeOrDefault(object? value)
        {
            return value == null ? null : JsonSerializer.Serialize(value, _serializerOptions);
        }
    }
}





