namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class DashboardResponseDTO
    {
        public DashboardStatistics Statistics { get; set; } = new();
        public List<BranchStatistics> BranchStats { get; set; } = new();
        public List<RecentActivity> RecentActivities { get; set; } = new();
        public FinancialSummary FinancialSummary { get; set; } = new();
    }

    public class DashboardStatistics
    {
        public int TotalPatients { get; set; }
        public int TotalBranches { get; set; }
        public int TotalDoctors { get; set; }
        public int TodayAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalDebts { get; set; }
        public int ActiveLabRequests { get; set; }
    }

    public class BranchStatistics
    {
        public int BranchId { get; set; }
        public string BranchName_En { get; set; } = string.Empty;
        public string BranchName_Ar { get; set; } = string.Empty;
        public int TotalPatients { get; set; }
        public int TodayAppointments { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }

    public class RecentActivity
    {
        public string Type { get; set; } = string.Empty; // Appointment, Invoice, Patient, etc.
        public string Description_En { get; set; } = string.Empty;
        public string Description_Ar { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class FinancialSummary
    {
        public decimal DailyRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal TotalPending { get; set; }
        public decimal TotalOverdue { get; set; }
        public List<MonthlyRevenueData> MonthlyChart { get; set; } = new();
    }

    public class MonthlyRevenueData
    {
        public string Month { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int InvoiceCount { get; set; }
    }
}



