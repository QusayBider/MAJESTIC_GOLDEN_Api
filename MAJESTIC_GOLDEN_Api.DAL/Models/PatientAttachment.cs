using System;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class PatientAttachment : BaseModel
    {
        public int Id { get; set; }
        public string PatientUserId { get; set; } = string.Empty;
        public Patient Patient { get; set; } = null!;
        
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty; 
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public string? UploadedBy { get; set; } 
    }
}



