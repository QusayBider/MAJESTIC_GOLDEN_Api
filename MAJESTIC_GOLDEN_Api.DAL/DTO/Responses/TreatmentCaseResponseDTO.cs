using System;
using System.Collections.Generic;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
  
    public class TreatmentCaseResponseDTO
    {
        public int Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        
        public string PatientId { get; set; } = string.Empty;
        public string PatientName_En { get; set; } = string.Empty;
        public string PatientName_Ar { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        
        public int BranchId { get; set; }
        public string BranchName_En { get; set; } = string.Empty;
        public string BranchName_Ar { get; set; } = string.Empty;
        
        public string Title_En { get; set; } = string.Empty;
        public string Title_Ar { get; set; } = string.Empty;
        public string Description_En { get; set; } = string.Empty;
        public string Description_Ar { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        
        public DateTime CaseDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? NextVisitDate { get; set; }
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public int? InvoiceId { get; set; }
        public string? InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        
        public List<CaseTreatmentResponseDTO> Treatments { get; set; } = new();
        public List<CaseDoctorResponseDTO> Doctors { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
  
    public class CaseTreatmentResponseDTO
    {
        public int Id { get; set; }
        public int CaseId { get; set; }
        
        public int ServiceId { get; set; }
        public string ServiceName_En { get; set; } = string.Empty;
        public string ServiceName_Ar { get; set; } = string.Empty;
        
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        
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
    
   
    public class CaseDoctorResponseDTO
    {
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Role_En { get; set; } = string.Empty;
        public string Role_Ar { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public bool IsPrimary { get; set; }
    }
    

    public class TreatmentCaseDetailedResponseDTO : TreatmentCaseResponseDTO
    {
        public PatientResponseDTO? PatientDetails { get; set; }
        public InvoiceResponseDTO? InvoiceDetails { get; set; }
    }
}


