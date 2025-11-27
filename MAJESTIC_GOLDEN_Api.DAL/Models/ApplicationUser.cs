using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName_En { get; set; } = string.Empty;
        public string FullName_Ar { get; set; } = string.Empty;
        
        public string FullName 
        { 
            get => FullName_En; 
            set => FullName_En = value; 
        }
        
        public Gender Gender { get; set; } = Gender.Male;
        public DateTime? DateOfBirth { get; set; }
        
        
        public int? Age 
        { 
            get 
            {
                if (!DateOfBirth.HasValue) return null;
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Value.Year;
                if (DateOfBirth.Value.Date > today.AddYears(-age)) age--;
                return age;
            } 
        }
        
        
        public string? Address_En { get; set; }
        public string? Address_Ar { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        
        
        public string? Occupation_En { get; set; }
        public string? Occupation_Ar { get; set; }
        
        
        public MaritalStatus? MaritalStatus { get; set; }
        
      
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpiredDate { get; set; }
        
        
        public string? Specialization { get; set; } 
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        
        public virtual Patient? PatientProfile { get; set; }
        public virtual Laboratory? LaboratoryProfile { get; set; }

        public virtual ICollection<Appointment> AppointmentsAsDoctor { get; set; } = new List<Appointment>();
        public virtual ICollection<Invoice> InvoicesAsDoctor { get; set; } = new List<Invoice>();
        public virtual ICollection<CaseTransfer> CaseTransfersFrom { get; set; } = new List<CaseTransfer>();
        public virtual ICollection<CaseTransfer> CaseTransfersTo { get; set; } = new List<CaseTransfer>();
        public virtual ICollection<LabRequest> LabRequests { get; set; } = new List<LabRequest>();
        public virtual ICollection<PatientTooth> PatientTeeth { get; set; } = new List<PatientTooth>();
    }

}
