using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface ICaseTransferService
    {
        Task<ApiResponse<CaseTransferResponseDTO>> CreateCaseTransferAsync(CaseTransferRequestDTO request, string fromDoctorId);
        Task<ApiResponse<CaseTransferResponseDTO>> UpdateCaseTransferStatusAsync(int id, CaseTransferStatusDTO request);
        Task<ApiResponse<CaseTransferResponseDTO>> GetCaseTransferByIdAsync(int id);
        Task<ApiResponse<IEnumerable<CaseTransferResponseDTO>>> GetCaseTransfersByPatientAsync(string patientUserId);
        Task<ApiResponse<IEnumerable<CaseTransferResponseDTO>>> GetCaseTransfersByDoctorAsync(string doctorId);
        Task<ApiResponse<IEnumerable<CaseTransferResponseDTO>>> GetPendingCaseTransfersAsync(string doctorId);
    }
}



