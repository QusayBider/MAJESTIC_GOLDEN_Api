using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
  
    public enum LabRequestStatus
    {
        
        [Description("Pending")]
        Pending = 0,
        
       
        [Description("SentToLab")]
        SentToLab = 1,
        
       
        [Description("InProgress")]
        InProgress = 2,
        
       
        [Description("Ready")]
        Ready = 3,
        
      
        [Description("Delivered")]
        Delivered = 4,
        
       
        [Description("Cancelled")]
        Cancelled = 5
    }
}






