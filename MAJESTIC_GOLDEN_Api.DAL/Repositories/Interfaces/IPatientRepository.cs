using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface IPatientRepository : IGenericRepository<Patient>
    {
        Task<Patient?> GetPatientWithDetailsAsync(string userId);
        Task<Patient?> GetPatientForUpdateAsync(string userId); // New method for updates (with tracking)
        Task<IEnumerable<Patient>> GetPatientsByBranchAsync(int branchId);
        Task<Patient?> GetPatientByPhoneAsync(string phone);
        Task<Patient?> GetPatientByEmailAsync(string email);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
    }
}



