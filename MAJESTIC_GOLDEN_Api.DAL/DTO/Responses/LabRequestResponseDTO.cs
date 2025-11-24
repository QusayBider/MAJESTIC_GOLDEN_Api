namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class LabRequestResponseDTO
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; } = string.Empty;
        public int LaboratoryId { get; set; }
        public string? LabName { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string PatientName_En { get; set; } = string.Empty;
        public string PatientName_Ar { get; set; } = string.Empty;
        public string DoctorId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Type_En { get; set; } = string.Empty;
        public string Type_Ar { get; set; } = string.Empty;
        public string? Description_En { get; set; }
        public string? Description_Ar { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal? Cost { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
}



