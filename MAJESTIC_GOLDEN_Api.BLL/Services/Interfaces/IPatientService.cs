using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IPatientService
    {
        Task<ApiResponse<PatientResponseDTO>> CreatePatientAsync(PatientRequestDTO request);
        Task<ApiResponse<PatientResponseDTO>> UpdatePatientAsync(string userId, PatientRequestDTO request);
        Task<ApiResponse<bool>> DeletePatientAsync(string userId);
        Task<ApiResponse<PatientDetailedResponseDTO>> GetPatientByIdAsync(string userId);
        Task<ApiResponse<IEnumerable<PatientResponseDTO>>> GetAllPatientsAsync();
        Task<ApiResponse<IEnumerable<PatientResponseDTO>>> GetPatientsByBranchAsync(int branchId);
        Task<ApiResponse<IEnumerable<PatientResponseDTO>>> SearchPatientsAsync(string searchTerm);
        Task<ApiResponse<bool>> AdminResetPasswordAsync(string userId, AdminResetPasswordRequestDTO request);
    }
}



