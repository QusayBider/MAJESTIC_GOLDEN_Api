using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Marital Status Enum - الحالة الاجتماعية
    /// </summary>
    public enum MaritalStatus
    {
        /// <summary>
        /// Single - أعزب
        /// </summary>
        [Description("Single")]
        Single = 0,
        
        /// <summary>
        /// Married - متزوج
        /// </summary>
        [Description("Married")]
        Married = 1,
        
        /// <summary>
        /// Divorced - مطلق
        /// </summary>
        [Description("Divorced")]
        Divorced = 2,
        
        /// <summary>
        /// Widowed - أرمل
        /// </summary>
        [Description("Widowed")]
        Widowed = 3
    }
}






