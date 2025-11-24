using MAJESTIC_GOLDEN_Api.BLL.Services.Classes;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MAJESTIC_GOLDEN_Api.Areas.HeadDoctor
{
    [Area("HeadDoctor")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(Roles = "HeadDoctor")]
    public class HeadDoctorController : ControllerBase
    {
        private readonly ITreatmentCaseService _treatmentCaseService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPatientService _patientService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public HeadDoctorController(ITreatmentCaseService treatmentCaseService, IInvoiceService invoiceService, IPatientService patientService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _treatmentCaseService = treatmentCaseService;
            _invoiceService = invoiceService;
            _patientService = patientService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpDelete("Delete_a_patient/{id}")]
        [Authorize(Roles = "HeadDoctor")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            var result = await _patientService.DeletePatientAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("users/role/{roleName}")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            try
            {
                var users = await _userManager.GetUsersInRoleAsync(roleName);
                var userDtos = users.Select(user => new UserDTOResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    Street = user.Street,
                    Specialization = user.Specialization,
                    BranchId = user.BranchId,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = new List<string> { roleName }
                }).ToList();

                return Ok(ApiResponse<IEnumerable<UserDTOResponse>>.SuccessResponse(
                    userDtos,
                    "Users retrieved successfully",
                    "تم استرجاع المستخدمين بنجاح"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<UserDTOResponse>>.ErrorResponse(
                    "Failed to retrieve users",
                    "فشل في استرجاع المستخدمين",
                    new List<string> { ex.Message }
                ));
            }
        }


        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> ChangeUserRole(string id, [FromBody] UserDTOChangeRole model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    ));
                }

                // Remove all current roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Add new role
                var result = await _userManager.AddToRoleAsync(user, model.NewRole);
                if (!result.Succeeded)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "Failed to change user role",
                        "فشل في تغيير دور المستخدم",
                        result.Errors.Select(e => e.Description).ToList()
                    ));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "User role changed successfully",
                    "تم تغيير دور المستخدم بنجاح"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Failed to change user role",
                    "فشل في تغيير دور المستخدم",
                    new List<string> { ex.Message }
                ));
            }
        }

        [HttpPost("users/{userId}/roles/{roleId}")]
        public async Task<IActionResult> AddRoleToUser(string userId, string roleId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    ));
                }

                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        "Role not found",
                        "الدور غير موجود"
                    ));
                }

                if (await _userManager.IsInRoleAsync(user, role.Name!))
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "User already in role",
                        "المستخدم لديه هذا الدور بالفعل"
                    ));
                }

                var result = await _userManager.AddToRoleAsync(user, role.Name!);
                if (!result.Succeeded)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "Failed to add role to user",
                        "فشل في إضافة الدور للمستخدم",
                        result.Errors.Select(e => e.Description).ToList()
                    ));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "Role added to user successfully",
                    "تمت إضافة الدور للمستخدم بنجاح"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Failed to add role to user",
                    "فشل في إضافة الدور للمستخدم",
                    new List<string> { ex.Message }
                ));
            }
        }


        [HttpDelete("users/{userId}/roles/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromUser(string userId, string roleId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    ));
                }

                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        "Role not found",
                        "الدور غير موجود"
                    ));
                }

                if (!await _userManager.IsInRoleAsync(user, role.Name!))
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "User does not have the role",
                        "المستخدم لا يمتلك هذا الدور"
                    ));
                }

                var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
                if (!result.Succeeded)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "Failed to remove role from user",
                        "فشل في إزالة الدور من المستخدم",
                        result.Errors.Select(e => e.Description).ToList()
                    ));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "Role removed from user successfully",
                    "تمت إزالة الدور من المستخدم بنجاح"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Failed to remove role from user",
                    "فشل في إزالة الدور من المستخدم",
                    new List<string> { ex.Message }
                ));
            }
        }

       
        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> ToggleUserStatus(string id, [FromBody] bool isActive)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    ));
                }

                user.IsActive = isActive;
                var result = await _userManager.UpdateAsync(user);
                
                if (!result.Succeeded)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "Failed to update user status",
                        "فشل في تحديث حالة المستخدم",
                        result.Errors.Select(e => e.Description).ToList()
                    ));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    $"User {(isActive ? "activated" : "deactivated")} successfully",
                    $"تم {(isActive ? "تفعيل" : "إلغاء تفعيل")} المستخدم بنجاح"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Failed to update user status",
                    "فشل في تحديث حالة المستخدم",
                    new List<string> { ex.Message }
                ));
            }
        }

      
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    ));
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse(
                        "Failed to delete user",
                        "فشل في حذف المستخدم",
                        result.Errors.Select(e => e.Description).ToList()
                    ));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(
                    true,
                    "User deleted successfully",
                    "تم حذف المستخدم بنجاح"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse(
                    "Failed to delete user",
                    "فشل في حذف المستخدم",
                    new List<string> { ex.Message }
                ));
            }
        }

        
        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            try
            {
                var roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return Ok(ApiResponse<IEnumerable<string>>.SuccessResponse(
                    roles!,
                    "Roles retrieved successfully",
                    "تم استرجاع الأدوار بنجاح"
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<IEnumerable<string>>.ErrorResponse(
                    "Failed to retrieve roles",
                    "فشل في استرجاع الأدوار",
                    new List<string> { ex.Message }
                ));
            }
        }


        [HttpPost("payments")]
        public async Task<IActionResult> RecordPayment([FromBody] PaymentRequestDTO request)
        {
            var receivedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await _invoiceService.RecordPaymentAsync(request, receivedBy);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{invoiceId}/payments")]
        public async Task<IActionResult> GetPaymentsByInvoice(int invoiceId)
        {
            var result = await _invoiceService.GetPaymentsByInvoiceAsync(invoiceId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}


