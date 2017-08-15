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
        private IPluginStore Store { get; }
        private ILogger Logger { get;  }

        public PluginManager(IPluginStore store, ILogger logger = null)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            Store = store;
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
            foreach (var candidate in Store.CandidatePlugins)
            {
                Logger?.LogInformation($"Load plugin from {candidate.PluginName}");
                PluginDescriptor descriptor;
                try
                {
                    descriptor = Store.PluginLoader.LoadPlugin(candidate);
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
