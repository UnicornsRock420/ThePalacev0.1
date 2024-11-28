using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace System
{
    public static class ObjectExts
    {
        public static class Types
        {
            public static readonly Type Object = typeof(Object);
            public static readonly Type ObjectArray = typeof(Object[]);
            public static readonly Type ObjectList = typeof(List<Object>);
            public static readonly Type ExpandoObject = typeof(ExpandoObject);
        }

        //static ObjectExts() { }

        public static int SizeOf(this object obj) =>
            Marshal.SizeOf(obj);

        public static IntPtr ToIntPtr<T>(this T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), nameof(obj) + " cannot be null");

            var result = IntPtr.Zero;
            var type = typeof(T);
            if (type.IsClass)
            {
                var handle = null as GCHandle?;
                try
                {
                    handle = GCHandle.Alloc(obj);

                    if (handle.HasValue)
                        result = GCHandle.ToIntPtr(handle.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.Trim());
                }
                finally
                {
                    if (handle?.IsAllocated == true)
                        handle?.Free();
                }
            }
            else
            {
                var handle = IntPtr.Zero;

                try
                {
                    handle = Marshal.AllocHGlobal(obj.SizeOf());
                    Marshal.StructureToPtr(obj, result, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.Trim());
                }
                finally
                {
                    if (handle != IntPtr.Zero)
                        Marshal.FreeHGlobal(handle);
                }
            }
            return result;
        }

        public static T FromIntPtr<T>(this IntPtr obj) =>
            (T)GCHandle.FromIntPtr(obj).Target;

        public static void ClearEvents(this object obj, string eventName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj), nameof(obj) + " cannot be null");
            else if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentNullException(nameof(eventName), nameof(eventName) + " cannot be null");

            var fi = obj.GetType()?.GetEventField(eventName);
            if (fi == null) throw new NullReferenceException($"Event field {eventName} was not found");

            fi.SetValue(obj, null);
        }

        public static T CastType<T>(this object obj)
        {
            var type = typeof(T);
            if (type.IsEnum)
#if WIN64
                try { return (T)(object)(long)obj; } catch { }
#else
                try { return (T)(object)(int)obj; } catch { }
#endif
            try { return (T)obj; } catch { }
            return default(T);
        }

        public static T TryParse<T>(this object value, T defaultValue = default(T), string format = null) => StringExts.TryParse(value.ToString(), defaultValue, format);
    }
}
