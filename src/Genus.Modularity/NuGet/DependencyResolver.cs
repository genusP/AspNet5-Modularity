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
                .SelectMany(s => s.GetCandidates(assemblyName))
                .OrderBy(c=>c.Priority)
                .ThenByDescending(c=>c.Package.AssemblyVersion)
                .Select(c => c.Package.AssemblyPath)
                .FirstOrDefault(ap => ap != null);
        }

        public static void AddDefaultStore()
        {
            foreach(var store in GetDefaultStores())
                PackageStores.Add(store);
        }

        private static IEnumerable<IPackageStore> GetDefaultStores()
            => GetProbeDirectories().Select(path => new FolderPackageStrore(path));

        private static IEnumerable<string> GetProbeDirectories()
        {
            //#if NETSTANDARD1_5||NETSTANDARD2_0
            //            var storePath = @"C:\Program Files\dotnet\store\x64\netcoreapp2.0";
            //            if(Directory.Exists(storePath))
            //                return new FolderPackageStrore( storePath);
            //            return null;
            //#endif
#if NETSTANDARD1_6||NETSTANDARD2_0
            var probeDirectories = AppContext.GetData("PROBING_DIRECTORIES");
#else
            var probeDirectories = AppDomain.CurrentDomain.GetData("PROBING_DIRECTORIES");
#endif
            var listOfDirectories = probeDirectories as string;
            if (!string.IsNullOrEmpty(listOfDirectories))
            {
                return listOfDirectories.Split(new char[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            }

            string basePath = Environment.GetEnvironmentVariable("HOME");
            if(string.IsNullOrEmpty(basePath)||!Directory.Exists(basePath))
                basePath = Environment.GetEnvironmentVariable("USERPROFILE"); ;

            if (string.IsNullOrEmpty(basePath))
                return new string[0];

            return new string[] { Path.Combine(basePath, ".nuget", "packages") };
        }
    }
}
