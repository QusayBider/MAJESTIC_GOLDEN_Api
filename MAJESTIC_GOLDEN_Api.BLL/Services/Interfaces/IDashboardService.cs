using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardResponseDTO>> GetDashboardStatisticsAsync();
        Task<ApiResponse<DashboardStatistics>> GetStatisticsSummaryAsync();
        Task<ApiResponse<FinancialSummary>> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResponse<IEnumerable<BranchStatistics>>> GetBranchStatisticsAsync();
    }
}







