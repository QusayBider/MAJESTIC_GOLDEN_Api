using System;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class PatientDebt : BaseModel
    {
        public int Id { get; set; }
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        
        public decimal AmountDue { get; set; }
        public DateTime? DueDate { get; set; }
        public PatientDebtStatus Status { get; set; } = PatientDebtStatus.Pending;
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
}


