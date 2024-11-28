using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
    public static class TypeExts
    {
        private static readonly BindingFlags _bindingFlags1;

        public static class Types
        {
            public static readonly Type Type = typeof(Type);
            public static readonly Type TypeArray = typeof(Type[]);
            public static readonly Type TypeList = typeof(List<Type>);
        }

        static TypeExts()
        {
            _bindingFlags1 = BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
        }

        public static T StaticMethod<T>(this Type type, string methodName, params object[] args) =>
            (T)type
                ?.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
                ?.Invoke(null, args);

        public static T StaticProperty<T>(this Type type, string methodName) =>
            (T)type
                ?.GetProperty(methodName, BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null, null);

        public static IEnumerable<T> FromHierarchy<T>(this T obj, Func<T, T> next, Func<T, bool> @continue = null)
            where T : class
        {
            if (@continue == null)
                @continue = r => r != null;
            for (; @continue(obj);
                obj = next(obj))
                yield return obj;
        }

        public static FieldInfo GetEventField<T>(this T _, string eventName) =>
            GetEventField(typeof(T), eventName);
        public static FieldInfo GetEventField(this Type type, string eventName)
        {
            var _eventName = $"EVENT_{eventName.ToUpperInvariant()}";
            var fieldTypes = (Type[])null;
            var fieldInfo = (FieldInfo)null;

            for (; type != null; type = type.BaseType)
            {
                /* Find events defined as field */
                fieldInfo = type.GetField(eventName, _bindingFlags1);
                fieldTypes = new Type[] { fieldInfo?.FieldType, fieldInfo?.FieldType?.BaseType };
                if (fieldTypes.Contains(DelegateExts.Types.MulticastDelegate)) return fieldInfo;

                /* Find events defined as property { add; remove; } */
                fieldInfo = type.GetField(_eventName, _bindingFlags1);
                if (fieldInfo != null) return fieldInfo;
            }
            return null;
        }

#if WIN64
        public static T ClearBit<T>(this T value, byte bitIndex) where T : struct =>
            ClearBit(value, 1 << bitIndex);
        public static T ClearBit<T>(this T value, long bitValue)
#else
        public static T ClearBit<T>(this T value, byte bitIndex) where T : struct =>
            ClearBit<T>(value, (int)(1 << bitIndex));
        public static T ClearBit<T>(this T value, int bitValue)
#endif
            where T : struct
        {
            var type = typeof(T);
            switch (type.TypeCode())
            {
                case TypeCodeEnum.Byte:
                case TypeCodeEnum.SByte:
                case TypeCodeEnum.Int16:
                case TypeCodeEnum.UInt16:
                case TypeCodeEnum.Int32:
                case TypeCodeEnum.UInt32:
#if WIN64
                case TypeCodeEnum.Int64:
                case TypeCodeEnum.UInt64:
                    var _value = value.CastType<long>();
#else
                    var _value = value.CastType<int>();
#endif
                    return (_value & ~bitValue).CastType<T>();
                default: throw new NotSupportedException($"Type '{type.Name}' is not supported");
            }
        }
#if WIN64
        public static bool IsBitSet<T>(this T value, byte bitIndex) where T : struct =>
            IsBitSet(value, 1 << bitIndex);
        public static bool IsBitSet<T>(this T value, long bitValue)
#else
        public static bool IsBitSet<T>(this T value, byte bitIndex) where T : struct =>
            IsBitSet<T>(value, (int)(1 << bitIndex));
        public static bool IsBitSet<T>(this T value, int bitValue)
#endif
            where T : struct
        {
            var type = typeof(T);
            switch (type.TypeCode())
            {
                case TypeCodeEnum.Byte:
                case TypeCodeEnum.SByte:
                case TypeCodeEnum.Int16:
                case TypeCodeEnum.UInt16:
                case TypeCodeEnum.Int32:
                case TypeCodeEnum.UInt32:
#if WIN64
                case TypeCodeEnum.Int64:
                case TypeCodeEnum.UInt64:
                    var _value = value.CastType<long>();
#else
                    var _value = value.CastType<int>();
#endif
                    return (_value & bitValue) == bitValue;
                default: throw new NotSupportedException($"Type '{type.Name}' is not supported");
            }
        }

        public static int TypeID<T>() =>
            (int)TypeCode(typeof(T));
        public static int TypeID<T>(this T _) =>
            (int)TypeCode(typeof(T));
        public static int TypeID(this Type type) =>
            (int)TypeCode(type);

        public static TypeCodeEnum TypeCode<T>() =>
            TypeCode(typeof(T));
        public static TypeCodeEnum TypeCode<T>(this T _) =>
            TypeCode(typeof(T));
        public static TypeCodeEnum TypeCode(this Type type)
        {
            var result = Type.GetTypeCode(type).CastType<TypeCodeEnum>();
            switch (result)
            {
                case TypeCodeEnum.Object:
                    if (type == GuidExts.Types.Guid) return TypeCodeEnum.Guid;
                    else if (type == TimeSpanExts.Types.TimeSpan) return TypeCodeEnum.TimeSpan;
                    else goto default;
                default:
                    if (type.IsEnum) return TypeCodeEnum.Enum;
                    else return result;
            }
        }

        public static int SizeOf<T>() =>
            Marshal.SizeOf(typeof(T));
        public static int SizeOf<T>(this T obj) =>
            Marshal.SizeOf(obj);
        public static int SizeOf(this Type type) =>
            Marshal.SizeOf(type);

        public static T[] GetArray<T>(this T value) => new T[] { value };
        public static T[] GetArray<T>(this T value, params T[] values) =>
            (new T[] { value }).Union(values).ToArray();
        public static T[] GetArray<T>(this T value, IEnumerable<T> values) =>
            (new T[] { value }).Union(values).ToArray();
        public static T[] GetArray<T>(this T[] values1, IEnumerable<T> values2) =>
            values1.Union(values2).ToArray();
        public static T[] GetArray<T>(this IEnumerable<T> values1, IEnumerable<T> values2) =>
            values1.Union(values2).ToArray();

        public static List<T> GetList<T>(this T value) =>
            new List<T> { value };
        public static List<T> GetList<T>(this T value, params T[] values) =>
            new List<T> { value }.Union(values).ToList();
        public static List<T> GetList<T>(this T value, IEnumerable<T> values) =>
            new List<T> { value }.Union(values).ToList();
        public static List<T> GetList<T>(this IEnumerable<T> values1, IEnumerable<T> values2) =>
            values1.Union(values2).ToList();

        public static IReadOnlyList<T> IReadOnlyList<T>(this T value) =>
            new List<T> { value }.AsReadOnly();
        public static IEnumerable<T> IEnumerable<T>(this T value) =>
            new List<T> { value }.AsEnumerable();
        public static IQueryable<T> IQueryable<T>(this T value) =>
            new List<T> { value }.AsQueryable();
        public static void RaiseEvent<T>(this T obj, string eventName, params object[] args)
        {
            args = (args?.Length ?? -1) < 1 ? obj.GetArray(args.AsEnumerable()) : obj.GetArray((object)new EventArgs());

            if (typeof(T).GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo fieldInfo && fieldInfo.GetValue(obj) is MulticastDelegate multicastDelegate)
                foreach (var @delegate in multicastDelegate.GetInvocationList())
                    @delegate.DynamicInvoke(@delegate.Target, args);
        }

        public static T GetInstance<T>()
            where T : class =>
            GetInstance<T>(typeof(T));

        public static T GetInstance<T>(this Type type, params object[] args)
            where T : class =>
            (T)Activator.CreateInstance(type, args);

        public static object GetInstance(this Type type, params object[] args) =>
            Activator.CreateInstance(type, args);

        public static object InvokeMethod<T>(this T source, string methodName, params string[] args) =>
            typeof(T).InvokeMember(methodName, BindingFlags.Default | BindingFlags.InvokeMethod, null, source, args);
    }
}
