using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Modularity
{
    public class CandidateDescriptor
    {
        public CandidateDescriptor(string pluginName, string assemblyPath, string contentRoot)
        {
            if (string.IsNullOrEmpty(pluginName))
                throw new ArgumentException(nameof(pluginName), "Need not empty string");
            if (string.IsNullOrEmpty(assemblyPath))
                throw new ArgumentException(nameof(assemblyPath), "Need not empty string");
            if (string.IsNullOrEmpty(contentRoot))
                throw new ArgumentException(nameof(contentRoot), "Need not empty string");

            PluginName = pluginName;
            AssemblyPath = assemblyPath;
            ContentRoot = contentRoot;
        }
        public string AssemblyPath { get;}
        public string ContentRoot { get;}
        public object PluginName { get;}
    }
}
