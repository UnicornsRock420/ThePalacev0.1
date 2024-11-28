using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;

namespace ThePalace.Core.Desktop.Plugins.Features.Network
{
    public sealed class Business : Disposable, IProvider
    {
        public string Name => string.Empty;
        public string Description => string.Empty;

        public DeviceTypes[] Devices => new DeviceTypes[] { DeviceTypes.NONE };
        public FeatureTypes[] Features => new FeatureTypes[] { FeatureTypes.NETWORK };
        public SubFeatureTypes[] SubFeatures => new SubFeatureTypes[] { SubFeatureTypes.BUSINESS };
        public PurposeTypes Purpose => PurposeTypes.PROVIDER;

        public override void Dispose() =>
            this.Dispose(false);

        public void Initialize(params object[] args)
        {
        }

        public object Provide(params object[] args)
        {
            return null;
        }
    }
}
