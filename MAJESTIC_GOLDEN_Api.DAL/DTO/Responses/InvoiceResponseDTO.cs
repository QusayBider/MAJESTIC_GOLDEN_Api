namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class InvoiceResponseDTO
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string PatientUserId { get; set; } = string.Empty;
        public string PatientName_En { get; set; } = string.Empty;
        public string PatientName_Ar { get; set; } = string.Empty;
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        public List<InvoiceItemResponseDTO> Items { get; set; } = new();
        public List<PaymentResponseDTO> Payments { get; set; } = new();
    }

    public class InvoiceItemResponseDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName_En { get; set; } = string.Empty;
        public string ServiceName_Ar { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }

    public class PaymentResponseDTO
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionReference { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        public string? ReceivedBy { get; set; }
    }
}



