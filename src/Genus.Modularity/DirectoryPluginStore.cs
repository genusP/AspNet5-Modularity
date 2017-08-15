using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Genus.Modularity
{
    public class DirectoryPluginStore<T> : DirectoryPluginStore
        where T:IPlugin
    {
        public DirectoryPluginStore(string pluginsDirectoryPath, params string[] pluginNames)
            : base(pluginsDirectoryPath, new AssemblyPluginLoader<T>(), pluginNames)
        {

        }
    }

    public class DirectoryPluginStore : IPluginStore
    {        
        public DirectoryPluginStore(string pluginsDirectoryPath, IPluginLoader pluginLoader, params string[] pluginNames)
        {
            PluginLoader = pluginLoader ?? throw new ArgumentNullException(nameof(pluginLoader)) ;

            if (string.IsNullOrWhiteSpace(pluginsDirectoryPath))
                pluginsDirectoryPath = Directory.GetCurrentDirectory();

            PluginNames = pluginNames ?? new string[0];
            PluginsDirectoryPath = Path.GetFullPath(pluginsDirectoryPath);
        }

        protected string[] PluginNames { get; }
        protected string PluginsDirectoryPath { get; }

        public IPluginLoader PluginLoader { get; }

        public virtual IEnumerable<CandidateDescriptor> CandidatePlugins
        {
            get
            {
                var pluginCandidates = Directory.GetFiles(PluginsDirectoryPath, "*.dll", SearchOption.AllDirectories);
                return from pluginPath in pluginCandidates
                       let pluginName = Path.GetFileNameWithoutExtension(pluginPath)
                       where PluginNames.Length==0 ||  PluginNames.Contains(pluginName)
                       select new CandidateDescriptor(pluginName, pluginPath, GetPluginRoot(pluginPath));
            }
        }

        protected virtual string GetPluginRoot(string pluginPath)
        {
            return Path.GetDirectoryName(pluginPath);
        }
    }
}
