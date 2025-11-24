using MAJESTIC_GOLDEN_Api.DAL.Models;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManger;

        public UserRepository(UserManager<ApplicationUser> userManger)
        {
            _userManger = userManger;
        }
        public async Task<List<ApplicationUser>> GetAllUserAsync()
        {
            return await _userManger.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userManger.FindByIdAsync(id);
        }
        public async Task<string> BlockUserAsync(string userId, int days)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (user == null)
                return "User Not Found";
            user.LockoutEnd = DateTime.UtcNow.AddDays(days);
            var result = await _userManger.UpdateAsync(user);
            if (result.Succeeded)
                return "User Blocked Successfully";
            return "Failed to Block User";
        }

        public async Task<string> UnBlockUserAsync(string userId)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (user == null)
                return "User Not Found";
            user.LockoutEnd = null;
            var result = await _userManger.UpdateAsync(user);
            if (result.Succeeded)
                return "User UnBlocked Successfully";
            return "Failed to UnBlock User";
        }

        public async Task<string> IsBlockedUserAsync(string userId)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (user == null)
                return "User Not Found";
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                return "User is Blocked";
            return "User is Not Blocked";

        }

        public async Task<string> ChangeUserRole(string userId, string roleName)
        {
            var user = await _userManger.FindByIdAsync(userId);
            if (user == null)
                return "User Not Found";
            var currentRoles = await _userManger.GetRolesAsync(user);
            var removeResult = await _userManger.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return "Failed to Remove User Roles";
            var addResult = await _userManger.AddToRoleAsync(user, roleName);
            if (addResult.Succeeded)
                return "User Role Changed Successfully";
            return "Failed to Change User Role";

        }
    }

}
