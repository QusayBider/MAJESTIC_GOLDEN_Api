using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class Laboratory : BaseModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ICollection<LabRequest> Requests { get; set; } = new List<LabRequest>();
        public ApplicationUser User { get; set; } = null!;
    }
}
