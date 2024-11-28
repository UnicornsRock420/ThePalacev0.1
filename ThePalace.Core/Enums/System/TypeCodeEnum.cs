namespace System
{
    /// <summary>
    /// <see cref="TypeCode"/>
    /// </summary>
    public enum TypeCodeEnum : int
    {
        Void = unchecked(-1),
        Empty = TypeCode.Empty,
        Object = TypeCode.Object,
        DBNull = TypeCode.DBNull,
        Boolean = TypeCode.Boolean,
        Char = TypeCode.Char,
        SByte = TypeCode.SByte,
        Byte = TypeCode.Byte,
        Int16 = TypeCode.Int16,
        UInt16 = TypeCode.UInt16,
        Int32 = TypeCode.Int32,
        UInt32 = TypeCode.UInt32,
        Int64 = TypeCode.Int64,
        UInt64 = TypeCode.UInt64,
        Single = TypeCode.Single,
        Double = TypeCode.Double,
        Decimal = TypeCode.Decimal,
        DateTime = TypeCode.DateTime,
        String = TypeCode.String,
        Interface = unchecked(19),
        Enum = unchecked(20),
        Guid = unchecked(21),
        TimeSpan = unchecked(22),
        Max
    }
}
