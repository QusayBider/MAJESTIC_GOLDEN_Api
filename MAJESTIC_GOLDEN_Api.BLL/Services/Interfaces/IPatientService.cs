using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using Microsoft.AspNetCore.Http;

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
        Task<ApiResponse<UploadFileResponseDTO>> UploadFileToPatientAsync(string patientUserId, IFormFile file, UploadFileRequestDTO request, string? uploadedBy = null);
        Task<ApiResponse<bool>> DeleteFileAsync(int fileId, string? deletedBy = null);
        Task<ApiResponse<IEnumerable<UploadFileResponseDTO>>> GetPatientFilesAsync(HttpRequest request, string patientUserId);
        Task<ApiResponse<UploadFileResponseDTO>> GetFileByIdAsync(HttpRequest request, int fileId);
    }
}



