using MAJESTIC_GOLDEN_Api.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<ApplicationUser>> GetAllUserAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<string> BlockUserAsync(string id, int days);
        Task<string> UnBlockUserAsync(string userId);
        Task<string> IsBlockedUserAsync(string userId);
        Task<string> ChangeUserRole(string userId, string roleName);
    }

}
