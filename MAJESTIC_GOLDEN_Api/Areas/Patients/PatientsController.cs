using MAJESTIC_GOLDEN_Api.BLL.Services.Classes;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.PLL.Areas.Patients
{
    [Area("Patients")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Patients_Admin")]

    public class PatientsController : ControllerBase
    {
        private readonly ITreatmentCaseService _treatmentCaseService;
        private readonly IPatientService _patientService;

        public PatientsController(ITreatmentCaseService treatmentCaseService, IPatientService patientService)
        {
            _treatmentCaseService = treatmentCaseService;
            _patientService = patientService;
        }

        private async Task<IActionResult> GetCaseById(string id)
        {
            try
            {
                var result = await _patientService.GetPatientByIdAsync(id);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                var doctorId = GetCurrentDoctorId();
                var isHeadDoctor = User.IsInRole("HeadDoctor");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve case",
                    Message_Ar = "فشل في استرجاع الحالة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        private string GetCurrentDoctorId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpGet("Get_patient_by_ID/{id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Patients_Admin,Patient")]
        public async Task<IActionResult> GetPatientById(string id)
        {
            // Patient can only view their own data
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Patient")
            {
                var patientId = User.FindFirst("PatientId")?.Value ?? "";
                if (patientId != id)
                {
                    return Forbid();
                }
            }

            var result = await _patientService.GetPatientByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }
        [HttpPost("Create_a_new_patient")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientRequestDTO request)
        {
            var result = await _patientService.CreatePatientAsync(request);
            return result.Success ? CreatedAtAction(nameof(GetPatientById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPut("Update_a_patient/{id}")]
        public async Task<IActionResult> UpdatePatient(string id, [FromBody] PatientRequestDTO request)
        {
            var result = await _patientService.UpdatePatientAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [Authorize(Roles = "HeadDoctor,Patients_Admin,SubDoctor,Receptionist")]
        [HttpPut("Admin_reset_password/{userId}")]
        public async Task<IActionResult> AdminResetPassword(string userId, [FromBody] AdminResetPasswordRequestDTO request)
        {
            var result = await _patientService.AdminResetPasswordAsync(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "HeadDoctor,Patients_Admin")]
        [HttpGet("Get_all_patients")]
        public async Task<IActionResult> GetAllPatients()
        {
            var result = await _patientService.GetAllPatientsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
        
        [Authorize(Roles = "HeadDoctor,Patients_Admin")]
        [HttpGet("Get_patients_by_branch/{branchId}")]
        public async Task<IActionResult> GetPatientsByBranch(int branchId)
        {
            var result = await _patientService.GetPatientsByBranchAsync(branchId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("Search_patients")]
        public async Task<IActionResult> SearchPatients([FromQuery] string searchTerm)
        {
            var result = await _patientService.SearchPatientsAsync(searchTerm);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "HeadDoctor,SubDoctor,Patients_Admin")]
        [HttpPost("Create_new_treatment_case")]
        public async Task<IActionResult> CreateCase([FromBody] TreatmentCaseRequestDTO request)
        {
            try
            {
                var doctorId = GetCurrentDoctorId();

                // Clean up empty or null doctor IDs from the list
                if (request.DoctorIds != null)
                {
                    request.DoctorIds = request.DoctorIds
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .ToList();
                }

                // Add current doctor to the case doctors list automatically if not present
                if (request.DoctorIds == null || !request.DoctorIds.Any())
                {
                    request.DoctorIds = new List<string> { doctorId };
                }
                else if (!request.DoctorIds.Contains(doctorId))
                {
                    // Make sure current doctor is in the list
                    request.DoctorIds.Insert(0, doctorId);
                }

                var result = await _treatmentCaseService.CreateCaseAsync(request, doctorId);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to create case",
                    Message_Ar = "فشل في إنشاء الحالة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        
    }
}
