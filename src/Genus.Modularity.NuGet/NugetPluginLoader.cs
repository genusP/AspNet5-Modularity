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
        static class DependencyResolver
        {
#if NET451
            public static void Attach()
                => AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            
            static Assembly Resolver( object s, ResolveEventArgs e)
            {
                var path = GetAssemblyPath(new AssemblyName( e.Name));
                if(!string.IsNullOrEmpty(path))
                    return Assembly.LoadFile(path);
                return null;
            }
#else
            public static void Attach() 
                => System.Runtime.Loader.AssemblyLoadContext.Default.Resolving+= Resolver;

            static Assembly Resolver(System.Runtime.Loader.AssemblyLoadContext context, AssemblyName an)
            {
                var path = GetAssemblyPath(an);
                if (!string.IsNullOrEmpty(path))
                {
                    var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                    return assembly;
                }
                return null;
            }
#endif

            public static List<IPackageStore> PackageStores = new List<IPackageStore>();
            private static string GetAssemblyPath(AssemblyName assemblyName)
            {
                return PackageStores
                    .Select(s => s.GetPackageDescriptor(assemblyName)?.AssemblyPath)
                    .FirstOrDefault(ap => ap != null);
            }
        }

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
            {
                var store = GetDefaultStore();
                if (store != null)
                    DependencyResolver.PackageStores.Add(store);
            }
        }

        private IPackageStore GetDefaultStore()
        {
            var profilePath =  Environment.GetEnvironmentVariable("USERPROFILE");
            var storePath = Path.Combine(profilePath, ".nuget", "packages");
            return new FolderPackageStrore(storePath);
        }

        protected override Assembly LoadAssembly(string assemblyPath, AssemblyName assemblyName)
        {
            return Assembly.Load(assemblyName);
        }        
    }

}
