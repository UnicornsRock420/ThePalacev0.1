using System;
using ThePalace.Core.Enums;

namespace ThePalace.Core.Client.Core.Interfaces
{
    public interface IFeature : IDisposable
    {
        string Name { get; }
        string Description { get; }

        DeviceTypes[] Devices { get; }
        FeatureTypes[] Features { get; }
        SubFeatureTypes[] SubFeatures { get; }
        PurposeTypes Purpose { get; }

        void Initialize(params object[] args);
    }
}
