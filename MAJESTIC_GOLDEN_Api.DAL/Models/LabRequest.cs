using System;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class LabRequest : BaseModel
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        public int LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; } = null!;
        public string DoctorId { get; set; } = string.Empty;
        public ApplicationUser Doctor { get; set; } = null!;
        
        public string Type_En { get; set; } = string.Empty; 
        public string Type_Ar { get; set; } = string.Empty;
        
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
        
        public LabRequestStatus Status { get; set; } = LabRequestStatus.Pending;
        
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpectedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal? Cost { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
}


