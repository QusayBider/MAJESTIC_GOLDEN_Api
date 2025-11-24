using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class LabRequestRepository : GenericRepository<LabRequest>, ILabRequestRepository
    {
        public LabRequestRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LabRequest?> GetLabRequestWithDetailsAsync(int id)
        {
            return await context.LabRequests
                .Include(lr => lr.Patient)
                    .ThenInclude(p => p.User)
                .Include(lr => lr.Doctor)
                .Include(lr => lr.Laboratory)
                    .ThenInclude(l => l.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(lr => lr.Id == id);
        }

        public async Task<IEnumerable<LabRequest>> GetLabRequestsByDoctorAsync(string doctorId)
        {
            return await context.LabRequests
                .Where(lr => lr.DoctorId == doctorId)
                .Include(lr => lr.Patient)
                    .ThenInclude(p => p.User)
                .Include(lr => lr.Laboratory)
                    .ThenInclude(l => l.User)
                .AsNoTracking()
                .OrderByDescending(lr => lr.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabRequest>> GetLabRequestsByPatientAsync(string patientUserId)
        {
            return await context.LabRequests
                .Where(lr => lr.PatientUserId == patientUserId)
                .Include(lr => lr.Doctor)
                .Include(lr => lr.Laboratory)
                .AsNoTracking()
                .OrderByDescending(lr => lr.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabRequest>> GetLabRequestsByLaboratoryAsync(int laboratoryId)
        {
            return await context.LabRequests
                .Where(lr => lr.LaboratoryId == laboratoryId)
                .Include(lr => lr.Patient)
                    .ThenInclude(p => p.User)
                .Include(lr => lr.Doctor)
                .Include(lr => lr.Laboratory)
                    .ThenInclude(l => l.User)
                .AsNoTracking()
                .OrderByDescending(lr => lr.RequestDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabRequest>> GetPendingLabRequestsAsync()
        {
            return await context.LabRequests
                .Where(lr => lr.Status == LabRequestStatus.Pending || lr.Status == LabRequestStatus.InProgress)
                .Include(lr => lr.Patient)
                    .ThenInclude(p => p.User)
                .Include(lr => lr.Doctor)
                .Include(lr => lr.Laboratory)
                    .ThenInclude(l => l.User)
                .AsNoTracking()
                .OrderBy(lr => lr.RequestDate)
                .ToListAsync();
        }

        public async Task<string> GenerateRequestNumberAsync()
        {
            var year = DateTime.Now.Year;
            var lastRequest = await context.LabRequests
                .Where(lr => lr.RequestNumber.StartsWith($"LAB-{year}"))
                .OrderByDescending(lr => lr.Id)
                .FirstOrDefaultAsync();

            if (lastRequest == null)
            {
                return $"LAB-{year}-0001";
            }

            var lastNumber = int.Parse(lastRequest.RequestNumber.Split('-')[2]);
            return $"LAB-{year}-{(lastNumber + 1):D4}";
        }
    }
}


