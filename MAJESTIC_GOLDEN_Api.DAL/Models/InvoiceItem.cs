using System;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class InvoiceItem : BaseModel
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
        
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal Total { get; set; }
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
}



