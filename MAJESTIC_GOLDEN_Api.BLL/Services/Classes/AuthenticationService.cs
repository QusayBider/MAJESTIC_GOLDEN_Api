using Azure.Core;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IAuthenticationService = MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces.IAuthenticationService;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Classes
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuditLogger _auditLogger;

        public AuthenticationService(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager, IAuditLogger auditLogger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailSender = emailSender;
            _signInManager = signInManager;
            _auditLogger = auditLogger;
        }

        public async Task<UserDTOResponse> LoginAsync(LoginDTORequest loginDTORequest, HttpRequest Request)
        {
            var user = await _userManager.FindByEmailAsync(loginDTORequest.Email);
            if (user is null)
            {
                throw new Exception("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTORequest.Password, true);
            if (result.Succeeded&& !result.IsNotAllowed)
            {
                var roles = await _userManager.GetRolesAsync(user);
                
                return new UserDTOResponse
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    Street = user.Street,
                    Specialization = user.Specialization,
                    BranchId = user.BranchId,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = roles.ToList(),
                    Token = await GeneretateJWT(user)
                };

            }
            else if (result.IsLockedOut)
            {
                throw new Exception("Your account is locked. Please try again later.");
            }
            else if (result.IsNotAllowed)
            {
                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                if (!isEmailConfirmed)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var escapeToken = Uri.EscapeDataString(token);
                    var emailUrl = $"{Request.Scheme}://{Request.Host}/api/Identity/Account/confirmEmail?token={escapeToken}&userId={user.Id}";
                    await _emailSender.SendEmailAsync(user.Email, "Email Confirmation - تأكيد البريد الإلكتروني", 
                        $"<h1>Hello {user.UserName} - مرحباً</h1>" +
                        $"<p>Please confirm your email to complete your login.</p>" +
                        $"<p>الرجاء تأكيد بريدك الإلكتروني لإكمال تسجيل الدخول.</p>" +
                        $"<a href='{emailUrl}' style='display:inline-block;padding:10px 20px;background-color:#007bff;color:white;text-decoration:none;border-radius:5px;'>Confirm Email - تأكيد البريد</a>");
                    
                    throw new Exception("Your email is not confirmed. A new confirmation email has been sent to your email address. Please check your inbox and confirm your email to login. | بريدك الإلكتروني غير مؤكد. تم إرسال رسالة تأكيد جديدة إلى بريدك الإلكتروني. الرجاء التحقق من صندوق الوارد وتأكيد بريدك الإلكتروني لتسجيل الدخول.");
                }
                throw new Exception("You need to confirm your email before logging in.");
            }
            else
            {
                throw new Exception("Invalid email or password");
            }
        }

        private async Task<string> GeneretateJWT(ApplicationUser user)
        {

            var Claims = new List<Claim>() {
                new Claim("Name",user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };

            var Roles = await _userManager.GetRolesAsync(user);
            foreach (var role in Roles)
            {
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConnection")["securityKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: Claims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<UserDTOResponse> RegisterAsync(RegisterDTORequest registerDTORequest, HttpRequest Request)
        {
            var user = new ApplicationUser
            {

                UserName = registerDTORequest.UserName,
                FullName = registerDTORequest.FullName,
                Email = registerDTORequest.Email,
                PhoneNumber = registerDTORequest.PhoneNumber

            };
            var Result = await _userManager.CreateAsync(user, registerDTORequest.Password);
            if (Result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Patient");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var escapeToken = Uri.EscapeDataString(token);
                var emailUrl = $"{Request.Scheme}://{Request.Host}/api/Identity/Account/confirmEmail?token={escapeToken}&userId={user.Id}";
                await _emailSender.SendEmailAsync(user.Email, "Welcome", $"<h1>Hello {user.UserName}</h1>" + $"<a href='{emailUrl}'>confirm</a>");
                return new UserDTOResponse()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    City = user.City,
                    Street = user.Street,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    Roles = new List<string> { "Patient" },
                    Token =""
                };
            }
            else
            {
                throw new Exception($"{Result.Errors}");
            }
        }

        public async Task<string> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                throw new Exception("user not found");
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return "Email confirmed succesfully";
            }
            return "Email confirmation failed";
        }

        public async Task<string> ForgetPassword(ForgetPasswordDTORequest request)
        {

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                throw new Exception("User not found");
            }

            var random = new Random();
            var code = random.Next(10000, 99999).ToString();

            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiredDate = DateTime.UtcNow.AddMinutes(10);

            await _userManager.UpdateAsync(user);
            await _emailSender.SendEmailAsync(request.Email, "Reset your password", $" <p style=\"margin:0 0 12px 0;font-size:14px;line-height:1.6;\"> Hi {user.UserName}, use the code below to reset your Account password:</p>" + $" <div class=\"code\" style=\"letter-spacing:4px;text-align:center;font-family:Consolas,Menlo,Monaco,monospace;font-weight:700;font-size:28px;padding:14px 20px;border:1px dashed #d1d5db;border-radius:10px;background:#f8fafc;color:#111827;margin:10px 0 14px 0;\">{code}</div>" + $"<p class=\"muted\" style=\"margin:0 0 8px 0;font-size:12px;line-height:1.6;color:#6b7280;\">This code expires in <strong>10 minutes</strong> and can be used once. Do not share it with anyone.</p>" + $"<p class=\"muted\" style=\"margin:0 0 16px 0;font-size:12px;line-height:1.6;color:#6b7280;\">If you didn’t request a password reset, you can safely ignore this email.</p>");
            return ("Check your email — we’ve sent you a message with instructions to reset your password.");
        }
        public async Task<string> ResetPassword(ResetPasswordDTORequest request)
        {

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return ("User not found");
            }
            if (user.PasswordResetCode != request.Code)
            {
                return ("The code is not correct ");
            }
            if (user.PasswordResetCodeExpiredDate < DateTime.UtcNow)
            {
                return ("The code is not correct ");
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (passwordValid)
            {
                return ("Your new password cannot be the same as your previous password. Please choose a different one.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);

            if (result.Succeeded)
            {
                await _emailSender.SendEmailAsync(user.Email, "Password Changed", $"<p>Hello {user.UserName},</p>" + $"<p>This is a confirmation that the password for your <b>QusayShop</b> account was successfully changed.</p>" + $"<p>If you made this change, no further action is required.<br>If you did not request this change, please reset your password immediately and contact our support team.</p>" + "<p style=\"color:#6b7280;font-size:12px;margin-top:20px;\">Thanks,<br>The {QusayShop} Team</p>");

                user.PasswordResetCode = "";
                await _userManager.UpdateAsync(user);

                await _auditLogger.LogAsync(
                    "ResetPassword",
                    nameof(ApplicationUser),
                    user.Id,
                    userId: user.Id,
                    newValues: new { PasswordResetViaCode = true });

                return ("Password Changed Successfully");
            }
            else
            {
                return ("Password Changed Falid");
            }

        }

        public async Task<UserDTOResponse> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTOResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                Street = user.Street,
                Specialization = user.Specialization,
                BranchId = user.BranchId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            };
        }

        public async Task<UserDTOResponse> UpdateUserAsync(string userId, RegisterDTORequest updateRequest)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var oldValues = new
            {
                user.FullName,
                user.PhoneNumber,
                user.City,
                user.Street
            };

            user.FullName = updateRequest.FullName;
            user.PhoneNumber = updateRequest.PhoneNumber;
            user.City = updateRequest.City;
            user.Street = updateRequest.Street;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to update user");
            }

            var newValues = new
            {
                user.FullName,
                user.PhoneNumber,
                user.City,
                user.Street
            };

            await _auditLogger.LogAsync(
                "UpdateProfile",
                nameof(ApplicationUser),
                user.Id,
                userId: user.Id,
                userName: user.FullName,
                oldValues: oldValues,
                newValues: newValues);

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTOResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                Street = user.Street,
                Specialization = user.Specialization,
                BranchId = user.BranchId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            };
        }
        
        public async Task<ApiResponse<bool>> AdminResetPasswordAsync(string userId, AdminResetPasswordRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null )
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    );
                }

                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    var errors = removePasswordResult.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResponse(
                        "Failed to reset password",
                        "فشل في إعادة تعيين كلمة المرور",
                        errors
                    );
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
                if (!addPasswordResult.Succeeded)
                {
                    var errors = addPasswordResult.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResponse(
                        "Failed to set new password",
                        "فشل في تعيين كلمة المرور الجديدة",
                        errors
                    );
                }

                await _auditLogger.LogAsync(
                    "AdminResetPassword",
                    nameof(ApplicationUser),
                    user.Id,
                    newValues: new { ResetByAdmin = true });

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Password reset successfully",
                    "تم إعادة تعيين كلمة المرور بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to reset password",
                    "فشل في إعادة تعيين كلمة المرور",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordRequestDTO request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "User not found",
                        "المستخدم غير موجود"
                    );
                }

                var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    return ApiResponse<bool>.ErrorResponse(
                        "Current password is incorrect",
                        "كلمة المرور الحالية غير صحيحة"
                    );
                }

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResponse(
                        "Failed to change password",
                        "فشل في تغيير كلمة المرور",
                        errors
                    );
                }

                await _auditLogger.LogAsync(
                    "ChangePassword",
                    nameof(ApplicationUser),
                    user.Id,
                    userId: userId,
                    newValues: new { PasswordChanged = true });

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Password changed successfully",
                    "تم تغيير كلمة المرور بنجاح"
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    "Failed to change password",
                    "فشل في تغيير كلمة المرور",
                    new List<string> { ex.Message }
                );
            }
        }


    }
}
