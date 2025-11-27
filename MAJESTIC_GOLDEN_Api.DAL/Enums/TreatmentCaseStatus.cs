using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{

    public enum TreatmentCaseStatus
    {
     
        [Description("Open")]
        Open = 0,

  
        [Description("InProgress")]
        InProgress = 1,
        
 
        [Description("Completed")]
        Completed = 2,
        

        [Description("OnHold")]
        OnHold = 3,
 
        [Description("Closed")]
        Closed = 4,

        [Description("Cancelled")]
        Cancelled = 5
    }
}






