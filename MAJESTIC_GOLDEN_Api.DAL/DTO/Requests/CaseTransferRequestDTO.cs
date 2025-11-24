using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class CaseTransferRequestDTO
    {
        [Required]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        public string ToDoctorId { get; set; } = string.Empty;
        
        [Required]
        public string Reason_En { get; set; } = string.Empty;
        
        [Required]
        public string Reason_Ar { get; set; } = string.Empty;
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }

    public class CaseTransferStatusDTO
    {
        [Required]
        public string Status { get; set; } = string.Empty; // Pending, Accepted, Rejected
        
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
}



