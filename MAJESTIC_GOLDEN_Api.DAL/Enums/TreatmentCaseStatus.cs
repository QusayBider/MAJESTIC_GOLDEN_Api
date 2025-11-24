using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Treatment Case Status Enum - حالة الحالة العلاجية
    /// </summary>
    public enum TreatmentCaseStatus
    {
        /// <summary>
        /// Open - مفتوحة
        /// </summary>
        [Description("Open")]
        Open = 0,
        
        /// <summary>
        /// In Progress - قيد التنفيذ
        /// </summary>
        [Description("InProgress")]
        InProgress = 1,
        
        /// <summary>
        /// Completed - مكتملة
        /// </summary>
        [Description("Completed")]
        Completed = 2,
        
        /// <summary>
        /// On Hold - معلقة
        /// </summary>
        [Description("OnHold")]
        OnHold = 3,
        
        /// <summary>
        /// Closed - مغلقة
        /// </summary>
        [Description("Closed")]
        Closed = 4,
        
        /// <summary>
        /// Cancelled - ملغاة
        /// </summary>
        [Description("Cancelled")]
        Cancelled = 5
    }
}






