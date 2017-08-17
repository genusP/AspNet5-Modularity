using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Modularity
{
    public sealed class PluginDescriptor
    {
        public PluginDescriptor(IPlugin plugin, string description, string pluginRoot, Assembly assembly)
        {
            Plugin = plugin;
            Description = description;
            PluginRoot = pluginRoot;
            Assembly = assembly;
        }
        public string Name { get { return Plugin.Name; } }

        public IPlugin Plugin { get; }

        public string Description { get;}

        public string PluginRoot { get;}

        public Assembly Assembly { get; }
    }
}
