using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class UpdateInvoiceRequestDTO
    {
        public List<InvoiceItemRequestDTO>? Items { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? Discount { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal? Tax { get; set; }
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
}





