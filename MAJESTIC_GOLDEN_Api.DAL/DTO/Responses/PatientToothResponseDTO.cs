namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class PatientToothResponseDTO
    {
        // Composite Key
        public int ToothId { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public string Status_En { get; set; } = string.Empty;
        public string Status_Ar { get; set; } = string.Empty;
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        public string? ImageUrl { get; set; }
        public string? TreatmentType_En { get; set; }
        public string? TreatmentType_Ar { get; set; }
        public DateTime? TreatmentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class DentalChartResponseDTO
    {
        public string PatientId { get; set; } = string.Empty;
        public string PatientName_En { get; set; } = string.Empty;
        public string PatientName_Ar { get; set; } = string.Empty;
        public List<PatientToothResponseDTO> Teeth { get; set; } = new();
    }
}


