using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    
    public class TreatmentCaseRequestDTO
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public int BranchId { get; set; }
        
        [Required]
        public string Title_En { get; set; } = string.Empty;
        
        [Required]
        public string Title_Ar { get; set; } = string.Empty;
        
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public DateTime? NextVisitDate { get; set; }
        
        
        public List<CaseTreatmentRequestDTO> Treatments { get; set; } = new();
        
        
        public List<string> DoctorIds { get; set; } = new();
        
        
        public bool AutoGenerateInvoice { get; set; } = true;
    }
    
   
    public class CaseTreatmentRequestDTO
    {
        [Required]
        public int ServiceId { get; set; }
        
        public int Quantity { get; set; } = 1;
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public string? DoctorId { get; set; } 
    }
    
   
    public class UpdateCaseStatusDTO
    {
        [Required]
        public string Status { get; set; } = string.Empty; // Open, InProgress, Completed, OnHold
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public DateTime? NextVisitDate { get; set; }
    }
    

    public class UpdateCaseDoctorsDTO
    {
        [Required]
        public List<string> DoctorIds { get; set; } = new();
        
        public string? PrimaryDoctorId { get; set; } // Optional: set primary doctor
    }
}


