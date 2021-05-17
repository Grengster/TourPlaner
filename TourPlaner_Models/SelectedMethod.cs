using System;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace TourPlaner_Models
{
    public enum SelectedMethod
    {   [Description("fastest")]
        Car,
        [Description("bicycle")]
        Bicycle,
        [Description("pedestrian")]
        Walking
    };

    public static class EnumDescriptionExtension
    {
        public static string GetDescription(this Enum value)
        {
            return ((DescriptionAttribute)Attribute.GetCustomAttribute(
                value.GetType().GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Single(x => x.GetValue(null).Equals(value)),
                typeof(DescriptionAttribute)))?.Description ?? value.ToString();
        }
    }
}
