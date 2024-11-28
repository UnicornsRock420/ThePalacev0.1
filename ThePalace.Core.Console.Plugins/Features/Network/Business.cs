using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;

namespace ThePalace.Core.Desktop.Plugins.Features.Network
{
    public class Business : IProvider
    {
        public string Name => string.Empty;
        public string Description => string.Empty;

        public DeviceTypes[] Devices => new DeviceTypes[] { DeviceTypes.NONE };
        public FeatureTypes[] Features => new FeatureTypes[] { FeatureTypes.NETWORK };
        public SubFeatureTypes[] SubFeatures => new SubFeatureTypes[] { SubFeatureTypes.BUSINESS };
        public PurposeTypes Purpose => PurposeTypes.PROVIDER;

        public void Dispose() { }

        public void Initialize(params object[] args)
        {
        }

        public object Provide(params object[] args)
        {
            return null;
        }
    }
}
