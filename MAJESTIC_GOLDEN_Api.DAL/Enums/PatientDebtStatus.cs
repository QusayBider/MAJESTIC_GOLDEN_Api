using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{

    public enum PatientDebtStatus
    {
       
        [Description("Pending")]
        Pending = 0,
        
      
        [Description("Overdue")]
        Overdue = 1,
        
      
        [Description("Paid")]
        Paid = 2,
        
     
        [Description("PartiallyPaid")]
        PartiallyPaid = 3,
        
     
        [Description("WrittenOff")]
        WrittenOff = 4
    }
}






