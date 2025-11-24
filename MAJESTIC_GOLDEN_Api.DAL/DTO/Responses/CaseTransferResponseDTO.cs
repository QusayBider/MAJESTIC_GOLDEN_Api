namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class CaseTransferResponseDTO
    {
        public int Id { get; set; }
        public string PatientId { get; set; } = string.Empty;
        public string PatientName_En { get; set; } = string.Empty;
        public string PatientName_Ar { get; set; } = string.Empty;
        public string FromDoctorId { get; set; } = string.Empty;
        public string FromDoctorName { get; set; } = string.Empty;
        public string ToDoctorId { get; set; } = string.Empty;
        public string ToDoctorName { get; set; } = string.Empty;
        public DateTime TransferDate { get; set; }
        public string Reason_En { get; set; } = string.Empty;
        public string Reason_Ar { get; set; } = string.Empty;
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}



