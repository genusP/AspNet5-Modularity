using System;
using System.Reflection;

namespace Genus.Modularity
{
    public interface IPluginLoader
    {
        PluginDescriptor LoadPlugin(CandidateDescriptor candidate, Action<Assembly> onAssemblyLoaded =null);
    }
}