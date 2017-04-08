using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Genus.Modularity
{
    public sealed class PluginManager: IPluginManager
    {
        private IList<PluginDescriptor> pluginDescriptorsList;
        private IPluginLoader Loader { get; }
        private IPluginProvider Provider { get; }
        private ILogger Logger { get;  }

        public PluginManager(IPluginProvider provider, IPluginLoader loader, ILogger logger = null)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (loader == null)
                throw new ArgumentNullException(nameof(loader));
            Provider = provider;
            Loader = loader;
            Logger = logger;
        }

        public IEnumerable<PluginDescriptor> LoadedPlugins
        {
            get
            {
                if (pluginDescriptorsList == null)
                    return Enumerable.Empty<PluginDescriptor>();
                return pluginDescriptorsList.Select(p => p);
            }
        }

        public PluginDescriptor this[TypeInfo type]
            => LoadedPlugins.SingleOrDefault(t => t.Assembly == type.Assembly);

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            if (this.pluginDescriptorsList != null)
                throw new InvalidOperationException("Already inizialized");

            Logger?.LogInformation("Begin load plugins");
            pluginDescriptorsList = new List<PluginDescriptor>();
            foreach (var candidate in Provider.CandidatePlugins)
            {
                try
                {
                    Logger?.LogInformation($"Load plugins from {candidate.PluginName}");
                    var descriptor = Loader.LoadPlugin(candidate);
                    pluginDescriptorsList.Add(descriptor);
                    descriptor.Plugin.ConfigureServices(serviceCollection);
                }
                catch(CreatePluginException ex)
                {
                    if(Logger!=null)
                        Logger.LogError(ex.Message);
                    throw;
                }
            }
            Logger?.LogInformation("End module load");
        }
    }
}
