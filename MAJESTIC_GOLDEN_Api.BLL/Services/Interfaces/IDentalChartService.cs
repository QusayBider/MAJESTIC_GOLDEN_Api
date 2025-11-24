using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IDentalChartService
    {
        Task<ApiResponse<PatientToothResponseDTO>> AddOrUpdateToothAsync(PatientToothRequestDTO request, string doctorId);
        Task<ApiResponse<DentalChartResponseDTO>> GetPatientDentalChartAsync(string patientUserId);
        Task<ApiResponse<PatientToothResponseDTO>> GetToothByCompositeKeyAsync(int toothId, string patientUserId);
        Task<ApiResponse<IEnumerable<PatientToothResponseDTO>>> GetTeethByPatientAsync(string patientUserId);
        Task<ApiResponse<bool>> DeleteToothRecordAsync(int toothId, string patientUserId);
    }
}


