using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class LaboratoryCreateDTO
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
    }

    public class LaboratoryUpdateDTO
    {
        public string? UserId { get; set; }
    }
}



