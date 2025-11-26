using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LaboratoriesController : ControllerBase
    {
        private readonly ILaboratoryService _laboratoryService;

        public LaboratoriesController(ILaboratoryService laboratoryService)
        {
            _laboratoryService = laboratoryService;
        }

        [HttpGet]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> GetAllLaboratories()
        {
            var result = await _laboratoryService.GetAllLaboratoriesAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Laboratory")]
        public async Task<IActionResult> GetLaboratoryById(int id)
        {
            if (User.IsInRole("Laboratory"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var labResult = await _laboratoryService.GetLaboratoryByIdAsync(id);
                if (!labResult.Success)
                {
                    return NotFound(labResult);
                }

                if (labResult.Data?.UserId != userId)
                {
                    return Forbid();
                }

                return Ok(labResult);
            }

            var result = await _laboratoryService.GetLaboratoryByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> GetLaboratoryByUserId(string userId)
        {
            var result = await _laboratoryService.GetLaboratoryByUserIdAsync(userId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Laboratory")]
        public async Task<IActionResult> GetMyLaboratoryProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _laboratoryService.GetLaboratoryByUserIdAsync(userId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> CreateLaboratory([FromBody] LaboratoryCreateDTO request)
        {
            var result = await _laboratoryService.CreateLaboratoryAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetLaboratoryById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HeadDoctor,Laboratory")]
        public async Task<IActionResult> UpdateLaboratory(int id, [FromBody] LaboratoryUpdateDTO request)
        {
            if (User.IsInRole("Laboratory"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var labResult = await _laboratoryService.GetLaboratoryByIdAsync(id);
                if (!labResult.Success)
                {
                    return NotFound(labResult);
                }

                if (labResult.Data?.UserId != userId)
                {
                    return Forbid();
                }
            }

            var result = await _laboratoryService.UpdateLaboratoryAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> DeleteLaboratory(int id)
        {
            var result = await _laboratoryService.DeleteLaboratoryAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
    }
}




