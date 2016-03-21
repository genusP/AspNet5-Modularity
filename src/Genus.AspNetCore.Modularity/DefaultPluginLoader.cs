using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Genus.AspNetCore.Modularity
{
    public class DefaultPluginLoader : IPluginLoader
    {
        private readonly ILogger _logger;

        public DefaultPluginLoader(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            _logger = logger;
        }

        public IEnumerable<IPlugin> LoadPlugins() {

            _logger.LogInformation("Begin load plugins");
            var lm = DnxPlatformServices.Default.LibraryManager;
            foreach (var depend in lm.GetLibrary(PlatformServices.Default.Application.ApplicationName).Dependencies)
            {
                _logger.LogInformation($"Load plugins from {depend}");
                var library = lm.GetLibrary(depend);
                foreach (var plugin in GetPluginsFromLibrary(library))
                {
                    yield return plugin;
                }
            }
            _logger.LogInformation("End module load");
        }

        private IEnumerable<IPlugin> GetPluginsFromLibrary(Library library)
        {
            foreach (var assemblyName in library.Assemblies)
            {
                TypeInfo item;
                try
                {
                    var assembly = Assembly.Load(assemblyName);
                    item = assembly.DefinedTypes.FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType);
                }
                catch
                {
                    yield break;
                }
                if (item != null)
                {
                    var plugin = CreatePluginInstance(item);
                    if (plugin != null)
                        yield return plugin;
                }
            }
        }

        private IPlugin CreatePluginInstance(Type item)
        {
            var defaultCtor = item.GetConstructor(Type.EmptyTypes);
            if(defaultCtor==null)
            {
                _logger.LogError($"Plugin {item.FullName} skiped. Constructor without parameterd not found.");
                return null;
            }
            return (IPlugin) defaultCtor.Invoke(new object[0]);
        }
    }
}
