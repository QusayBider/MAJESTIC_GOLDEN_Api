using System;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class Appointment : BaseModel
    {
        public int Id { get; set; }
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        
        public string DoctorId { get; set; } = string.Empty;
        public ApplicationUser Doctor { get; set; } = null!;
        
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
        
        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; } = 30;
        
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string Source { get; set; } = "Internal"; // Internal, Online
        
        public string? Reason_En { get; set; }
        public string? Reason_Ar { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public string? CancelledBy { get; set; }
        public DateTime? CancelledDate { get; set; }
        public string? CancellationReason_En { get; set; }
        public string? CancellationReason_Ar { get; set; }
    }
}


