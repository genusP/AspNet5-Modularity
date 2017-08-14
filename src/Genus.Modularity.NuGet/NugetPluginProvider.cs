using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Genus.Modularity;

namespace Genus.Modularity.NuGet
{
    public class NugetPluginProvider:IPluginProvider
    {
        public NugetPluginProvider(string packageStorePath, params string[] pluginNames) 
            :this(new FolderPackageStrore(packageStorePath), pluginNames)
        {

        }
        public NugetPluginProvider(IPackageStore store, params string[] pluginNames)
        {
            _store = store;
            _pluginNames = pluginNames;
        }

        readonly string[] _pluginNames;
        readonly IPackageStore _store;

        public IEnumerable<CandidateDescriptor> CandidatePlugins
        {
            get
            {
                return from pn in _pluginNames
                       let packageDescriptor = _store.GetPackageDescriptor(new AssemblyName(pn))
                       where packageDescriptor !=null
                       select new CandidateDescriptor(pn, packageDescriptor.AssemblyPath, packageDescriptor.ContentPath);
            }
        }
    }
}
