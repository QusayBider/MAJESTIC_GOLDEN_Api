using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;

namespace MAJESTIC_GOLDEN_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> GetDashboardStatistics()
        {
            var result = await _dashboardService.GetDashboardStatisticsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("statistics")]
        [Authorize(Roles = "HeadDoctor,Receptionist")]
        public async Task<IActionResult> GetStatisticsSummary()
        {
            var result = await _dashboardService.GetStatisticsSummaryAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpGet("financial")]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> GetFinancialSummary([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _dashboardService.GetFinancialSummaryAsync(startDate, endDate);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("branches")]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> GetBranchStatistics()
        {
            var result = await _dashboardService.GetBranchStatisticsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}







