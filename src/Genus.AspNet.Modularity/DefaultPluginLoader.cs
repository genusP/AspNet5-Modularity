using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;

namespace Genus.AspNet.Modularity
{
    public class DefaultPluginLoader : IPluginLoader
    {
        private IEnumerable<string> _assemblyNames;
        private ILogger logger;

        public DefaultPluginLoader(IEnumerable<string> assemblyNames, ILogger logger)
        {
            if (assemblyNames == null)
                throw new ArgumentNullException(nameof(assemblyNames));
            if (logger==null)
                throw new ArgumentNullException(nameof(logger));

            _assemblyNames = assemblyNames;
            this.logger = logger;
        }

        public IEnumerable<IPlugin> LoadPlugins() {

            logger.LogInformation("Begin load plugins");
            foreach (var assemblyPath in _assemblyNames)
            {
                logger.LogInformation($"Load plugins from {assemblyPath}");
                foreach (var plugin in GetPluginsFromAssembly(assemblyPath))
                {
                    yield return plugin;
                }
            }
            logger.LogInformation("End module load");
        }

        private IEnumerable<IPlugin> GetPluginsFromAssembly(string assemblyName)
        {
            Assembly assembly;
            try
            {
#if NET451
                assembly = Assembly.Load( assemblyName);
#else
                assembly = Assembly.Load( new AssemblyName(assemblyName));
#endif
            }
            catch (Exception ex)
            {
                logger.LogError("Error with loading assembly", ex);
                yield break;
            }
            var item = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t));
            if(item!=null)
            {
                var plugin = CreatePluginInstance(item);
                if (plugin != null)
                    yield return plugin;
            }
        }

        private IPlugin CreatePluginInstance(Type item)
        {
            var defaultCtor = item.GetConstructor(Type.EmptyTypes);
            if(defaultCtor==null)
            {
                logger.LogError($"Plugin {item.FullName} skiped. Constructor without parameterd not found.");
                return null;
            }
            return (IPlugin) defaultCtor.Invoke(new object[0]);
        }
    }
}
