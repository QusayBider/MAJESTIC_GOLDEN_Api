namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class PatientResponseDTO
    {
        public string Id { get; set; } = string.Empty; // UserId
        public string FullName_En { get; set; } = string.Empty;
        public string FullName_Ar { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        
        // Address - العنوان
        public string? Address_En { get; set; }
        public string? Address_Ar { get; set; }
        
        // Occupation - الوظيفة
        public string? Occupation_En { get; set; }
        public string? Occupation_Ar { get; set; }
        
        // Marital Status - الحالة الاجتماعية
        public string? MaritalStatus_En { get; set; }
        public string? MaritalStatus_Ar { get; set; }
        
        // Treatment Plan - خطة العلاج
        public string? TreatmentPlan_En { get; set; }
        public string? TreatmentPlan_Ar { get; set; }
        
        public string? MedicalHistory_En { get; set; }
        public string? MedicalHistory_Ar { get; set; }
        public string? Allergies_En { get; set; }
        public string? Allergies_Ar { get; set; }
        public int BranchId { get; set; }
        public string BranchName_En { get; set; } = string.Empty;
        public string BranchName_Ar { get; set; } = string.Empty;
        public bool HasUserAccount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PatientDetailedResponseDTO : PatientResponseDTO
    {
        public int TotalAppointments { get; set; }
        public int TotalInvoices { get; set; }
        public decimal TotalDebt { get; set; }
        public int TotalTeethRecords { get; set; }
        public DateTime? LastVisit { get; set; }
    }
}


