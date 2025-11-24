using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Patient Debt Status Enum - حالة دين المريض
    /// </summary>
    public enum PatientDebtStatus
    {
        /// <summary>
        /// Pending - في الانتظار
        /// </summary>
        [Description("Pending")]
        Pending = 0,
        
        /// <summary>
        /// Overdue - متأخر
        /// </summary>
        [Description("Overdue")]
        Overdue = 1,
        
        /// <summary>
        /// Paid - مدفوع
        /// </summary>
        [Description("Paid")]
        Paid = 2,
        
        /// <summary>
        /// Partially Paid - مدفوع جزئياً
        /// </summary>
        [Description("PartiallyPaid")]
        PartiallyPaid = 3,
        
        /// <summary>
        /// Written Off - تم شطبه
        /// </summary>
        [Description("WrittenOff")]
        WrittenOff = 4
    }
}






