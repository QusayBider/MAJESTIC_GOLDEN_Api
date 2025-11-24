using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IBranchService
    {
        Task<ApiResponse<BranchResponseDTO>> CreateBranchAsync(BranchRequestDTO request);
        Task<ApiResponse<BranchResponseDTO>> UpdateBranchAsync(int id, UpdateBranchRequestDTO request);
        Task<ApiResponse<bool>> DeleteBranchAsync(int id);
        Task<ApiResponse<BranchResponseDTO>> GetBranchByIdAsync(int id);
        Task<ApiResponse<IEnumerable<BranchResponseDTO>>> GetAllBranchesAsync();
        Task<ApiResponse<IEnumerable<BranchResponseDTO>>> GetActiveBranchesAsync();
    }
}







