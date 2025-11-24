using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Payment Method Enum - طريقة الدفع
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Cash - نقداً
        /// </summary>
        [Description("Cash")]
        Cash = 0,
        
        /// <summary>
        /// Credit Card - بطاقة ائتمان
        /// </summary>
        [Description("CreditCard")]
        CreditCard = 1,
        
        /// <summary>
        /// Debit Card - بطاقة خصم
        /// </summary>
        [Description("DebitCard")]
        DebitCard = 2,
        
        /// <summary>
        /// Bank Transfer - تحويل بنكي
        /// </summary>
        [Description("BankTransfer")]
        BankTransfer = 3,
        
        /// <summary>
        /// Insurance - تأمين
        /// </summary>
        [Description("Insurance")]
        Insurance = 4,
        
        /// <summary>
        /// Check - شيك
        /// </summary>
        [Description("Check")]
        Check = 5,
        
        /// <summary>
        /// Mobile Payment - دفع إلكتروني
        /// </summary>
        [Description("MobilePayment")]
        MobilePayment = 6
    }
}






