using System.ComponentModel;
using System.Reflection;

namespace Mps.Domain.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute?)field?.GetCustomAttribute(typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static T GetEnum<T>(this int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
    }
}
