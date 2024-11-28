using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System
{
    public static class AttributeExts
    {
        public static class Types
        {
            public static readonly Type Attribute = typeof(Attribute);
            public static readonly Type AttributeArray = typeof(Attribute[]);
            public static readonly Type AttributeList = typeof(List<Attribute>);
            public static readonly Type DescriptionAttribute = typeof(DescriptionAttribute);
        }

        public static string GetDescription(this Type type) =>
            type
                ?.GetCustomAttributes(Types.DescriptionAttribute, false)
                ?.Cast<DescriptionAttribute>()
                ?.Select(a => a.Description)
                ?.FirstOrDefault();

        public static string GetDescription(this Enum value) =>
            value
                ?.GetType()
                ?.GetField(value.ToString())
                ?.GetCustomAttributes(Types.DescriptionAttribute, false)
                ?.Cast<DescriptionAttribute>()
                ?.Select(a => a.Description)
                ?.FirstOrDefault() ??
            value
                ?.ToString();

        //static AttributeExts() { }
    }
}
