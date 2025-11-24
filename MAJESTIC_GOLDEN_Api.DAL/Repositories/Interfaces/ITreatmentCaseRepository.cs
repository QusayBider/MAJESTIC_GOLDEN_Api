using MAJESTIC_GOLDEN_Api.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface ITreatmentCaseRepository : IGenericRepository<TreatmentCase>
    {
        Task<TreatmentCase?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<TreatmentCase>> GetByPatientIdAsync(string patientUserId);
        Task<IEnumerable<TreatmentCase>> GetByDoctorIdAsync(string doctorId);
        Task<IEnumerable<TreatmentCase>> GetByStatusAsync(string status);
        Task<IEnumerable<TreatmentCase>> GetByBranchIdAsync(int branchId);
        Task<string> GenerateCaseNumberAsync();
        Task<IEnumerable<TreatmentCase>> GetUpcomingVisitsAsync(DateTime fromDate, DateTime toDate);
    }
}


