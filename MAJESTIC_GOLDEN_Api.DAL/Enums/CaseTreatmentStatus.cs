using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
   
    public enum CaseTreatmentStatus
    {
        
        [Description("Pending")]
        Pending = 0,
        
       
        [Description("InProgress")]
        InProgress = 1,
        
       
        [Description("Completed")]
        Completed = 2,
        
       
        [Description("Cancelled")]
        Cancelled = 3
    }
}






