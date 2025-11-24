using AutoMapper;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Migrations;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBranchRepository _branchRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAuditLogger _auditLogger;

        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper,IUserRepository userRepository,UserManager<ApplicationUser> userManager, IBranchRepository branchRepository, IPatientRepository patientRepository, IAuditLogger auditLogger )
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _userManager = userManager;
            _branchRepository = branchRepository;
            _patientRepository = patientRepository;
            _auditLogger = auditLogger;
        }

        public async Task<ApiResponse<AppointmentResponseDTO>> CreateAppointmentAsync(AppointmentRequestDTO request, string createdBy)
        {
            try
            {
                DateTime appointmentDate = request.AppointmentDateTime;
                DateTime currentTime= DateTime.Now;

                var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(
                    request.DoctorId,
                    request.AppointmentDateTime,
                    request.DurationMinutes
                );

                if (appointmentDate < currentTime) { 
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Appointment date and time must be in the future",
                        "يجب أن يكون تاريخ ووقت الموعد في المستقبل"
                    );
                }

                if (!isAvailable)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Time slot is already booked",
                        "هذا الموعد محجوز بالفعل"
                    );
                }

                var user = await _userRepository.GetUserByIdAsync(createdBy);
                if (user == null)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    );
                }
                request.Source = user.FullName;

                // Validate that the patient exists as a User
                var patient = await _userRepository.GetUserByIdAsync(request.PatientUserId);
                if (patient == null)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Patient not found",
                        "المريض غير موجود"
                    );
                }

                // Validate a Patient profile exists (Patients table uses UserId as PK)
                var patientProfile = await _patientRepository.GetPatientWithDetailsAsync(request.PatientUserId);
                if (patientProfile == null)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Patient profile not found. Please register the user as a patient first",
                        "ملف المريض غير موجود. يرجى تسجيل المستخدم كمريض أولاً"
                    );
                }

                // Validate that the doctor exists
                var doctor = await _userRepository.GetUserByIdAsync(request.DoctorId);
                if (doctor == null)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Doctor not found",
                        "الطبيب غير موجود"
                    );
                }

                // Validate that the branch exists
                var branch = await _branchRepository.GetByIdAsync(request.BranchId);
                if (branch == null)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Branch not found",
                        "الفرع غير موجود"
                    );
                }

                bool itsSubDoctor=false;
                bool its_admin=false;

                var roles = await _userManager.GetRolesAsync(user);

                var appointment = request.Adapt<Appointment>();

                foreach (var role in roles)
                {
                    if (role == "Patient")
                    {
                        appointment.Status = AppointmentStatus.Pending;
                    }
                    if (role == "HeadDoctor" || role == "SubDoctor" || role == "Receptionist" || role == "Appointments_Admin") {
                        appointment.Status = AppointmentStatus.Confirmed;
                    }
                    if (role == "SubDoctor")
                    {
                        if (!its_admin) {
                            itsSubDoctor = true;
                        }
                        
                    }
                    if (role == "Appointments_Admin") {
                        its_admin=true;
                        itsSubDoctor =false;
                    }
                }

                if (itsSubDoctor)
                {
                    if (createdBy != request.DoctorId)
                    {
                       return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                            "Doctor can only book appointments for themselves",
                            "يمكن للطبيب  حجز المواعيد لنفسه فقط"
                        );
                    }
                }

                await _appointmentRepository.AddAsync(appointment);

                var response = _mapper.Map<AppointmentResponseDTO>(appointment);
                return ApiResponse<AppointmentResponseDTO>.SuccessResponse(
                    response,
                    "Appointment booked successfully",
                    "تم حجز الموعد بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                    "Failed to create appointment",
                    "فشل في حجز الموعد",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<AppointmentResponseDTO>> UpdateAppointmentAsync(int id, AppointmentRequestDTO request)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Appointment not found",
                        "الموعد غير موجود"
                    );
                }

                
                var isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(
                    request.DoctorId,
                    request.AppointmentDateTime,
                    request.DurationMinutes,
                    id
                );

                if (!isAvailable)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Time slot is already booked",
                        "هذا الموعد محجوز بالفعل"
                    );
                }

                var oldValues = _mapper.Map<AppointmentResponseDTO>(appointment);

                _mapper.Map(request, appointment);
                await _appointmentRepository.UpdateAsync(appointment);

                var response = _mapper.Map<AppointmentResponseDTO>(appointment);

                await _auditLogger.LogAsync(
                    "Update",
                    nameof(Appointment),
                    appointment.Id.ToString(),
                    oldValues: oldValues,
                    newValues: response);

                return ApiResponse<AppointmentResponseDTO>.SuccessResponse(
                    response,
                    "Appointment updated successfully",
                    "تم تحديث الموعد بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                    "Failed to update appointment",
                    "فشل في تحديث الموعد",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> UpdateAppointmentStatusAsync(int id, UpdateAppointmentStatusDTO request)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Appointment not found",
                        "الموعد غير موجود"
                    );
                }

                var oldValues = new
                {
                    appointment.Status,
                    appointment.Notes_En,
                    appointment.Notes_Ar
                };

                appointment.Status = request.Status;
                if (request.Notes_En != null) appointment.Notes_En = request.Notes_En;
                if (request.Notes_Ar != null) appointment.Notes_Ar = request.Notes_Ar;

                await _appointmentRepository.UpdateAsync(appointment);

                var newValues = new
                {
                    appointment.Status,
                    appointment.Notes_En,
                    appointment.Notes_Ar
                };

                await _auditLogger.LogAsync(
                    "UpdateStatus",
                    nameof(Appointment),
                    appointment.Id.ToString(),
                    oldValues: oldValues,
                    newValues: newValues);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Appointment status updated successfully",
                    "تم تحديث حالة الموعد بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to update appointment status",
                    "فشل في تحديث حالة الموعد",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> CancelAppointmentAsync(int id, string cancelledBy, CancelAppointmentDTO? request = null)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Appointment not found",
                        "الموعد غير موجود"
                    );
                }

                var oldValues = new
                {
                    appointment.Status,
                    appointment.CancelledBy,
                    appointment.CancelledDate,
                    appointment.CancellationReason_En,
                    appointment.CancellationReason_Ar
                };

                appointment.Status = AppointmentStatus.Cancelled;
                appointment.CancelledBy = cancelledBy;
                appointment.CancelledDate = DateTime.UtcNow;
                appointment.CancellationReason_En = request?.CancellationReason_En ?? "Cancelled";
                appointment.CancellationReason_Ar = request?.CancellationReason_Ar ?? "ملغي";

                await _appointmentRepository.UpdateAsync(appointment);

                var newValues = new
                {
                    appointment.Status,
                    appointment.CancelledBy,
                    appointment.CancelledDate,
                    appointment.CancellationReason_En,
                    appointment.CancellationReason_Ar
                };

                await _auditLogger.LogAsync(
                    "Cancel",
                    nameof(Appointment),
                    appointment.Id.ToString(),
                    oldValues: oldValues,
                    newValues: newValues);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Appointment cancelled successfully",
                    "تم إلغاء الموعد بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to cancel appointment",
                    "فشل في إلغاء الموعد",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<AppointmentResponseDTO>> GetAppointmentByIdAsync(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentWithDetailsAsync(id);
                if (appointment == null)
                {
                    return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                        "Appointment not found",
                        "الموعد غير موجود"
                    );
                }

                var response = _mapper.Map<AppointmentResponseDTO>(appointment);
                return ApiResponse<AppointmentResponseDTO>.SuccessResponse(
                    response,
                    "Appointment retrieved successfully",
                    "تم استرجاع الموعد بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<AppointmentResponseDTO>.ErrorResponse(
                    "Failed to retrieve appointment",
                    "فشل في استرجاع الموعد",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetAppointmentsByDoctorAsync(string doctorId, DateTime? date = null)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId, date);
                var response = _mapper.Map<IEnumerable<AppointmentResponseDTO>>(appointments);

                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.SuccessResponse(
                    response,
                    "Appointments retrieved successfully",
                    "تم استرجاع المواعيد بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.ErrorResponse(
                    "Failed to retrieve appointments",
                    "فشل في استرجاع المواعيد",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetAppointmentsByPatientAsync(string patientUserId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsByPatientAsync(patientUserId);
                var response = _mapper.Map<IEnumerable<AppointmentResponseDTO>>(appointments);

                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.SuccessResponse(
                    response,
                    "Appointments retrieved successfully",
                    "تم استرجاع المواعيد بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.ErrorResponse(
                    "Failed to retrieve appointments",
                    "فشل في استرجاع المواعيد",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetTodayAppointmentsAsync()
        {
            try
            {
                var appointments = await _appointmentRepository.GetTodayAppointmentsAsync();
                var response = _mapper.Map<IEnumerable<AppointmentResponseDTO>>(appointments);

                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.SuccessResponse(
                    response,
                    "Today's appointments retrieved successfully",
                    "تم استرجاع مواعيد اليوم بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.ErrorResponse(
                    "Failed to retrieve today's appointments",
                    "فشل في استرجاع مواعيد اليوم",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetTodayAppointmentsByDoctorAsync(string doctorId)
        {
            try
            {
                var today = DateTime.Today;
                var appointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId, today);
                var response = _mapper.Map<IEnumerable<AppointmentResponseDTO>>(appointments);

                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.SuccessResponse(
                    response,
                    "Today's appointments retrieved successfully",
                    "تم استرجاع مواعيد اليوم بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.ErrorResponse(
                    "Failed to retrieve today's appointments",
                    "فشل في استرجاع مواعيد اليوم",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetUpcomingAppointmentsByDoctorAsync(string doctorId)
        {
            try
            {
                var allAppointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId);
                var upcomingAppointments = allAppointments
                    .Where(a => a.AppointmentDateTime >= DateTime.UtcNow && a.Status != AppointmentStatus.Cancelled && a.Status != AppointmentStatus.Completed)
                    .OrderBy(a => a.AppointmentDateTime);
                
                var response = _mapper.Map<IEnumerable<AppointmentResponseDTO>>(upcomingAppointments);

                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.SuccessResponse(
                    response,
                    "Upcoming appointments retrieved successfully",
                    "تم استرجاع المواعيد القادمة بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.ErrorResponse(
                    "Failed to retrieve upcoming appointments",
                    "فشل في استرجاع المواعيد القادمة",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetAppointmentsByDateRangeAsync(string doctorId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var allAppointments = await _appointmentRepository.GetAppointmentsByDoctorAsync(doctorId);
                var filteredAppointments = allAppointments
                    .Where(a => a.AppointmentDateTime.Date >= startDate.Date && a.AppointmentDateTime.Date <= endDate.Date)
                    .OrderBy(a => a.AppointmentDateTime);
                
                var response = _mapper.Map<IEnumerable<AppointmentResponseDTO>>(filteredAppointments);

                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.SuccessResponse(
                    response,
                    "Appointments retrieved successfully",
                    "تم استرجاع المواعيد بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AppointmentResponseDTO>>.ErrorResponse(
                    "Failed to retrieve appointments",
                    "فشل في استرجاع المواعيد",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}


