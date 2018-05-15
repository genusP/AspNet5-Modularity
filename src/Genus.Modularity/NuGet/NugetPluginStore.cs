using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Modularity.NuGet
{
    public class NugetPluginStore<T>:NugetPluginStore
        where T: IPlugin
    {
        public NugetPluginStore(string packageStorePath, params string[] pluginNames) 
            :this(new FolderPackageStrore(packageStorePath), pluginNames)
        {

        }

        public NugetPluginStore(IPackageStore store, params string[] pluginNames)
            :base(store, new NugetPluginLoader<T>(store), pluginNames)
        {

        }
    }

    public class NugetPluginStore: IPluginStore
    {
        
        public NugetPluginStore(IPackageStore store, IPluginLoader pluginLoader, params string[] pluginNames)
        {
            PluginLoader = pluginLoader ?? throw new ArgumentNullException(nameof(pluginLoader));
            _store = store;
            _pluginNames = pluginNames;
        }

        readonly string[] _pluginNames;
        readonly IPackageStore _store;

        public IPluginLoader PluginLoader { get; }

        public IEnumerable<CandidateDescriptor> CandidatePlugins
        {
            get
            {
                return from pn in _pluginNames
                       let packageDescriptor = _store.GetCandidates(new AssemblyName(pn))
                                                .OrderBy(c=>c.Priority)
                                                .ThenByDescending(c=>c.Package.AssemblyVersion)
                                                .Select(c=>c.Package)
                                                .FirstOrDefault()
                       where packageDescriptor !=null
                       select new CandidateDescriptor(pn, packageDescriptor.AssemblyPath, packageDescriptor.ContentPath);
            }
        }
    }
}
