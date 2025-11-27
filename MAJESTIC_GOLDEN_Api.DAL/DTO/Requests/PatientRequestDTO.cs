using System.ComponentModel.DataAnnotations;
using MAJESTIC_GOLDEN_Api.DAL.Validation;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class PatientRequestDTO
    {
        [Required(ErrorMessage = "Full name in English is required | الاسم الكامل بالإنجليزية مطلوب")]
        [FullNameValidation(ErrorMessage = "Full name must contain at least 3 words (triple name or more) | يجب أن يحتوي الاسم الكامل على 3 كلمات على الأقل (اسم ثلاثي أو أكثر)")]
        public string FullName_En { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Full name in Arabic is required | الاسم الكامل بالعربية مطلوب")]
        [FullNameValidation(ErrorMessage = "Full name must contain at least 3 words (triple name or more) | يجب أن يحتوي الاسم الكامل على 3 كلمات على الأقل (اسم ثلاثي أو أكثر)")]
        public string FullName_Ar { get; set; } = string.Empty;
        
        [Required]
        public string Gender { get; set; } = string.Empty; 
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        [EmailAddress]
        public string? Email { get; set; }
        
        public string? Address_En { get; set; }
        public string? Address_Ar { get; set; }
        
        public string? Occupation_En { get; set; }
        public string? Occupation_Ar { get; set; }
        
        public string? MaritalStatus_En { get; set; }
        public string? MaritalStatus_Ar { get; set; }
        
        public string? TreatmentPlan_En { get; set; }
        public string? TreatmentPlan_Ar { get; set; }
        
        public string? MedicalHistory_En { get; set; }
        public string? MedicalHistory_Ar { get; set; }
        public string? Allergies_En { get; set; }
        public string? Allergies_Ar { get; set; }
        
        [Required]
        public int BranchId { get; set; }
    }

    public class PatientPortalRegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Full name in English is required | الاسم الكامل بالإنجليزية مطلوب")]
        [FullNameValidation(ErrorMessage = "Full name must contain at least 3 words (triple name or more) | يجب أن يحتوي الاسم الكامل على 3 كلمات على الأقل (اسم ثلاثي أو أكثر)")]
        public string FullName_En { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Full name in Arabic is required | الاسم الكامل بالعربية مطلوب")]
        [FullNameValidation(ErrorMessage = "Full name must contain at least 3 words (triple name or more) | يجب أن يحتوي الاسم الكامل على 3 كلمات على الأقل (اسم ثلاثي أو أكثر)")]
        public string FullName_Ar { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string Gender { get; set; } = string.Empty;
        
        [Required]
        public int BranchId { get; set; }
    }
}


