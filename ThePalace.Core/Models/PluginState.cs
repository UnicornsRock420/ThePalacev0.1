using System.Reflection;
using System.Runtime.Loader;

namespace ThePalace.Core.Models
{
    public sealed class PluginState : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName) => null;
    }
}
