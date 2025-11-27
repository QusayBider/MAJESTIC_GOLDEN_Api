using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
   
    public enum InvoiceStatus
    {
       
        [Description("Unpaid")]
        Unpaid = 0,
        
        
        [Description("PartiallyPaid")]
        PartiallyPaid = 1,
        
       
        [Description("Paid")]
        Paid = 2,
        
       
        [Description("Cancelled")]
        Cancelled = 3,
        
       
        [Description("Overdue")]
        Overdue = 4
    }
}






