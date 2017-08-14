using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Modularity
{
    public static class DefaultPluginDependencyResolver
    {
        private static readonly List<string> _probingPaths = new List<string>();

        public static void AddProbingPath(string path) {
            if (!Directory.Exists(path))
                throw new ArgumentException($"Directory '{path}' not found");
            if(!_probingPaths.Contains(path))
                _probingPaths.Add(path);
        }

#if NET451
        public static void Attach()
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
        }

        static Assembly Resolver(object sender, ResolveEventArgs e)
        {
            var asmPath = GetAssemblyPath(e.Name);
            if (asmPath != null)
                return Assembly.LoadFile(asmPath);
            return null;
        }
#else
        public static void Attach() {
            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += Resolver;
        }

        private static Assembly Resolver(System.Runtime.Loader.AssemblyLoadContext context, AssemblyName assemblyName)
        {
            var asmPath = GetAssemblyPath(assemblyName.Name);
            if (asmPath != null)
                return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(asmPath);
            return null;
        }
#endif

        private static string GetAssemblyPath(string asemblyName)
        {
            var fileName = asemblyName + ".dll";
            return _probingPaths
                        .SelectMany(p => Directory.GetFiles(p, fileName))
                        .FirstOrDefault();
        }
    }
}
