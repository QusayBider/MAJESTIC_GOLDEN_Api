using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<ApiResponse<AppointmentResponseDTO>> CreateAppointmentAsync(AppointmentRequestDTO request, string createdBy);
        Task<ApiResponse<AppointmentResponseDTO>> UpdateAppointmentAsync(int id, AppointmentRequestDTO request);
        Task<ApiResponse<bool>> UpdateAppointmentStatusAsync(int id, UpdateAppointmentStatusDTO request);
        Task<ApiResponse<bool>> CancelAppointmentAsync(int id, string cancelledBy, CancelAppointmentDTO? request = null);
        Task<ApiResponse<AppointmentResponseDTO>> GetAppointmentByIdAsync(int id);
        Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetAppointmentsByDoctorAsync(string doctorId, DateTime? date = null);
        Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetAppointmentsByPatientAsync(string patientUserId);
        Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetTodayAppointmentsAsync();
        Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetTodayAppointmentsByDoctorAsync(string doctorId);
        Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetUpcomingAppointmentsByDoctorAsync(string doctorId);
        Task<ApiResponse<IEnumerable<AppointmentResponseDTO>>> GetAppointmentsByDateRangeAsync(string doctorId, DateTime startDate, DateTime endDate);
    }
}


