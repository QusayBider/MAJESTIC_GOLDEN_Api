using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Appointment Status Enum - حالة المواعيد
    /// </summary>
    public enum AppointmentStatus
    {
        /// <summary>
        /// Pending - في الانتظار
        /// </summary>
        [Description("Pending")]
        Pending = 0,
        
        /// <summary>
        /// Confirmed - مؤكد
        /// </summary>
        [Description("Confirmed")]
        Confirmed = 1,
        
        /// <summary>
        /// Completed - مكتمل
        /// </summary>
        [Description("Completed")]
        Completed = 2,
        
        /// <summary>
        /// Cancelled - ملغي
        /// </summary>
        [Description("Cancelled")]
        Cancelled = 3,
        
        /// <summary>
        /// No Show - لم يحضر
        /// </summary>
        [Description("NoShow")]
        NoShow = 4,
        
        /// <summary>
        /// Rescheduled - تم إعادة الجدولة
        /// </summary>
        [Description("Rescheduled")]
        Rescheduled = 5
    }
}






