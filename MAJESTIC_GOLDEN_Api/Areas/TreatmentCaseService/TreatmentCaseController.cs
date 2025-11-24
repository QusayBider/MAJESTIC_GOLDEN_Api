using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.PLL.Areas.TreatmentCaseService
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("TreatmentCaseService")]
    [Authorize(Roles = "SubDoctor,HeadDoctor,TreatmentCase_Admin")]
    public class TreatmentCaseController : ControllerBase
    {
        private readonly ITreatmentCaseService _treatmentCaseService;

        public TreatmentCaseController(ITreatmentCaseService treatmentCaseService)
        {
            _treatmentCaseService = treatmentCaseService;
        }

        private string GetCurrentDoctorId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpGet("my-treatment-cases")]
        [Authorize(Roles = "SubDoctor,HeadDoctor")]
        public async Task<IActionResult> GetMyCases()
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                var result = await _treatmentCaseService.GetCasesByDoctorIdAsync(doctorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve cases",
                    Message_Ar = "فشل في استرجاع الحالات",
                    Errors = new List<string> { ex.Message }
                });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCaseById(int id)
        {
            try
            {
                var result = await _treatmentCaseService.GetCaseByIdAsync(id);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                var doctorId = GetCurrentDoctorId();
                var isHeadDoctor = User.IsInRole("HeadDoctor") || User.IsInRole("TreatmentCase_Admin");


                if (!isHeadDoctor && !result.Data!.Doctors.Any(d => d.DoctorId == doctorId))
                {
                    return Forbid();
                }

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


        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetCaseDetails(int id)
        {
            try
            {
                var result = await _treatmentCaseService.GetCaseDetailsByIdAsync(id);

                if (!result.Success)
                {
                    return NotFound(result);
                }

                var doctorId = GetCurrentDoctorId();
                var isHeadDoctor = User.IsInRole("HeadDoctor") || User.IsInRole("TreatmentCase_Admin");


                if (!isHeadDoctor && !result.Data!.Doctors.Any(d => d.DoctorId == doctorId))
                {
                    return Forbid();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve case details",
                    Message_Ar = "فشل في استرجاع تفاصيل الحالة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("Get_cases_by_patient/{patientId}")]
        [Authorize(Roles = "SubDoctor,HeadDoctor,TreatmentCase_Admin,Patient")]
        public async Task<IActionResult> GetCasesByPatient(string patientId)
        {
            try
            {
                var result = await _treatmentCaseService.GetCasesByPatientIdAsync(patientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve patient cases",
                    Message_Ar = "فشل في استرجاع حالات المريض",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPatch("Update_case_status/{id}")]
        public async Task<IActionResult> UpdateCaseStatus(int id, [FromBody] UpdateCaseStatusDTO request)
        {
            try
            {
                // Verify doctor is involved
                var caseResult = await _treatmentCaseService.GetCaseByIdAsync(id);
                if (!caseResult.Success)
                {
                    return NotFound(caseResult);
                }

                var doctorId = GetCurrentDoctorId();
                var isHeadDoctor = User.IsInRole("HeadDoctor") || User.IsInRole("TreatmentCase_Admin");

                if (!isHeadDoctor && !caseResult.Data!.Doctors.Any(d => d.DoctorId == doctorId))
                {
                    return Forbid();
                }

                var result = await _treatmentCaseService.UpdateCaseStatusAsync(id, request);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to update case status",
                    Message_Ar = "فشل في تحديث حالة الحالة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }


        [HttpGet("upcoming-visits")]
        [Authorize(Roles = "SubDoctor,HeadDoctor")]
        public async Task<IActionResult> GetUpcomingVisits([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.Today;
                var to = toDate ?? DateTime.Today.AddDays(30);

                var result = await _treatmentCaseService.GetUpcomingVisitsAsync(from, to);

                var doctorId = GetCurrentDoctorId();
                var isHeadDoctor = User.IsInRole("HeadDoctor");


                if (!isHeadDoctor)
                {
                    result.Data = result.Data?.Where(c => c.Doctors.Any(d => d.DoctorId == doctorId)).ToList();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve upcoming visits",
                    Message_Ar = "فشل في استرجاع الزيارات القادمة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("Get_cases_in_progress")]
        public async Task<IActionResult> GetInProgressCases()
        {
            try
            {
                var result = await _treatmentCaseService.GetCasesByStatusAsync("InProgress");

                var doctorId = GetCurrentDoctorId();
                var isHeadDoctor = User.IsInRole("HeadDoctor") || User.IsInRole("TreatmentCase_Admin");


                if (!isHeadDoctor)
                {
                    result.Data = result.Data?.Where(c => c.Doctors.Any(d => d.DoctorId == doctorId)).ToList();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve in progress cases",
                    Message_Ar = "فشل في استرجاع الحالات قيد المعالجة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("Doctor-Cases-ByPatient/{patientId}")]
        [Authorize(Roles = "SubDoctor,HeadDoctor")]
        public async Task<IActionResult> GetMyCasesWithPatient(string patientId)
        {
            try
            {
                var doctorId = GetCurrentDoctorId();

                var myCasesResult = await _treatmentCaseService.GetCasesByDoctorIdAsync(doctorId);

                if (!myCasesResult.Success)
                {
                    return BadRequest(myCasesResult);
                }

                // Filter by patient
                var filteredCases = myCasesResult.Data?
                    .Where(c => c.PatientId == patientId)
                    .OrderByDescending(c => c.CaseDate)
                    .ToList();

                return Ok(new ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>
                {
                    Success = true,
                    Message_En = $"Your cases with patient retrieved successfully",
                    Message_Ar = $"تم استرجاع حالاتك مع المريض بنجاح",
                    Data = filteredCases ?? new List<TreatmentCaseResponseDTO>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve cases",
                    Message_Ar = "فشل في استرجاع الحالات",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("Get_case_statistics_for_doctor")]
        [Authorize(Roles = "SubDoctor,HeadDoctor")]
        public async Task<IActionResult> GetMyStatistics()
        {
            try
            {
                var doctorId = GetCurrentDoctorId();
                var allCasesResult = await _treatmentCaseService.GetCasesByDoctorIdAsync(doctorId);

                if (!allCasesResult.Success || allCasesResult.Data == null)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message_En = "Statistics retrieved successfully",
                        Message_Ar = "تم استرجاع الإحصائيات بنجاح",
                        Data = new
                        {
                            TotalCases = 0,
                            OpenCases = 0,
                            InProgressCases = 0,
                            CompletedCases = 0,
                            OnHoldCases = 0,
                            TotalRevenue = 0m,
                            TotalPatients = 0
                        }
                    });
                }

                var cases = allCasesResult.Data.ToList();

                var statistics = new
                {
                    TotalCases = cases.Count,
                    OpenCases = cases.Count(c => c.Status == "Open"),
                    InProgressCases = cases.Count(c => c.Status == "InProgress"),
                    CompletedCases = cases.Count(c => c.Status == "Completed"),
                    OnHoldCases = cases.Count(c => c.Status == "OnHold"),
                    TotalRevenue = cases.Sum(c => c.TotalAmount),
                    TotalPatients = cases.Select(c => c.PatientId).Distinct().Count(),
                    RecentCases = cases.OrderByDescending(c => c.CaseDate).Take(5).Select(c => new
                    {
                        c.Id,
                        c.CaseNumber,
                        c.PatientName_En,
                        c.PatientName_Ar,
                        c.Status,
                        c.CaseDate,
                        c.TotalAmount
                    })
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message_En = "Statistics retrieved successfully",
                    Message_Ar = "تم استرجاع الإحصائيات بنجاح",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve statistics",
                    Message_Ar = "فشل في استرجاع الإحصائيات",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("doctor/{doctorId}/patient/{patientId}")]
        [Authorize(Roles = "HeadDoctor,TreatmentCase_Admin")]
        public async Task<IActionResult> GetCasesByDoctorAndPatient(string doctorId, string patientId)
        {
            try
            {
                var currentDoctorId = GetCurrentDoctorId();
                var isHeadDoctor = User.IsInRole("HeadDoctor") || User.IsInRole("TreatmentCase_Admin");


                if (!isHeadDoctor && currentDoctorId != doctorId)
                {
                    return Forbid();
                }

                var doctorCasesResult = await _treatmentCaseService.GetCasesByDoctorIdAsync(doctorId);

                if (!doctorCasesResult.Success)
                {
                    return BadRequest(doctorCasesResult);
                }

                var filteredCases = doctorCasesResult.Data?
                    .Where(c => c.PatientId == patientId)
                    .OrderByDescending(c => c.CaseDate)
                    .ToList();

                return Ok(new ApiResponse<IEnumerable<TreatmentCaseResponseDTO>>
                {
                    Success = true,
                    Message_En = $"Cases retrieved successfully for doctor and patient",
                    Message_Ar = $"تم استرجاع الحالات بنجاح للطبيب والمريض",
                    Data = filteredCases ?? new List<TreatmentCaseResponseDTO>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve cases",
                    Message_Ar = "فشل في استرجاع الحالات",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("create-treatment-case")]
        [Authorize(Roles = "SubDoctor,HeadDoctor")]
        public async Task<IActionResult> CreateTreatmentCase([FromBody] TreatmentCaseRequestDTO request)
        {
            try
            {
                var DoctorId = GetCurrentDoctorId();
                var result = await _treatmentCaseService.CreateCaseAsync(request, DoctorId);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to create treatment case",
                    Message_Ar = "فشل في إنشاء حالة العلاج",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("delete-treatment-case/{id}")]
        [Authorize(Roles = "SubDoctor,HeadDoctor,TreatmentCase_Admin")]
        public async Task<IActionResult> DeleteTreatmentCase(int id)
        {
            try
            {
                var DoctorId = GetCurrentDoctorId();
                var isHeadDoctor = User.IsInRole("HeadDoctor") || User.IsInRole("TreatmentCase_Admin");
                var caseResult = await _treatmentCaseService.GetCaseByIdAsync(id);
                if (!isHeadDoctor)
                {

                    foreach (var doc in caseResult.Data.Doctors)
                    {
                        if (doc.DoctorId != DoctorId)
                        {
                            return Forbid();
                        }
                    }

                }
                var result = await _treatmentCaseService.DeleteCaseAsync(id);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to delete treatment case",
                    Message_Ar = "فشل في حذف حالة العلاج",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("remove-doctor-from-case/{caseId}/{doctorId}")]
        [Authorize(Roles = "SubDoctor,HeadDoctor,TreatmentCase_Admin")]
        public async Task<IActionResult> RemoveDoctorFromCase(int caseId, string doctorId)
        {
            try
            {
                var isHeadDoctor = User.IsInRole("HeadDoctor") || User.IsInRole("TreatmentCase_Admin");
                if (!isHeadDoctor)
                {
                    return Forbid();
                }
                var result = await _treatmentCaseService.RemoveDoctorFromCaseAsync(caseId, doctorId);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to remove doctor from case",
                    Message_Ar = "فشل في إزالة الطبيب من الحالة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("add-doctor-to-case/{caseId}/{doctorId}")]
        [Authorize(Roles = "SubDoctor,HeadDoctor,TreatmentCase_Admin")]
        public async Task<IActionResult> AddDoctorToCase(int caseId, string doctorId)
        {
            try
            {
                var isHeadDoctor = User.IsInRole("HeadDoctor") || User.IsInRole("TreatmentCase_Admin");
                var caseResult = await _treatmentCaseService.GetCaseByIdAsync(caseId);

                if (!isHeadDoctor)
                {
                    foreach (var doc in caseResult.Data.Doctors)
                    {
                        if (doc.DoctorId != GetCurrentDoctorId())
                        {
                            return Forbid();
                        }
                    }
                }
                var result = await _treatmentCaseService.AddDoctorToCaseAsync(caseId, doctorId);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to add doctor to case",
                    Message_Ar = "فشل في إضافة الطبيب إلى الحالة",
                    Errors = new List<string> { ex.Message }
                });
            }

        }
    }
}
