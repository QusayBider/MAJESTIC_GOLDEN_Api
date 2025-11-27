using System;
using MAJESTIC_GOLDEN_Api.DAL.Enums;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class Payment : BaseModel
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? TransactionReference { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        public string? ReceivedBy { get; set; } 
    }
}


