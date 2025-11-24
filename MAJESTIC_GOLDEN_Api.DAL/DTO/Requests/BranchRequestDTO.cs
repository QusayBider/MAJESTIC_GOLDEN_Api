using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class BranchRequestDTO
    {
        [Required]
        public string Name_En { get; set; } = string.Empty;
        
        [Required]
        public string Name_Ar { get; set; } = string.Empty;
        
        [Required]
        public string Address_En { get; set; } = string.Empty;
        
        [Required]
        public string Address_Ar { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class UpdateBranchRequestDTO : BranchRequestDTO
    {
        public bool IsActive { get; set; } = true;
    }
}



