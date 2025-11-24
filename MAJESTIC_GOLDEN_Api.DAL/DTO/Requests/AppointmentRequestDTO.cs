using MAJESTIC_GOLDEN_Api.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class AppointmentRequestDTO
    {
        [Required]
        public string PatientUserId { get; set; } = string.Empty;
        
        [Required]
        public string DoctorId { get; set; } = string.Empty;
        
        [Required]
        public int BranchId { get; set; }
        
        [Required]
        public DateTime AppointmentDateTime { get; set; }
        
        public int DurationMinutes { get; set; } = 30;
        
        public string? Reason_En { get; set; }
        public string? Reason_Ar { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }

        public string Source { get; set; } = "";
    }
    public class AppointmentPatientRequestDTO {

        [JsonIgnore]
        public string PatientUserId { get; set; } = string.Empty;

        [Required]
        public string DoctorId { get; set; } = string.Empty;

        [Required]
        public int BranchId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public string? Reason_En { get; set; }
        public string? Reason_Ar { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }

        public string Source { get; set; } = "";



    }

    public class AppointmentSubDoctorRequestDTO
    {

        [Required]
        public string PatientUserId { get; set; } = string.Empty;

        [JsonIgnore]
        public string DoctorId { get; set; } = string.Empty;

        [Required]
        public int BranchId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public string? Reason_En { get; set; }
        public string? Reason_Ar { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }

        public string Source { get; set; } = "";



    }
    public class UpdateAppointmentStatusDTO
    {
        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AppointmentStatus Status { get; set; } 

        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }
    }
    public class UpdateAppointmentStatusRequestDTO {

        [JsonIgnore]
        public string PatientUserId { get; set; } = string.Empty;

        [JsonIgnore]
        public string DoctorId { get; set; } = string.Empty;

        [Required]
        public int BranchId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        public int DurationMinutes { get; set; } = 30;

        public string? Reason_En { get; set; }
        public string? Reason_Ar { get; set; }
        public string? Notes_En { get; set; }
        public string? Notes_Ar { get; set; }

        public string Source { get; set; } = "";
    }
    public class CancelAppointmentDTO
    {
        [Required]
        public string CancellationReason_En { get; set; } = string.Empty;
        
        [Required]
        public string CancellationReason_Ar { get; set; } = string.Empty;
    }
}



