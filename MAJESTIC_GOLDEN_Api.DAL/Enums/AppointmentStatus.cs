using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
   
    public enum AppointmentStatus
    {
       
        [Description("Pending")]
        Pending = 0,
        
        
        [Description("Confirmed")]
        Confirmed = 1,
        
       
        [Description("Completed")]
        Completed = 2,
        
        
        [Description("Cancelled")]
        Cancelled = 3,
        
        
        [Description("NoShow")]
        NoShow = 4,
        
      
        [Description("Rescheduled")]
        Rescheduled = 5
    }
}






