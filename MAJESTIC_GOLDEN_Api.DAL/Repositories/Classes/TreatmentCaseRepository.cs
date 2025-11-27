using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class TreatmentCaseRepository : GenericRepository<TreatmentCase>, ITreatmentCaseRepository
    {
        public TreatmentCaseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TreatmentCase?> GetByIdWithDetailsAsync(int id)
        {
            return await context.TreatmentCases
                .Include(tc => tc.Patient)
                .Include(tc => tc.Branch)
                .Include(tc => tc.Invoice)
                .Include(tc => tc.Treatments)
                    .ThenInclude(t => t.Service)
                .Include(tc => tc.Treatments)
                    .ThenInclude(t => t.Doctor)
                .Include(tc => tc.CaseDoctors)
                    .ThenInclude(cd => cd.Doctor)
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task<IEnumerable<TreatmentCase>> GetByPatientIdAsync(string patientUserId)
        {
            return await context.TreatmentCases
                .Include(tc => tc.Branch)
                .Include(tc => tc.Invoice)
                .Include(tc => tc.CaseDoctors)
                    .ThenInclude(cd => cd.Doctor)
                .Where(tc => tc.PatientUserId == patientUserId)
                .OrderByDescending(tc => tc.CaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TreatmentCase>> GetByDoctorIdAsync(string doctorId)
        {
            return await context.TreatmentCases
                .Include(tc => tc.Patient)
                .Include(tc => tc.Branch)
                .Include(tc => tc.Invoice)
                .Include(tc => tc.CaseDoctors)
                    .ThenInclude(cd => cd.Doctor)
                .Where(tc => tc.CaseDoctors.Any(cd => cd.DoctorId == doctorId))
                .OrderByDescending(tc => tc.CaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TreatmentCase>> GetByStatusAsync(string status)
        {
            var statusEnum = EnumExtensions.ParseEnum<TreatmentCaseStatus>(status);
            
            return await context.TreatmentCases
                .Include(tc => tc.Patient)
                .Include(tc => tc.Branch)
                .Where(tc => tc.Status == statusEnum)
                .OrderByDescending(tc => tc.CaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TreatmentCase>> GetByBranchIdAsync(int branchId)
        {
            return await context.TreatmentCases
                .Include(tc => tc.Patient)
                .Include(tc => tc.Branch)
                .Where(tc => tc.BranchId == branchId)
                .OrderByDescending(tc => tc.CaseDate)
                .ToListAsync();
        }

        public async Task<string> GenerateCaseNumberAsync()
        {
            var today = DateTime.Today;
            var prefix = $"TC{today:yyyyMMdd}";
            
            var lastCase = await context.TreatmentCases
                .Where(tc => tc.CaseNumber.StartsWith(prefix))
                .OrderByDescending(tc => tc.CaseNumber)
                .FirstOrDefaultAsync();

            if (lastCase == null)
            {
                return $"{prefix}-001";
            }

            var lastNumber = int.Parse(lastCase.CaseNumber.Split('-')[1]);
            return $"{prefix}-{(lastNumber + 1):D3}";
        }

        public async Task<IEnumerable<TreatmentCase>> GetUpcomingVisitsAsync(DateTime fromDate, DateTime toDate)
        {
            return await context.TreatmentCases
                .Include(tc => tc.Patient)
                .Include(tc => tc.Branch)
                .Include(tc => tc.CaseDoctors)
                    .ThenInclude(cd => cd.Doctor)
                .Where(tc => tc.NextVisitDate.HasValue 
                    && tc.NextVisitDate.Value.Date >= fromDate.Date 
                    && tc.NextVisitDate.Value.Date <= toDate.Date)
                .OrderBy(tc => tc.NextVisitDate)
                .ToListAsync();
        }
    }
}

