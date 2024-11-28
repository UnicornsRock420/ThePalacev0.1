using ThePalace.Core.Client.Core.Interfaces;
using ThePalace.Core.Console.Plugins.Options.Core.Scripting.JavaScript;
using ThePalace.Core.Enums;

namespace ThePalace.Core.Console.Plugins.Features.Core.Scripting
{
    public class JavaScript : IProvider
    {
        public string Name => string.Empty;
        public string Description => string.Empty;

        public DeviceTypes[] Devices => new DeviceTypes[] { DeviceTypes.NONE };
        public FeatureTypes[] Features => new FeatureTypes[] { FeatureTypes.CORE };
        public SubFeatureTypes[] SubFeatures => new SubFeatureTypes[] { SubFeatureTypes.SCRIPTING };
        public PurposeTypes Purpose => PurposeTypes.PROVIDER;

        public void Dispose() { }

        public void Initialize(params object[] args)
        {
        }

        public object Provide(params object[] args)
        {
            if (Cyborgs.Current.Value)
            {
            }

            return null;
        }
    }
}
