using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Appointment?> GetAppointmentWithDetailsAsync(int id)
        {
            return await context.Appointments
                .Include(a => a.Patient).Include(a => a.Patient.User)
                .Include(a => a.Doctor)
                .Include(a => a.Branch)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAsync(string doctorId, DateTime? date = null)
        {
            var query = context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .Include(a => a.Doctor)
                .Include(a => a.Patient.User).Include(a => a.Patient)
                .Include(a => a.Branch)
                .AsNoTracking();

            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(a => a.AppointmentDateTime >= startDate && a.AppointmentDateTime < endDate);
            }

            return await query.OrderBy(a => a.AppointmentDateTime).ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientAsync(string patientUserId)
        {
            return await context.Appointments
                .Where(a => a.PatientUserId == patientUserId)
                .Include(a => a.Doctor)
                .Include(a => a.Patient.User)
                .Include(a => a.Branch)
                .AsNoTracking()
                .OrderByDescending(a => a.AppointmentDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByBranchAsync(int branchId, DateTime? date = null)
        {
            var query = context.Appointments
                .Where(a => a.BranchId == branchId)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsNoTracking();

            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(a => a.AppointmentDateTime >= startDate && a.AppointmentDateTime < endDate);
            }

            return await query.OrderBy(a => a.AppointmentDateTime).ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(string doctorId, DateTime appointmentDateTime, int durationMinutes, int? excludeAppointmentId = null)
        {
            var endTime = appointmentDateTime.AddMinutes(durationMinutes);

            var query = context.Appointments
                .Where(a => a.DoctorId == doctorId &&
                           a.Status != AppointmentStatus.Cancelled &&
                           ((a.AppointmentDateTime >= appointmentDateTime && a.AppointmentDateTime < endTime) ||
                            (a.AppointmentDateTime.AddMinutes(a.DurationMinutes) > appointmentDateTime &&
                             a.AppointmentDateTime < appointmentDateTime)));

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Appointment>> GetTodayAppointmentsAsync()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            return await context.Appointments
                .Where(a => a.AppointmentDateTime >= today && a.AppointmentDateTime < tomorrow)
                .Include(a => a.Patient)
                .Include(a => a.Patient.User)
                .Include(a => a.Doctor)
                .Include(a => a.Branch)
                .AsNoTracking()
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
        }
    }
}


