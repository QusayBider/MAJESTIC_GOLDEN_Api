using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    /// <summary>
    /// Extension methods for Enum types
    /// طرق توسيع لأنواع Enum
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get Description attribute value from enum
        /// الحصول على قيمة Description من enum
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();
            
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
        
        /// <summary>
        /// Parse string to enum value
        /// تحويل نص إلى قيمة enum
        /// </summary>
        public static T ParseEnum<T>(string value) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;
                
            if (Enum.TryParse<T>(value, true, out var result))
                return result;
                
            // Try to find by Description
            var type = typeof(T);
            foreach (var field in type.GetFields())
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute?.Description.Equals(value, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return (T)field.GetValue(null)!;
                }
            }
            
            return default;
        }
        
        /// <summary>
        /// Convert enum to string (Description if available, otherwise name)
        /// تحويل enum إلى نص
        /// </summary>
        public static string ToStatusString(this Enum value)
        {
            return GetDescription(value);
        }
    }
}






