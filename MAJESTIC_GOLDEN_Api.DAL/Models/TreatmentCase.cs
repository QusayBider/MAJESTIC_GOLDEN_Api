using System;
using System.Collections.Generic;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    
    public class TreatmentCase : BaseModel
    {
        public int Id { get; set; }
        
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
        
        public string CaseNumber { get; set; } = string.Empty; 
        public string Title_En { get; set; } = string.Empty; 
        public string Title_Ar { get; set; } = string.Empty;
        
        public string Description_En { get; set; } = string.Empty; 
        public string Description_Ar { get; set; } = string.Empty;
        
        public TreatmentCaseStatus Status { get; set; } = TreatmentCaseStatus.Open;
        
      
        public DateTime CaseDate { get; set; } = DateTime.UtcNow; 
        public DateTime? CompletedDate { get; set; } 
        public DateTime? NextVisitDate { get; set; } 
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public int? InvoiceId { get; set; } 
        public Invoice? Invoice { get; set; }
        
        public virtual ICollection<CaseTreatment> Treatments { get; set; } = new List<CaseTreatment>(); 
        public virtual ICollection<CaseDoctor> CaseDoctors { get; set; } = new List<CaseDoctor>(); 
    }
    
  
    public class CaseTreatment : BaseModel
    {
        public int Id { get; set; }
        
        public int CaseId { get; set; }
        public TreatmentCase Case { get; set; } = null!;
        
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        
        public string? DoctorId { get; set; } 
        public ApplicationUser? Doctor { get; set; }
        
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public DateTime TreatmentDate { get; set; } = DateTime.UtcNow;
        public CaseTreatmentStatus Status { get; set; } = CaseTreatmentStatus.Completed;
    }
    
 
    public class CaseDoctor : BaseModel
    {
        public int CaseId { get; set; }
        public TreatmentCase Case { get; set; } = null!;
        
        public string DoctorId { get; set; } = string.Empty;
        public ApplicationUser Doctor { get; set; } = null!;
        
        public string Role_En { get; set; } = "Treating Doctor"; 
        public string Role_Ar { get; set; } = "طبيب معالج";
        
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public bool IsPrimary { get; set; } = false; 
    }
}

