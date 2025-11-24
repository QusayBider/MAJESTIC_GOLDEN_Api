using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IServiceManagementService
    {
        Task<ApiResponse<ServiceResponseDTO>> CreateServiceAsync(ServiceRequestDTO request);
        Task<ApiResponse<ServiceResponseDTO>> UpdateServiceAsync(int id, ServiceRequestDTO request);
        Task<ApiResponse<bool>> DeleteServiceAsync(int id);
        Task<ApiResponse<ServiceResponseDTO>> GetServiceByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ServiceResponseDTO>>> GetAllServicesAsync();
        Task<ApiResponse<IEnumerable<ServiceResponseDTO>>> GetActiveServicesAsync();
        Task<ApiResponse<IEnumerable<ServiceResponseDTO>>> GetServicesByCategoryAsync(string category);
    }
}







