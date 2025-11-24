using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface ILabRequestRepository : IGenericRepository<LabRequest>
    {
        Task<LabRequest?> GetLabRequestWithDetailsAsync(int id);
        Task<IEnumerable<LabRequest>> GetLabRequestsByDoctorAsync(string doctorId);
        Task<IEnumerable<LabRequest>> GetLabRequestsByPatientAsync(string patientUserId);
        Task<IEnumerable<LabRequest>> GetLabRequestsByLaboratoryAsync(int laboratoryId);
        Task<IEnumerable<LabRequest>> GetPendingLabRequestsAsync();
        Task<string> GenerateRequestNumberAsync();
    }
}



