using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Genus.Modularity
{
    public class AssemblyPluginLoader<T> : IPluginLoader
        where T:IPlugin
    {
        static AssemblyPluginLoader()
        {
            DefaultPluginDependencyResolver.Attach(); //Attach dependency resolver
        }

        public virtual PluginDescriptor LoadPlugin(CandidateDescriptor candidate, Action<Assembly> onAssemblyLoad = null)
        {
            var assemblyPath = Path.GetFullPath(candidate.AssemblyPath);
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
            var assemblyDir  = Path.GetDirectoryName(assemblyPath);

            Assembly assembly = LoadAssembly(assemblyPath, new AssemblyName( assemblyName));

            DefaultPluginDependencyResolver.AddProbingPath(assemblyDir);
            if (onAssemblyLoad != null)
#pragma warning disable IDE1005 // Delegate invocation can be simplified.
                onAssemblyLoad(assembly);
#pragma warning restore IDE1005 // Delegate invocation can be simplified.

            var plugin = GetPluginFromAssembly(assembly);
            if (plugin != null)
            {
                return new PluginDescriptor(
                    plugin,
                    (plugin as IPluginWithDescription)?.Description,
                    candidate.ContentRoot,
                    assembly);
            }
            return null;
        }

        protected virtual Assembly LoadAssembly(string assemblyPath, AssemblyName assemblyName)
        {
            //try get already loaded assembly
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (FileNotFoundException) { } //catch exception if assembly not loaded

#if NET451
            return Assembly.LoadFile(assemblyPath);
#else
            return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
#endif
        }

        private IPlugin GetPluginFromAssembly(Assembly assembly)
        {
            var pluginType = typeof(T);
            var item = assembly.DefinedTypes.FirstOrDefault(t => pluginType.GetTypeInfo().IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType);

            if (item != null)
            {
                var plugin = CreatePluginInstance(item);
                if (plugin != null)
                    return plugin;
            }
            return null;
        }

        private IPlugin CreatePluginInstance(TypeInfo item)
        {
            var defaultCtor = item.DeclaredConstructors.SingleOrDefault(c=>c.GetParameters().Length == 0);
            if (defaultCtor == null)
                throw new CreatePluginException($"Plugin {item.FullName} skiped. Constructor without parameter not found.");

            return (IPlugin)defaultCtor.Invoke(new object[0]);
        }
    }
}
