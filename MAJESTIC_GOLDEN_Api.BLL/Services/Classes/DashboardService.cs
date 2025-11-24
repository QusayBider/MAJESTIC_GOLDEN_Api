using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Enums;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class DashboardService : IDashboardService
    {
        private readonly IGenericRepository<Branch> _branchRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IGenericRepository<PatientDebt> _debtRepository;
        private readonly ILabRequestRepository _labRequestRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardService(
            IGenericRepository<Branch> branchRepository,
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IInvoiceRepository invoiceRepository,
            IGenericRepository<PatientDebt> debtRepository,
            ILabRequestRepository labRequestRepository,
            UserManager<ApplicationUser> userManager)
        {
            _branchRepository = branchRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _invoiceRepository = invoiceRepository;
            _debtRepository = debtRepository;
            _labRequestRepository = labRequestRepository;
            _userManager = userManager;
        }

        public async Task<ApiResponse<DashboardResponseDTO>> GetDashboardStatisticsAsync()
        {
            try
            {
                var response = new DashboardResponseDTO
                {
                    Statistics = await GetStatisticsSummaryInternalAsync(),
                    FinancialSummary = await GetFinancialSummaryInternalAsync(null, null),
                    BranchStats = (await GetBranchStatisticsInternalAsync()).ToList()
                };

                return ApiResponse<DashboardResponseDTO>.SuccessResponse(
                    response,
                    "Dashboard statistics retrieved successfully",
                    "تم استرجاع إحصائيات لوحة التحكم بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<DashboardResponseDTO>.ErrorResponse(
                    "Failed to retrieve dashboard statistics",
                    "فشل في استرجاع إحصائيات لوحة التحكم",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<DashboardStatistics>> GetStatisticsSummaryAsync()
        {
            try
            {
                var statistics = await GetStatisticsSummaryInternalAsync();
                return ApiResponse<DashboardStatistics>.SuccessResponse(
                    statistics,
                    "Statistics retrieved successfully",
                    "تم استرجاع الإحصائيات بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<DashboardStatistics>.ErrorResponse(
                    "Failed to retrieve statistics",
                    "فشل في استرجاع الإحصائيات",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<FinancialSummary>> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var summary = await GetFinancialSummaryInternalAsync(startDate, endDate);
                return ApiResponse<FinancialSummary>.SuccessResponse(
                    summary,
                    "Financial summary retrieved successfully",
                    "تم استرجاع الملخص المالي بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<FinancialSummary>.ErrorResponse(
                    "Failed to retrieve financial summary",
                    "فشل في استرجاع الملخص المالي",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<IEnumerable<BranchStatistics>>> GetBranchStatisticsAsync()
        {
            try
            {
                var branchStats = await GetBranchStatisticsInternalAsync();
                return ApiResponse<IEnumerable<BranchStatistics>>.SuccessResponse(
                    branchStats,
                    "Branch statistics retrieved successfully",
                    "تم استرجاع إحصائيات الفروع بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<BranchStatistics>>.ErrorResponse(
                    "Failed to retrieve branch statistics",
                    "فشل في استرجاع إحصائيات الفروع",
                    new List<string> { ex.Message }
                );
            }
        }

        // Internal helper methods
        private async Task<DashboardStatistics> GetStatisticsSummaryInternalAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            var branches = await _branchRepository.GetAllAsync();
            var doctors = await _userManager.GetUsersInRoleAsync("HeadDoctor");
            var subDoctors = await _userManager.GetUsersInRoleAsync("SubDoctor");
            var todayAppointments = await _appointmentRepository.GetTodayAppointmentsAsync();
            var pendingAppointments = await _appointmentRepository.FindAsync(a => a.Status == AppointmentStatus.Pending);
            var debts = await _debtRepository.FindAsync(d => d.Status == PatientDebtStatus.Pending);
            var labRequests = await _labRequestRepository.GetPendingLabRequestsAsync();

            var totalRevenue = await _invoiceRepository.GetTotalRevenueAsync();

            return new DashboardStatistics
            {
                TotalPatients = patients.Count(),
                TotalBranches = branches.Count(),
                TotalDoctors = doctors.Count + subDoctors.Count,
                TodayAppointments = todayAppointments.Count(),
                PendingAppointments = pendingAppointments.Count(),
                TotalRevenue = totalRevenue,
                TotalDebts = debts.Sum(d => d.AmountDue),
                ActiveLabRequests = labRequests.Count()
            };
        }

        private async Task<FinancialSummary> GetFinancialSummaryInternalAsync(DateTime? startDate, DateTime? endDate)
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            var dailyRevenue = await _invoiceRepository.GetTotalRevenueAsync(today, today.AddDays(1));
            var weeklyRevenue = await _invoiceRepository.GetTotalRevenueAsync(startOfWeek, today.AddDays(1));
            var monthlyRevenue = await _invoiceRepository.GetTotalRevenueAsync(startOfMonth, today.AddDays(1));

            var pendingDebts = await _debtRepository.FindAsync(d => d.Status == PatientDebtStatus.Pending);
            var overdueDebts = await _debtRepository.FindAsync(d => 
                d.Status == PatientDebtStatus.Pending && d.DueDate.HasValue && d.DueDate < DateTime.UtcNow);

            // Monthly chart data (last 6 months)
            var monthlyChart = new List<MonthlyRevenueData>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = today.AddMonths(-i).AddDays(-today.Day + 1);
                var monthEnd = monthStart.AddMonths(1);
                var revenue = await _invoiceRepository.GetTotalRevenueAsync(monthStart, monthEnd);
                var invoices = await _invoiceRepository.FindAsync(inv => 
                    inv.InvoiceDate >= monthStart && inv.InvoiceDate < monthEnd);

                monthlyChart.Add(new MonthlyRevenueData
                {
                    Month = monthStart.ToString("MMM yyyy"),
                    Revenue = revenue,
                    InvoiceCount = invoices.Count()
                });
            }

            return new FinancialSummary
            {
                DailyRevenue = dailyRevenue,
                WeeklyRevenue = weeklyRevenue,
                MonthlyRevenue = monthlyRevenue,
                TotalPending = pendingDebts.Sum(d => d.AmountDue),
                TotalOverdue = overdueDebts.Sum(d => d.AmountDue),
                MonthlyChart = monthlyChart
            };
        }

        private async Task<IEnumerable<BranchStatistics>> GetBranchStatisticsInternalAsync()
        {
            var branches = await _branchRepository.GetAllAsync();
            var branchStats = new List<BranchStatistics>();
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);

            foreach (var branch in branches)
            {
                var patients = await _patientRepository.GetPatientsByBranchAsync(branch.Id);
                var appointments = await _appointmentRepository.GetAppointmentsByBranchAsync(branch.Id, today);
                var monthlyRevenue = await _invoiceRepository.GetTotalRevenueAsync(startOfMonth, today.AddDays(1));

                branchStats.Add(new BranchStatistics
                {
                    BranchId = branch.Id,
                    BranchName_En = branch.Name_En,
                    BranchName_Ar = branch.Name_Ar,
                    TotalPatients = patients.Count(),
                    TodayAppointments = appointments.Count(),
                    MonthlyRevenue = monthlyRevenue
                });
            }

            return branchStats;
        }
    }
}


