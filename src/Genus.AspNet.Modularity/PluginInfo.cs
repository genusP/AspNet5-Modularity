using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.AspNet.Modularity
{
    public sealed class PluginInfo
    {
        internal PluginInfo(IPlugin plugin, string description, string pluginRoot, Assembly assembly)
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
