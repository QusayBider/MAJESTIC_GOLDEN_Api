namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class AppointmentResponseDTO
    {
        public int Id { get; set; }
        public string PatientUserId { get; set; } = string.Empty;
        public string FullName_En { get; set; } = string.Empty;
        public string FullName_Ar { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string BranchName_En { get; set; } = string.Empty;
        public string BranchName_Ar { get; set; } = string.Empty;
        public DateTime AppointmentDateTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string? Reason_En { get; set; }
        public string? Reason_Ar { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}



