using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
   
    public class ChangePasswordRequestDTO
    {
        [Required(ErrorMessage = "Current password is required | كلمة المرور الحالية مطلوبة")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required | كلمة المرور الجديدة مطلوبة")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters | كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required | تأكيد كلمة المرور مطلوب")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match | كلمات المرور غير متطابقة")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }


    public class AdminResetPasswordRequestDTO
    {
        [Required(ErrorMessage = "New password is required | كلمة المرور الجديدة مطلوبة")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters | كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required | تأكيد كلمة المرور مطلوب")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match | كلمات المرور غير متطابقة")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}


