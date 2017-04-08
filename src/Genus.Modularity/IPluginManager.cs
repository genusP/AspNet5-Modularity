using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genus.Modularity
{
    public interface IPluginManager
    {
        IEnumerable<PluginDescriptor> LoadedPlugins { get; }

        PluginDescriptor this[TypeInfo type]{ get; }
    }
}