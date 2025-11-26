using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
   
    public enum PaymentMethod
    {
        
        [Description("Cash")]
        Cash = 0,
        
        
        [Description("CreditCard")]
        CreditCard = 1,
        
        
        [Description("DebitCard")]
        DebitCard = 2,
        
        
        [Description("BankTransfer")]
        BankTransfer = 3,
        
        
        [Description("Insurance")]
        Insurance = 4,
        
        
        [Description("Check")]
        Check = 5,
        
        
        [Description("MobilePayment")]
        MobilePayment = 6
    }
}






