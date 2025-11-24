using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.PLL.Areas.TreatmentCaseService
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("CaseTransfers")]
    [Authorize(Roles ="SubDoctor,HeadDoctor")]
    public class CaseTransfersController : ControllerBase
    {
        private readonly ICaseTransferService _caseTransferService;

        public CaseTransfersController(ICaseTransferService caseTransferService)
        {
            _caseTransferService = caseTransferService;
        }

        [HttpGet("case-transfer/{id}")]
        public async Task<IActionResult> GetCaseTransferById(int id)
        {
            var result = await _caseTransferService.GetCaseTransferByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("Case-Transfers-ByPatient/{patientId}")]
        public async Task<IActionResult> GetCaseTransfersByPatient(string patientId)
        {
            var result = await _caseTransferService.GetCaseTransfersByPatientAsync(patientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        
        [HttpGet("Case-Transfers-By-Doctor/{doctorId}")]
        public async Task<IActionResult> GetCaseTransfersByDoctor(string doctorId)
        {
            // SubDoctor can only view their own transfers
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "SubDoctor" && userId != doctorId)
            {
                return Forbid();
            }

            var result = await _caseTransferService.GetCaseTransfersByDoctorAsync(doctorId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("pending-case-transfers")]
        public async Task<IActionResult> GetPendingCaseTransfers()
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await _caseTransferService.GetPendingCaseTransfersAsync(doctorId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCaseTransfer([FromBody] CaseTransferRequestDTO request)
        {
            var fromDoctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await _caseTransferService.CreateCaseTransferAsync(request, fromDoctorId);
            return result.Success ? CreatedAtAction(nameof(GetCaseTransferById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPatch("Update-Status/{id}")]
        public async Task<IActionResult> UpdateCaseTransferStatus(int id, [FromBody] CaseTransferStatusDTO request)
        {
            var result = await _caseTransferService.UpdateCaseTransferStatusAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

       


    }

}
