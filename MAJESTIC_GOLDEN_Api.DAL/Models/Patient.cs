using System;
using System.Collections.Generic;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{

    public class Patient : BaseModel
    {

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
   
        public string? TreatmentPlan_En { get; set; }
        public string? TreatmentPlan_Ar { get; set; }
        
        public string? MedicalHistory_En { get; set; }
        public string? MedicalHistory_Ar { get; set; }
        
        public string? Allergies_En { get; set; }
        public string? Allergies_Ar { get; set; }
        
       
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelation { get; set; }
        
        
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        
       
        public string? BloodType { get; set; }
        
        
        public string? SpecialNotes_En { get; set; }
        public string? SpecialNotes_Ar { get; set; }

       
        public virtual ICollection<PatientTooth> PatientTeeth { get; set; } = new List<PatientTooth>();
        public virtual ICollection<PatientAttachment> Attachments { get; set; } = new List<PatientAttachment>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public virtual ICollection<PatientDebt> Debts { get; set; } = new List<PatientDebt>();
        public virtual ICollection<CaseTransfer> CaseTransfers { get; set; } = new List<CaseTransfer>();
        public virtual ICollection<LabRequest> LabRequests { get; set; } = new List<LabRequest>();
        public virtual ICollection<TreatmentCase> TreatmentCases { get; set; } = new List<TreatmentCase>();
    }
}


