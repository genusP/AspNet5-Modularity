using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Modularity.NuGet
{
    public static class DependencyResolver
    {
#if NET451
        public static void Attach()
            => AppDomain.CurrentDomain.AssemblyResolve += Resolver;

        static Assembly Resolver(object s, ResolveEventArgs e)
        {
            var path = GetAssemblyPath(new AssemblyName(e.Name));
            if (!string.IsNullOrEmpty(path))
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

        public static void AddDefaultStore()
        {
            var store = GetDefaultStore();
            if (store != null)
                PackageStores.Add(store);
        }

        private static IPackageStore GetDefaultStore()
        {
            var profilePath = Environment.GetEnvironmentVariable("USERPROFILE");
            var storePath = Path.Combine(profilePath, ".nuget", "packages");
            return new FolderPackageStrore(storePath);
        }
    }
}
