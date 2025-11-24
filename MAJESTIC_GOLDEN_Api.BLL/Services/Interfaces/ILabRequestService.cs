using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface ILabRequestService
    {
        Task<ApiResponse<LabRequestResponseDTO>> CreateLabRequestAsync(LabRequestCreateDTO request, string doctorId, string? doctorName);
        Task<ApiResponse<LabRequestResponseDTO>> UpdateLabRequestAsync(int id, LabRequestUpdateDTO request, string userId, string? userName);
        Task<ApiResponse<LabRequestResponseDTO>> GetLabRequestByIdAsync(int id);
        Task<ApiResponse<IEnumerable<LabRequestResponseDTO>>> GetLabRequestsByDoctorAsync(string doctorId);
        Task<ApiResponse<IEnumerable<LabRequestResponseDTO>>> GetLabRequestsByPatientAsync(string patientUserId);
        Task<ApiResponse<IEnumerable<LabRequestResponseDTO>>> GetLabRequestsByLaboratoryAsync(int laboratoryId);
        Task<ApiResponse<IEnumerable<LabRequestResponseDTO>>> GetPendingLabRequestsAsync();
    }
}



