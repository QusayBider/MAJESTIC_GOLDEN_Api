using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface ILaboratoryService
    {
        Task<ApiResponse<LaboratoryResponseDTO>> CreateLaboratoryAsync(LaboratoryCreateDTO request);
        Task<ApiResponse<LaboratoryResponseDTO>> UpdateLaboratoryAsync(int id, LaboratoryUpdateDTO request);
        Task<ApiResponse<bool>> DeleteLaboratoryAsync(int id);
        Task<ApiResponse<LaboratoryResponseDTO>> GetLaboratoryByIdAsync(int id);
        Task<ApiResponse<LaboratoryResponseDTO>> GetLaboratoryByUserIdAsync(string userId);
        Task<ApiResponse<IEnumerable<LaboratoryResponseDTO>>> GetAllLaboratoriesAsync();
    }
}




