using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genus.AspNetCore.Modularity
{
    public interface IPluginManager
    {
        IEnumerable<PluginInfo> LoadedPlugins { get; }

        PluginInfo this[TypeInfo type]{ get; }
    }
}