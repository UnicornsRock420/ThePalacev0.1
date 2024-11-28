using System.Collections.Generic;

namespace ThePalace.Core.Interfaces
{
    public interface ISettingBase
    {
        string Category { get; }
        string Name { get; }
        string Description { get; }

        void Load(params string[] values);
    }

    public interface ISetting : ISettingBase
    {
        string Text { get; }
    }

    public interface ISetting<T> : ISetting
    {
        T Value { get; }
    }

    public interface ISettingList : ISettingBase
    {
        string[] Text { get; }
    }

    public interface ISettingList<T> : ISettingList
    {
        List<T> Values { get; }
    }
}
