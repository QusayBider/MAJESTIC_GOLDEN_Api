using System;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class PatientTooth : BaseModel
    {
        // Composite Key: ToothId + PatientUserId
        public int ToothId { get; set; } // 1-32 (tooth number)
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        
        public string? DoctorId { get; set; }
        public ApplicationUser? Doctor { get; set; }
        
        public string Status_En { get; set; } = "Healthy";
        public string Status_Ar { get; set; } = "سليم";
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public string? ImageUrl { get; set; }
        public string? TreatmentType_En { get; set; }
        public string? TreatmentType_Ar { get; set; }
        public DateTime? TreatmentDate { get; set; }
    }
}


