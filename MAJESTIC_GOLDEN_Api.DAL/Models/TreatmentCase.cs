using System;
using System.Collections.Generic;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    /// <summary>
    /// Treatment Case - حالة علاجية
    /// Represents a medical case for a patient that can involve multiple doctors and treatments
    /// </summary>
    public class TreatmentCase : BaseModel
    {
        public int Id { get; set; }
        
        // Patient Information
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        
        // Branch Information
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;
        
        // Case Information
        public string CaseNumber { get; set; } = string.Empty; // رقم الحالة الفريد
        public string Title_En { get; set; } = string.Empty; // عنوان الحالة
        public string Title_Ar { get; set; } = string.Empty;
        
        public string Description_En { get; set; } = string.Empty; // وصف الحالة
        public string Description_Ar { get; set; } = string.Empty;
        
        public TreatmentCaseStatus Status { get; set; } = TreatmentCaseStatus.Open;
        
        // Dates
        public DateTime CaseDate { get; set; } = DateTime.UtcNow; // تاريخ فتح الحالة
        public DateTime? CompletedDate { get; set; } // تاريخ إغلاق الحالة
        public DateTime? NextVisitDate { get; set; } // موعد الزيارة القادمة
        
        // Notes
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        // Financial
        public int? InvoiceId { get; set; } // الفاتورة المرتبطة بالحالة
        public Invoice? Invoice { get; set; }
        
        // Navigation Properties
        public virtual ICollection<CaseTreatment> Treatments { get; set; } = new List<CaseTreatment>(); // العلاجات المقدمة
        public virtual ICollection<CaseDoctor> CaseDoctors { get; set; } = new List<CaseDoctor>(); // الأطباء المشاركين
    }
    
    /// <summary>
    /// Case Treatment - علاج مقدم في الحالة
    /// Represents a specific treatment/service provided in the case
    /// </summary>
    public class CaseTreatment : BaseModel
    {
        public int Id { get; set; }
        
        public int CaseId { get; set; }
        public TreatmentCase Case { get; set; } = null!;
        
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        
        public string? DoctorId { get; set; } // الطبيب الذي قدم هذا العلاج
        public ApplicationUser? Doctor { get; set; }
        
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        public DateTime TreatmentDate { get; set; } = DateTime.UtcNow;
        public CaseTreatmentStatus Status { get; set; } = CaseTreatmentStatus.Completed;
    }
    
    /// <summary>
    /// Case Doctor - طبيب مشارك في الحالة
    /// Represents a doctor involved in treating the case
    /// </summary>
    public class CaseDoctor : BaseModel
    {
        public int CaseId { get; set; }
        public TreatmentCase Case { get; set; } = null!;
        
        public string DoctorId { get; set; } = string.Empty;
        public ApplicationUser Doctor { get; set; } = null!;
        
        public string Role_En { get; set; } = "Treating Doctor"; // الدور (طبيب معالج، استشاري، إلخ)
        public string Role_Ar { get; set; } = "طبيب معالج";
        
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public bool IsPrimary { get; set; } = false; // هل هو الطبيب الرئيسي
    }
}

