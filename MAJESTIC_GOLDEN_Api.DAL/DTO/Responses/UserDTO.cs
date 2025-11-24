using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }

        public DateTime Create_at { get; set; } = DateTime.Now;

        public bool emailConfirmed { get; set; }
        public string UserRole { get; set; }
    }
}
