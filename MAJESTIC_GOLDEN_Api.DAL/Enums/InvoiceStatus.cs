using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Invoice Status Enum - حالة الفاتورة
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// Unpaid - غير مدفوعة
        /// </summary>
        [Description("Unpaid")]
        Unpaid = 0,
        
        /// <summary>
        /// Partially Paid - مدفوعة جزئياً
        /// </summary>
        [Description("PartiallyPaid")]
        PartiallyPaid = 1,
        
        /// <summary>
        /// Paid - مدفوعة بالكامل
        /// </summary>
        [Description("Paid")]
        Paid = 2,
        
        /// <summary>
        /// Cancelled - ملغاة
        /// </summary>
        [Description("Cancelled")]
        Cancelled = 3,
        
        /// <summary>
        /// Overdue - متأخرة
        /// </summary>
        [Description("Overdue")]
        Overdue = 4
    }
}






