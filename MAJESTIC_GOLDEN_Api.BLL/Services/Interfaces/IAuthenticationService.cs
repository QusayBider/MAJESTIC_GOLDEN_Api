using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public Task<UserDTOResponse> LoginAsync(LoginDTORequest loginDTORequest, HttpRequest Request);
        public Task<UserDTOResponse> RegisterAsync(RegisterDTORequest registerDTORequest, HttpRequest Request);
        public Task<string> ConfirmEmail(string token, string userId);
        public Task<string> ForgetPassword(ForgetPasswordDTORequest request);
        public Task<string> ResetPassword(ResetPasswordDTORequest request);
        public Task<UserDTOResponse> GetUserByIdAsync(string userId);
        public Task<UserDTOResponse> UpdateUserAsync(string userId, RegisterDTORequest updateRequest);
        public  Task<ApiResponse<bool>> ChangePasswordAsync(string userId, ChangePasswordRequestDTO request);
        Task<ApiResponse<bool>> AdminResetPasswordAsync(string userId, AdminResetPasswordRequestDTO request);

    }
}
