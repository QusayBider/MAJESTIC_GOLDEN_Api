using System;
using System.Collections.Generic;

namespace MAJESTIC_GOLDEN_Api.DAL.Models
{
    public class Branch : BaseModel
    {
        public int Id { get; set; }
        public string Name_En { get; set; } = string.Empty;
        public string Name_Ar { get; set; } = string.Empty;
        public string Address_En { get; set; } = string.Empty;
        public string Address_Ar { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}



