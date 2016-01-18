using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.AspNet.Modularity
{
    public sealed class PluginInfo
    {
        internal PluginInfo(string name, string description, string pluginRoot)
        {
            Name = name;
            Description = description;
            PluginRoot = pluginRoot;
        }
        public string Name { get; private set; }

        public string Description { get; private set; }

        public string PluginRoot { get; private set; }
    }
}
