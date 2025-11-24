using System.ComponentModel;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Gender Enum - الجنس
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// Male - ذكر
        /// </summary>
        [Description("Male")]
        Male = 0,
        
        /// <summary>
        /// Female - أنثى
        /// </summary>
        [Description("Female")]
        Female = 1,
        
        /// <summary>
        /// Other - آخر
        /// </summary>
        [Description("Other")]
        Other = 2
    }
}






