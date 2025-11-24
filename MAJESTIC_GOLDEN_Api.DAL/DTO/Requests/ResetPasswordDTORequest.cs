using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Requests
{
    public class ResetPasswordDTORequest
    {
        public string Password { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
    }
}
