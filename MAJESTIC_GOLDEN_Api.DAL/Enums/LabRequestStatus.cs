using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Lab Request Status Enum - حالة طلب المختبر
    /// </summary>
    public enum LabRequestStatus
    {
        /// <summary>
        /// Pending - في الانتظار
        /// </summary>
        [Description("Pending")]
        Pending = 0,
        
        /// <summary>
        /// Sent to Lab - تم الإرسال للمختبر
        /// </summary>
        [Description("SentToLab")]
        SentToLab = 1,
        
        /// <summary>
        /// In Progress - قيد التنفيذ
        /// </summary>
        [Description("InProgress")]
        InProgress = 2,
        
        /// <summary>
        /// Ready - جاهز
        /// </summary>
        [Description("Ready")]
        Ready = 3,
        
        /// <summary>
        /// Delivered - تم التسليم
        /// </summary>
        [Description("Delivered")]
        Delivered = 4,
        
        /// <summary>
        /// Cancelled - ملغي
        /// </summary>
        [Description("Cancelled")]
        Cancelled = 5
    }
}






