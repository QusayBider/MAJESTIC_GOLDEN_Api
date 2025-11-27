using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MAJESTIC_GOLDEN_Api.DAL.Enums
{
    
    public static class EnumExtensions
    {
       
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();
            
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
        
      
        public static T ParseEnum<T>(string value) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;
                
            if (Enum.TryParse<T>(value, true, out var result))
                return result;
                
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
        
      
        public static string ToStatusString(this Enum value)
        {
            return GetDescription(value);
        }
    }
}






