using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Enums;

namespace ThePalace.Core.Desktop.Plugins.Features.Media
{
    public sealed class Audio : Disposable, IProvider, IConsumer
    {
        public string Name => string.Empty;
        public string Description => string.Empty;

        public DeviceTypes[] Devices => new DeviceTypes[] { DeviceTypes.NONE };
        public FeatureTypes[] Features => new FeatureTypes[] { FeatureTypes.MEDIA };
        public SubFeatureTypes[] SubFeatures => new SubFeatureTypes[] { SubFeatureTypes.AUDIO };
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

        public void Consume(params object[] args)
        {
        }
    }
}
