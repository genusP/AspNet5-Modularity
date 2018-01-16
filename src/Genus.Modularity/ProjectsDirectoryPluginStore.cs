using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Modularity
{
    public class ProjectsDirectoryPluginStore<T> : ProjectsDirectoryPluginStore
        where T: IPlugin
    {
        public ProjectsDirectoryPluginStore(string pluginsDirectoryPath, params string[] pluginNames)
            :this (pluginsDirectoryPath, null, pluginNames)
        { }

        public ProjectsDirectoryPluginStore(string pluginsDirectoryPath, string outputDir, params string[] pluginNames)
            :base(pluginsDirectoryPath, new AssemblyPluginLoader<T>(), outputDir, pluginNames)
        { }
    }

    public class ProjectsDirectoryPluginStore : DirectoryPluginStore
    {
        static ProjectsDirectoryPluginStore()
        {
            NuGet.DependencyResolver.AddDefaultStore();
            NuGet.DependencyResolver.Attach();
        }
        public ProjectsDirectoryPluginStore(string pluginsDirectoryPath, IPluginLoader loader, params string[] pluginNames)
            :this(pluginsDirectoryPath, loader, null, pluginNames)
        { }

        public ProjectsDirectoryPluginStore(string pluginsDirectoryPath, IPluginLoader loader, string outputDir, params string[] pluginNames)
            : base(pluginsDirectoryPath, loader, pluginNames)
        {
            OutputDir = string.IsNullOrWhiteSpace(outputDir)
                                ? "bin/Debug"
                                : outputDir;
        }

        protected string OutputDir { get; }

        public override IEnumerable<CandidateDescriptor> CandidatePlugins
        {
            get
            {
                return from pluginName in PluginNames
                       from dir in Directory.EnumerateDirectories(PluginsDirectoryPath, pluginName)
                       let searchDir = Path.Combine(dir, OutputDir)
                       where Directory.Exists(searchDir)
                       from asm in Directory.GetFiles(searchDir, pluginName + ".dll", SearchOption.AllDirectories)
                       select new CandidateDescriptor(
                           pluginName,
                           asm,
                           dir
                           );
            }
        }
    }
}
