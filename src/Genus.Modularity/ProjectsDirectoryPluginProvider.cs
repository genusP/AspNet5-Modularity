using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Modularity
{
    public class ProjectsDirectoryPluginProvider : DirectoryPluginProvider
    {
        public ProjectsDirectoryPluginProvider(string[] pluginNames = null, string pluginsDirectoryPath=null, string outputDir=null)
            : base(pluginNames, pluginsDirectoryPath)
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
