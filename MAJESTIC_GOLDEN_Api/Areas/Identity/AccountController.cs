using MAJESTIC_GOLDEN_Api.BLL.Services.Classes;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.PLL.Areas.Identity
{
    /// <summary>
    /// Account management and authentication endpoints
    /// نقاط نهاية إدارة الحساب والمصادقة
    /// </summary>
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTOResponse>> Register([FromBody] RegisterDTORequest registerDTORequest)
        {
            var result = await _authenticationService.RegisterAsync(registerDTORequest, Request);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTOResponse>> Login([FromBody] LoginDTORequest loginDTORequest)
        {
            var result = await _authenticationService.LoginAsync(loginDTORequest, Request);
            return Ok(result);
        }

        /// <summary>
        /// Confirm email address
        /// تأكيد عنوان البريد الإلكتروني
        /// </summary>
        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult<string>> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            var result = await _authenticationService.ConfirmEmail(token, userId);
            return Ok(result);
        }

        /// <summary>
        /// Request password reset
        /// طلب إعادة تعيين كلمة المرور
        /// </summary>
        [HttpPost("ForgetPassword")]
        public async Task<ActionResult<string>> ForgetPassword([FromBody] ForgetPasswordDTORequest request)
        {
            var result = await _authenticationService.ForgetPassword(request);
            return Ok(result);
        }

        /// <summary>
        /// Reset password with code
        /// إعادة تعيين كلمة المرور بالرمز
        /// </summary>
        [HttpPatch("ResetPassword")]
        public async Task<ActionResult<string>> ResetPassword([FromBody] ResetPasswordDTORequest request)
        {
            var result = await _authenticationService.ResetPassword(request);
            return Ok(result);
        }

        /// <summary>
        /// Get current user profile
        /// الحصول على ملف تعريف المستخدم الحالي
        /// </summary>
        [HttpGet("Profile")]
        [Authorize]
        public async Task<ActionResult<UserDTOResponse>> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message_En = "Unauthorized", Message_Ar = "غير مصرح" });
            }

            var result = await _authenticationService.GetUserByIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Update user profile
        /// تحديث ملف تعريف المستخدم
        /// </summary>
        [HttpPut("Profile")]
        [Authorize]
        public async Task<ActionResult<UserDTOResponse>> UpdateProfile([FromBody] RegisterDTORequest updateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message_En = "Unauthorized", Message_Ar = "غير مصرح" });
            }

            var result = await _authenticationService.UpdateUserAsync(userId, updateRequest);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _authenticationService.ChangePasswordAsync(currentUserId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [Authorize(Roles = "HeadDoctor")]
        [HttpPut("Admin_reset_password/{userId}")]
        public async Task<IActionResult> AdminResetPassword(string userId, [FromBody] AdminResetPasswordRequestDTO request)
        {
            var result = await _authenticationService.AdminResetPasswordAsync(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
