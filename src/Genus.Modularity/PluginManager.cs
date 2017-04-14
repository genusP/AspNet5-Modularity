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
        private Lazy<IList<PluginDescriptor>> pluginDescriptorsList;
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
            pluginDescriptorsList = new Lazy<IList<PluginDescriptor>>(() => LoadPlugins().ToList());
        }

        public IEnumerable<PluginDescriptor> LoadedPlugins
        {
            get
            {
                return pluginDescriptorsList.Value.Select(p => p);
            }
        }

        public PluginDescriptor this[TypeInfo type]
            => LoadedPlugins.SingleOrDefault(t => t.Assembly == type.Assembly);

        bool isConfigureServicesCalled = false;
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            if (isConfigureServicesCalled)
                throw new InvalidOperationException("Already inizialized");
            isConfigureServicesCalled = true;
            foreach (var descriptor in LoadedPlugins)
            {
                    descriptor.Plugin.ConfigureServices(serviceCollection);
            }
        }

        private IEnumerable<PluginDescriptor> LoadPlugins()
        {
            Logger?.LogInformation("Begin load plugins");
            foreach (var candidate in Provider.CandidatePlugins)
            {
                Logger?.LogInformation($"Load plugin from {candidate.PluginName}");
                PluginDescriptor descriptor;
                try
                {
                    descriptor = Loader.LoadPlugin(candidate);
                }
                catch(CreatePluginException ex)
                {
                    if(Logger!=null)
                        Logger.LogError(ex.Message);
                    throw;
                }
                yield return descriptor;
            }
            Logger?.LogInformation("End load plugins");
        }
    }
}
