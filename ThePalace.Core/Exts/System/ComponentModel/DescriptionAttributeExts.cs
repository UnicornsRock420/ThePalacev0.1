using System.Linq;

namespace System.ComponentModel
{
    public static class DescriptionAttributeExts
    {
        public static class Types
        {
            public static readonly Type DescriptionAttribute = typeof(DescriptionAttribute);
            public static readonly Type DescriptionAttributeArray = typeof(DescriptionAttribute[]);
        }

        //static DescriptionAttributeExts() { }

        public static string GetDescription<T>(this T value)
        {
            var type = typeof(T);
            var _value = value?.ToString()?.Trim();
            return
                string.IsNullOrWhiteSpace(_value) ? null
                : (!(type?.IsEnum ?? false) ? null
                    : type?.GetField(_value)?.GetCustomAttributes(Types.DescriptionAttribute, true)
                ?? ((object)value as Type)?.GetCustomAttributes(Types.DescriptionAttribute, true)
                ?? type?.GetCustomAttributes(Types.DescriptionAttribute, true))
                    ?.Cast<DescriptionAttribute>()
                    ?.Select(a => a?.Description)
                    ?.Where(s => s != null)
                    ?.FirstOrDefault()
                ?? _value;
        }
    }
}
