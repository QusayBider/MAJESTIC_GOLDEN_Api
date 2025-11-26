using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.DTO.Responses
{
    public class CheckOutDTOResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? URL { get; set; }
        public string? PaymentId { get; set; }
    }
}
