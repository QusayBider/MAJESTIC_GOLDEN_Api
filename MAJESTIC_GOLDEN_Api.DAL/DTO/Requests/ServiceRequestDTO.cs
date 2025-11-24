using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class ServiceRequestDTO
    {
        [Required]
        public string Name_En { get; set; } = string.Empty;
        
        [Required]
        public string Name_Ar { get; set; } = string.Empty;
        
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal BasePrice { get; set; }
        
        public string? Category_En { get; set; }
        public string? Category_Ar { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}



