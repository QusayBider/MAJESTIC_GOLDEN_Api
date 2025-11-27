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

                if (request.DoctorIds != null)
                {
                    request.DoctorIds = request.DoctorIds
                        .Where(id => !string.IsNullOrWhiteSpace(id))
                        .ToList();
                }

                if (request.DoctorIds == null || !request.DoctorIds.Any())
                {
                    request.DoctorIds = new List<string> { doctorId };
                }
                else if (!request.DoctorIds.Contains(doctorId))
                {
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

        [HttpPost("UploadFilePatient/{patient_Id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Patients_Admin,Laboratory")]
        public async Task<IActionResult> UploadFilePatient(string patient_Id, [FromForm] UploadFileRequestDTO request)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "File is required",
                        Message_Ar = "الملف مطلوب"
                    });
                }

                var uploadedBy = GetCurrentDoctorId();

                
                var result = await _patientService.UploadFileToPatientAsync(patient_Id, request.File, request, uploadedBy);

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to upload file",
                    Message_Ar = "فشل في رفع الملف",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("DeleteFilePatient/{file_Id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Patients_Admin,Laboratory")]
        public async Task<IActionResult> DeleteFilePatient(int file_Id)
        {
            try
            {
                var deletedBy = GetCurrentDoctorId();
                var result = await _patientService.DeleteFileAsync(file_Id, deletedBy);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to delete file",
                    Message_Ar = "فشل في حذف الملف",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("GetPatientFiles/{patient_Id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Patients_Admin,Laboratory,Patient")]
        public async Task<IActionResult> GetPatientFiles(string patient_Id)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Patient")
                {
                    var currentPatientId = User.FindFirst("PatientId")?.Value ?? "";
                    if (currentPatientId != patient_Id)
                    {
                        return Forbid();
                    }
                }
                

                var result = await _patientService.GetPatientFilesAsync(Request, patient_Id);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve files",
                    Message_Ar = "فشل في استرجاع الملفات",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("GetFileById/{file_Id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor,Receptionist,Patients_Admin,Laboratory,Patient")]
        public async Task<IActionResult> GetFileById(int file_Id)
        {
            try
            {
                var result = await _patientService.GetFileByIdAsync(Request, file_Id);
                
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Patient" && result.Success && result.Data != null)
                {
                    var currentPatientId = User.FindFirst("PatientId")?.Value ?? "";
                    if (currentPatientId != result.Data.PatientUserId)
                    {
                        return Forbid();
                    }
                }

                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve file",
                    Message_Ar = "فشل في استرجاع الملف",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        
    }
}
