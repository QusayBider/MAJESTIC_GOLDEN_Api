using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class PatientToothRequestDTO
    {
        [Required]
        [Range(1, 32)]
        public int ToothId { get; set; }
        
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string Status_En { get; set; } = "Healthy";
        
        [Required]
        public string Status_Ar { get; set; } = "سليم";
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        public string? ImageUrl { get; set; }
        public string? TreatmentType_En { get; set; }
        public string? TreatmentType_Ar { get; set; }
        public DateTime? TreatmentDate { get; set; }
    }
}



