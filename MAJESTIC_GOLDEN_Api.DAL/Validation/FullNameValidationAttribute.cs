using System.ComponentModel.DataAnnotations;

namespace MAJESTIC_GOLDEN_Api.DAL.Validation
{
    /// <summary>
    /// Validates that the full name contains at least 3 words (triple name or more)
    /// يتحقق من أن الاسم الكامل يحتوي على 3 كلمات على الأقل (اسم ثلاثي أو أكثر)
    /// </summary>
    public class FullNameValidationAttribute : ValidationAttribute
    {
        private readonly int _minimumWords;
        
        public FullNameValidationAttribute(int minimumWords = 3)
        {
            _minimumWords = minimumWords;
            ErrorMessage = ErrorMessage ?? $"The full name must contain at least {_minimumWords} words (triple name or more) | يجب أن يحتوي الاسم الكامل على {_minimumWords} كلمات على الأقل (اسم ثلاثي أو أكثر)";
        }
        
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Full name is required | الاسم الكامل مطلوب");
            }
            
            var fullName = value.ToString()!.Trim();
            
            // Split by spaces and filter out empty entries
            var words = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if (words.Length < _minimumWords)
            {
                return new ValidationResult(
                    $"The full name must contain at least {_minimumWords} words. You entered {words.Length} word(s). | " +
                    $"يجب أن يحتوي الاسم الكامل على {_minimumWords} كلمات على الأقل. لقد أدخلت {words.Length} كلمة/كلمات."
                );
            }
            
            // Validate that each word contains at least 2 characters
            foreach (var word in words)
            {
                if (word.Length < 2)
                {
                    return new ValidationResult(
                        "Each word in the full name must contain at least 2 characters | " +
                        "يجب أن تحتوي كل كلمة في الاسم الكامل على حرفين على الأقل"
                    );
                }
            }
            
            return ValidationResult.Success;
        }
    }
}


