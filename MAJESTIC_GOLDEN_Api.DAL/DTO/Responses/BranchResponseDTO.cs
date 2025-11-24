namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class BranchResponseDTO
    {
        public int Id { get; set; }
        public string Name_En { get; set; } = string.Empty;
        public string Name_Ar { get; set; } = string.Empty;
        public string Address_En { get; set; } = string.Empty;
        public string Address_Ar { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public int TotalPatients { get; set; }
        public int TotalStaff { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}



