using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class LabRequestCreateDTO
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;

        [Required]
        public int LaboratoryId { get; set; }

        [Required]
        public string Type_En { get; set; } = string.Empty;
        
        [Required]
        public string Type_Ar { get; set; } = string.Empty;
        
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
        
        public DateTime? ExpectedDate { get; set; }
        
        public decimal? Cost { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }

    public class LabRequestUpdateDTO
    {
        public string? Status { get; set; } 
        public DateTime? ExpectedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal? Cost { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
}



