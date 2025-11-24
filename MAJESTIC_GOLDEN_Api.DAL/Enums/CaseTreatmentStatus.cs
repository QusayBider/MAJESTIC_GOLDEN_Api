using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Case Treatment Status Enum - حالة علاج الحالة
    /// </summary>
    public enum CaseTreatmentStatus
    {
        /// <summary>
        /// Pending - في الانتظار
        /// </summary>
        [Description("Pending")]
        Pending = 0,
        
        /// <summary>
        /// In Progress - قيد التنفيذ
        /// </summary>
        [Description("InProgress")]
        InProgress = 1,
        
        /// <summary>
        /// Completed - مكتمل
        /// </summary>
        [Description("Completed")]
        Completed = 2,
        
        /// <summary>
        /// Cancelled - ملغي
        /// </summary>
        [Description("Cancelled")]
        Cancelled = 3
    }
}






