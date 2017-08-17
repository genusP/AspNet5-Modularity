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
            foreach(var store in GetDefaultStores())
                PackageStores.Add(store);
        }

        private static IEnumerable<IPackageStore> GetDefaultStores()
        {
            var profilePath = Environment.GetEnvironmentVariable("USERPROFILE");
            var storePath = Path.Combine(profilePath, ".nuget", "packages");
            yield return new FolderPackageStrore(storePath);
#if NETSTANDARD2_0
            var coreapp2store = GetDefaultNetCoreApp2Store();
            if (coreapp2store != null)
                yield return coreapp2store;
        }

        private static IPackageStore GetDefaultNetCoreApp2Store()
        {
            var storePath = @"C:\Program Files\dotnet\store\x64\netcoreapp2.0";
            if(Directory.Exists(storePath))
                return new FolderPackageStrore( storePath);
            return null;
#endif
        }
    }
}
