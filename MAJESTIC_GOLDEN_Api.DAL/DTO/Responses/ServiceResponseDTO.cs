namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class ServiceResponseDTO
    {
        public int Id { get; set; }
        public string Name_En { get; set; } = string.Empty;
        public string Name_Ar { get; set; } = string.Empty;
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
        public decimal BasePrice { get; set; }
        public string? Category_En { get; set; }
        public string? Category_Ar { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}



