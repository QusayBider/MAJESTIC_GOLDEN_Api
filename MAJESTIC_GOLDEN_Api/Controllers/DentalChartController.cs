using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DentalChartController : ControllerBase
    {
        private readonly IDentalChartService _dentalChartService;

        public DentalChartController(IDentalChartService dentalChartService)
        {
            _dentalChartService = dentalChartService;
        }

        
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientDentalChart(string patientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Patient")
            {
                var currentPatientId = User.FindFirst("PatientId")?.Value ?? "";
                if (currentPatientId != patientId)
                {
                    return Forbid();
                }
            }

            var result = await _dentalChartService.GetPatientDentalChartAsync(patientId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("patient/{patientId}/teeth")]
        public async Task<IActionResult> GetTeethByPatient(string patientId)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Patient")
            {
                var currentPatientId = User.FindFirst("PatientId")?.Value ?? "";
                if (currentPatientId != patientId)
                {
                    return Forbid();
                }
            }

            var result = await _dentalChartService.GetTeethByPatientAsync(patientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("tooth/{toothId}/patient/{patientId}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> GetToothByCompositeKey(int toothId, string patientId)
        {
            var result = await _dentalChartService.GetToothByCompositeKeyAsync(toothId, patientId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> AddOrUpdateTooth([FromBody] PatientToothRequestDTO request)
        {
            var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await _dentalChartService.AddOrUpdateToothAsync(request, doctorId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("tooth/{toothId}/patient/{patientId}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> DeleteToothRecord(int toothId, string patientId)
        {
            var result = await _dentalChartService.DeleteToothRecordAsync(toothId, patientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}


