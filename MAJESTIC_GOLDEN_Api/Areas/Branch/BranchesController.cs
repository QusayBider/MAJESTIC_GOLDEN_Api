using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MAJESTIC_GOLDEN_Api.PLL.Areas.Branches
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Branch")]
    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        
        [HttpGet("Get_all_branches")]
        [Authorize(Roles = "HeadDoctor,Branches_Admin")]
        public async Task<IActionResult> GetAllBranches()
        {
            var result = await _branchService.GetAllBranchesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveBranches()
        {
            var result = await _branchService.GetActiveBranchesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            var result = await _branchService.GetBranchByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

       
        [HttpPost]
        [Authorize(Roles = "HeadDoctor,Branches_Admin")]
        public async Task<IActionResult> CreateBranch([FromBody] BranchRequestDTO request)
        {
            var result = await _branchService.CreateBranchAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetBranchById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "HeadDoctor,Branches_Admin")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] UpdateBranchRequestDTO request)
        {
            var result = await _branchService.UpdateBranchAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "HeadDoctor,Branches_Admin")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _branchService.DeleteBranchAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
