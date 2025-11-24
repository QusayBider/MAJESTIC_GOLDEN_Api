using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class InvoiceRequestDTO
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;

        [Required]
        public string DoctorId { get; set; }
        
        [Required]
        public List<InvoiceItemRequestDTO> Items { get; set; } = new();
        
        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; } = 0;
        
        [Range(0, double.MaxValue)]
        public decimal Tax { get; set; } = 0;
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }

    public class InvoiceSubDoctorRequestDTO
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;
        [JsonIgnore]
        public string? DoctorId { get; set; }

        [Required]
        public List<InvoiceItemRequestDTO> Items { get; set; } = new();

        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal Tax { get; set; } = 0;

        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
    public class InvoiceItemRequestDTO
    {
        [Required]
        public int ServiceId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
        
        // Optional: If not provided or 0, service BasePrice will be used
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; } = 0;
        
        [Range(0, double.MaxValue)]
        public decimal Discount { get; set; } = 0;
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }

    public class PaymentRequestDTO
    {
        [Required]
        public int InvoiceId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, Bank Transfer
        
        public string? TransactionReference { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
}



