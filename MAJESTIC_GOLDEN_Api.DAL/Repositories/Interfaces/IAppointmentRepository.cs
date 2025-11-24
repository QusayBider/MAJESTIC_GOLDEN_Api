using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<Appointment?> GetAppointmentWithDetailsAsync(int id);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(string doctorId, DateTime? date = null);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(string patientUserId);
        Task<IEnumerable<Appointment>> GetAppointmentsByBranchAsync(int branchId, DateTime? date = null);
        Task<bool> IsTimeSlotAvailableAsync(string doctorId, DateTime appointmentDateTime, int durationMinutes, int? excludeAppointmentId = null);
        Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync();
    }
}



