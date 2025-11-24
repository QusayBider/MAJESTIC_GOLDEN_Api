using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.PLL.Areas.Appointments
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Appointments")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IPatientService patientService, IAppointmentService appointmentService)
        {
            _patientService = patientService;
            _appointmentService = appointmentService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        [HttpPost("CreateAppointment")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Appointments_Admin")]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequestDTO request)
        {
            var userId = GetCurrentUserId();
            var result = await _appointmentService.CreateAppointmentAsync(request, userId);
            return result.Success ? CreatedAtAction(nameof(GetAppointmentById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }


        [HttpPost("CreatePatientsAppointment")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreatePatientsAppointment([FromBody] AppointmentPatientRequestDTO request)
        {
            var userId = GetCurrentUserId();
            request.PatientUserId = userId;
            AppointmentRequestDTO requestDTO = request.Adapt<AppointmentRequestDTO>();
            var result = await _appointmentService.CreateAppointmentAsync(requestDTO, userId);
            return result.Success ? CreatedAtAction(nameof(GetAppointmentById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }

        [HttpPost("CreateSubDoctorAppointment")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> CreateSubDoctorAppointment([FromBody] AppointmentSubDoctorRequestDTO request)
        {
            var userId = GetCurrentUserId();
            request.DoctorId = userId;
            AppointmentRequestDTO requestDTO = request.Adapt<AppointmentRequestDTO>();
            var result = await _appointmentService.CreateAppointmentAsync(requestDTO, userId);
            return result.Success ? CreatedAtAction(nameof(GetAppointmentById), new { id = result.Data?.Id }, result) : BadRequest(result);
        }


        [HttpGet("today")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Appointments_Admin")]
        public async Task<IActionResult> GetTodayAppointments()
        {
            var result = await _appointmentService.GetTodayAppointmentsAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }
  

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Appointments_Admin")]
        public async Task<IActionResult> GetAppointmentsByDoctor(string doctorId, [FromQuery] DateTime? date = null)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userRole == "SubDoctor" && userId != doctorId)
            {
                return Forbid();
            }

            var result = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId, date);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "HeadDoctor,Receptionist,Appointments_Admin")]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatient(string patientId)
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

            var result = await _appointmentService.GetAppointmentsByPatientAsync(patientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "HeadDoctor,Receptionist,Appointments_Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var result = await _appointmentService.GetAppointmentByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }


        [Authorize(Roles = "Patient")]
        [HttpGet("GetPatientAppointment")]
        public async Task<IActionResult> GetPatientAppointments()
        {
            var currentPatientId = GetCurrentUserId();
            var result = await _appointmentService.GetAppointmentsByPatientAsync(currentPatientId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("PatientCancelAppointment/{id}")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CancelPatientAppointment(int id, [FromBody] CancelAppointmentDTO request)
        {
            var userId = GetCurrentUserId();
            var UserAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (!UserAppointment.Success || UserAppointment.Data == null || UserAppointment.Data.PatientUserId != userId)
            {
                return Forbid();
            }
            if (UserAppointment.Data.Status == "Cancelled")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Appointment is already cancelled",
                    Message_Ar = "الموعد ملغي بالفعل",
                    Errors = new List<string> { "Appointment is already cancelled" }
                });
            }
            var result = await _appointmentService.CancelAppointmentAsync(id, userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("CancelSubDoctorAppointment/{id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> CancelSubDoctorAppointment(int id, [FromBody] CancelAppointmentDTO cancelRequest)
        {
            try
            {
                var doctorId = GetCurrentUserId();
                var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (!existingAppointment.Success)
                {
                    return NotFound(existingAppointment);
                }

                if (existingAppointment.Data?.DoctorId != doctorId)
                {
                    return Forbid();
                }
                if (existingAppointment.Data.Status == "Cancelled")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Appointment is already cancelled",
                        Message_Ar = "الموعد ملغي بالفعل",
                        Errors = new List<string> { "Appointment is already cancelled" }
                    });
                }
                var result = await _appointmentService.CancelAppointmentAsync(id, doctorId, cancelRequest);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to cancel appointment",
                    Message_Ar = "فشل في إلغاء الموعد",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        

        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        [HttpGet("Get_all_patients_treated_by_current_doctor")]
        public async Task<IActionResult> GetMyPatients()
        {
            try
            {
                var doctorId = GetCurrentUserId();

                var appointmentsResult = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);

                if (!appointmentsResult.Success || appointmentsResult.Data == null)
                {
                    return Ok(new ApiResponse<List<object>>
                    {
                        Success = true,
                        Message_En = "No patients found",
                        Message_Ar = "لا يوجد مرضى",
                        Data = new List<object>()
                    });
                }


                var uniquePatientIds = appointmentsResult.Data
                    .Select(a => a.PatientUserId)
                    .Distinct()
                    .ToList();


                var patients = new List<PatientResponseDTO>();
                foreach (var patientId in uniquePatientIds)
                {
                    var patientResult = await _patientService.GetPatientByIdAsync(patientId);
                    if (patientResult.Success && patientResult.Data != null)
                    {
                        patients.Add(patientResult.Data);
                    }
                }

                return Ok(new ApiResponse<IEnumerable<PatientResponseDTO>>
                {
                    Success = true,
                    Message_En = "Patients retrieved successfully",
                    Message_Ar = "تم استرجاع المرضى بنجاح",
                    Data = patients
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve patients",
                    Message_Ar = "فشل في استرجاع المرضى",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        [HttpGet("SubDoctorTodayAppointments")]
        public async Task<IActionResult> GetSubDoctorTodayAppointments()
        {
            try
            {
                var doctorId = GetCurrentUserId();
                var result = await _appointmentService.GetTodayAppointmentsByDoctorAsync(doctorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve today's appointments",
                    Message_Ar = "فشل في استرجاع مواعيد اليوم",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("UpcomingSubDoctorAppointments")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> GetUpcomingSubDoctorAppointments()
        {
            try
            {
                var doctorId = GetCurrentUserId();
                var result = await _appointmentService.GetUpcomingAppointmentsByDoctorAsync(doctorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve upcoming appointments",
                    Message_Ar = "فشل في استرجاع المواعيد القادمة",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("GetSubDoctorAppointments")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> GetMyAppointments()
        {
            try
            {
                var doctorId = GetCurrentUserId();
                var result = await _appointmentService.GetAppointmentsByDoctorAsync(doctorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to retrieve appointments",
                    Message_Ar = "فشل في استرجاع المواعيد",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("UpdateSubDoctorAppointment/{id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> UpdateSubDoctorAppointment(int id, [FromBody] UpdateAppointmentStatusRequestDTO request)
        {
            try
            {
                var doctorId = GetCurrentUserId();

                var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (!existingAppointment.Success)
                {
                    return NotFound(existingAppointment);
                }

                if (existingAppointment.Data?.DoctorId != doctorId)
                {
                    return Forbid();
                }

                if (existingAppointment.Data.Status == "Cancelled")
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Cannot update a cancelled appointment",
                        Message_Ar = "لا يمكن تحديث موعد ملغي",
                        Errors = new List<string> { "Cannot update a cancelled appointment" }
                    });
                }

                if (request.AppointmentDateTime <= DateTime.Today)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message_En = "Appointment date and time cannot be in the past",
                        Message_Ar = "لا يمكن أن يكون تاريخ ووقت الموعد في الماضي",
                        Errors = new List<string> { "Appointment date and time cannot be in the past" }
                    });
                }
                request.DoctorId = doctorId;
                request.PatientUserId = existingAppointment.Data.PatientUserId;
                request.Source = existingAppointment.Data.Source;
                AppointmentRequestDTO requestDTO = request.Adapt<AppointmentRequestDTO>();
                var result = await _appointmentService.UpdateAppointmentAsync(id, requestDTO);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Failed to update appointment",
                    Message_Ar = "فشل في تحديث الموعد",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("UpdateAnAppointment/{id}")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Appointments_Admin")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] UpdateAppointmentStatusRequestDTO request)
        {
            var userRoles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            bool isAdmin = false;
            foreach (var role in userRoles)
            {
                if (role == "HeadDoctor" || role == "Appointments_Admin")
                {
                    isAdmin = true;
                }
            }
            var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (!existingAppointment.Success)
            {
                return NotFound(existingAppointment);
            }
            if (existingAppointment.Data.Status == "Cancelled")
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Cannot update a cancelled appointment",
                    Message_Ar = "لا يمكن تحديث موعد ملغي",
                    Errors = new List<string> { "Cannot update a cancelled appointment" }
                });
            }

            if (request.AppointmentDateTime < DateTime.Today && !isAdmin)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Appointment date and time cannot be in the past",
                    Message_Ar = "لا يمكن أن يكون تاريخ ووقت الموعد في الماضي",
                    Errors = new List<string> { "Appointment date and time cannot be in the past" }
                });
            }
            AppointmentRequestDTO requestDTO = request.Adapt<AppointmentRequestDTO>();
            requestDTO.DoctorId = existingAppointment.Data.DoctorId;
            requestDTO.PatientUserId = existingAppointment.Data.PatientUserId;
            requestDTO.Source = existingAppointment.Data.Source;
            var result = await _appointmentService.UpdateAppointmentAsync(id, requestDTO);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("UpdateSubDoctorAppointmentStatus/{id}")]
        [Authorize(Roles = "HeadDoctor,SubDoctor")]
        public async Task<IActionResult> UpdateSubDoctorAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDTO request)
        {

            var doctorId = GetCurrentUserId();

            var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (!existingAppointment.Success)
            {
                return NotFound(existingAppointment);
            }

            if (existingAppointment.Data?.DoctorId != doctorId)
            {
                return Forbid();
            }
            if (existingAppointment.Data.AppointmentDateTime < DateTime.Today) { 
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message_En = "Cannot update status of past appointment",
                    Message_Ar = "لا يمكن تحديث حالة موعد في الماضي",
                    Errors = new List<string> { "Cannot update status of past appointment" }
                });
            }
            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }


        [HttpPatch("UpdateAppointmentStatus/{id}")]
        [Authorize(Roles = "HeadDoctor,Receptionist,Appointments_Admin")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusDTO request)
        {
            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }


    }
}
