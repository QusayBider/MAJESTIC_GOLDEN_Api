namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class UploadFileResponseDTO
    {
        public int Id { get; set; }
        public string PatientUserId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
        public DateTime UploadDate { get; set; }
        public string? UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

