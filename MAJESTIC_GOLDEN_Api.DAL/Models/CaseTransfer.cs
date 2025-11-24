using System;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class CaseTransfer : BaseModel
    {
        public int Id { get; set; }
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        
        public string FromDoctorId { get; set; } = string.Empty;
        public ApplicationUser FromDoctor { get; set; } = null!;
        
        public string ToDoctorId { get; set; } = string.Empty;
        public ApplicationUser ToDoctor { get; set; } = null!;
        
        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
        public string Reason_En { get; set; } = string.Empty;
        public string Reason_Ar { get; set; } = string.Empty;
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected
    }
}



