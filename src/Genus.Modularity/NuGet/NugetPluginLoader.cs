using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Modularity.NuGet
{
    public class NugetPluginLoader<T>:AssemblyPluginLoader<T>
        where T: IPlugin
    {
        

        static NugetPluginLoader()
        {
            DependencyResolver.Attach();
        }

        public NugetPluginLoader(string packageStorePath) : this(packageStorePath, false) { }

        public NugetPluginLoader(string packageStorePath, bool useDefaultStore) : this(new FolderPackageStrore(packageStorePath), true) { }

        public NugetPluginLoader(IPackageStore packageStore) : this(packageStore, false) { }

        public NugetPluginLoader(IPackageStore packageStore, bool useDefaultStore)
        {
            DependencyResolver.PackageStores.Add(packageStore);
            if (useDefaultStore)
                DependencyResolver.AddDefaultStore();
        }        
    }
}
