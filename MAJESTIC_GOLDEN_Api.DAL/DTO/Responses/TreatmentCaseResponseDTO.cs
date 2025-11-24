using System;
using System.Collections.Generic;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    /// <summary>
    /// Response DTO for Treatment Case
    /// استجابة الحالة العلاجية
    /// </summary>
    public class TreatmentCaseResponseDTO
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        
        // Patient Information
        public string PatientId { get; set; } = string.Empty;
        public string PatientName_En { get; set; } = string.Empty;
        public string PatientName_Ar { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        
        // Branch Information
        public int BranchId { get; set; }
        public string BranchName_En { get; set; } = string.Empty;
        public string BranchName_Ar { get; set; } = string.Empty;
        
        // Case Information
        public string Title_En { get; set; } = string.Empty;
        public string Title_Ar { get; set; } = string.Empty;
        public string Description_En { get; set; } = string.Empty;
        public string Description_Ar { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        
        // Dates
        public DateTime CaseDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? NextVisitDate { get; set; }
        
        // Notes
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        // Financial
        public int? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        
        // Lists
        public List<CaseTreatmentResponseDTO> Treatments { get; set; } = new();
        public List<CaseDoctorResponseDTO> Doctors { get; set; } = new();
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// Response DTO for Case Treatment
    /// استجابة علاج الحالة
    /// </summary>
    public class CaseTreatmentResponseDTO
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        
        // Service Information
        public int ServiceId { get; set; }
        public string ServiceName_En { get; set; } = string.Empty;
        public string ServiceName_Ar { get; set; } = string.Empty;
        
        // Doctor Information
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        
        // Treatment Details
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public DateTime TreatmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// Response DTO for Case Doctor
    /// استجابة طبيب الحالة
    /// </summary>
    public class CaseDoctorResponseDTO
    {
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Role_En { get; set; } = string.Empty;
        public string Role_Ar { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public bool IsPrimary { get; set; }
    }
    
    /// <summary>
    /// Detailed Response DTO for Treatment Case (includes all related data)
    /// استجابة مفصلة للحالة العلاجية
    /// </summary>
    public class TreatmentCaseDetailedResponseDTO : TreatmentCaseResponseDTO
    {
        public PatientResponseDTO? PatientDetails { get; set; }
        public InvoiceResponseDTO? InvoiceDetails { get; set; }
    }
}


