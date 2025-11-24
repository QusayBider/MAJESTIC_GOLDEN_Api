using System;
using System.Collections.Generic;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class Invoice : BaseModel
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        
        public string? DoctorId { get; set; }
        public ApplicationUser? Doctor { get; set; }
        
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal Tax { get; set; } = 0;
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; } = 0;
        public decimal RemainingAmount { get; set; }
        
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        
        // Navigation properties
        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual PatientDebt? Debt { get; set; }
    }
}


