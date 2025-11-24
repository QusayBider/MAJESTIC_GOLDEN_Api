using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class PatientRepository : GenericRepository<Patient>, IPatientRepository
    {
        public PatientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetPatientWithDetailsAsync(string userId)
        {
            return await context.Patients
                .Include(p => p.User)
                    .ThenInclude(u => u.Branch)
                .Include(p => p.PatientTeeth)
                .Include(p => p.Appointments)
                .Include(p => p.Invoices)
                .Include(p => p.Debts)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<Patient?> GetPatientForUpdateAsync(string userId)
        {
            // This method is used for updates - WITH tracking (no AsNoTracking)
            return await context.Patients
                .Include(p => p.User)
                    .ThenInclude(u => u.Branch)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<Patient>> GetPatientsByBranchAsync(int branchId)
        {
            return await context.Patients
                .Include(p => p.User)
                    .ThenInclude(u => u.Branch)
                .Where(p => p.User.BranchId == branchId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientByPhoneAsync(string phone)
        {
            return await context.Patients
                .Include(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.User.PhoneNumber == phone);
        }

        public async Task<Patient?> GetPatientByEmailAsync(string email)
        {
            return await context.Patients
                .Include(p => p.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.User.Email == email);
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            return await context.Patients
                .Include(p => p.User)
                    .ThenInclude(u => u.Branch)
                .Where(p => p.User.FullName_En.Contains(searchTerm) ||
                           p.User.FullName_Ar.Contains(searchTerm) ||
                           (p.User.PhoneNumber != null && p.User.PhoneNumber.Contains(searchTerm)) ||
                           (p.User.Email != null && p.User.Email.Contains(searchTerm)))
                .AsNoTracking()
                .ToListAsync();
        }
    }
}



