using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MAJESTIC_GOLDEN_Api.DAL.Validation;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class RegisterDTORequest
    {
        [Required(ErrorMessage = "Username is required | اسم المستخدم مطلوب")]
        public string UserName { get; set; }
        
        [Required(ErrorMessage = "Email is required | البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "Invalid email format | صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required | كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters | كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Phone number is required | رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "Invalid phone number format | صيغة رقم الهاتف غير صحيحة")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Full name is required | الاسم الكامل مطلوب")]
        [FullNameValidation(ErrorMessage = "Full name must contain at least 3 words (triple name or more) | يجب أن يحتوي الاسم الكامل على 3 كلمات على الأقل (اسم ثلاثي أو أكثر)")]
        public string FullName { get; set; }
        
        public string? City { get; set; }
        public string? Street { get; set; }
    }
}
