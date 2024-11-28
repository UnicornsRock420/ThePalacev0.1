using System.Collections.Generic;

namespace ThePalace.Core.Interfaces
{
    public interface IOption : ISettingBase
    {
        string Text { get; }

        bool Enabled();
        //object Parse(string value);
    }

    public interface IOption<T> : IOption
    {
        IReadOnlyDictionary<string, T> Values { get; }
        T Value { get; set; }
    }
}
