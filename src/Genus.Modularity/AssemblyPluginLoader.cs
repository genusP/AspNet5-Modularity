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
        public PluginDescriptor LoadPlugin(CandidateDescriptor candidate)
        {
            var assemblyPath = Path.GetFullPath(candidate.AssemblyPath);
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
            var assembly = Assembly.Load(new AssemblyName(assemblyName));
            if (assembly == null)
            {
#if NET451
                assembly = Assembly.LoadFile(assemblyPath);
#else
                assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
#endif
            }

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
