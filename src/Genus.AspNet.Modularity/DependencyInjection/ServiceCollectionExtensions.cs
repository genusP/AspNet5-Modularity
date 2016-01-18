using Genus.AspNet.Modularity;
using Genus.AspNet.Modularity.ApplicationModel;
using Genus.AspNet.Modularity.ViewFutures;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IPluginManager AddPlugins(this IServiceCollection services, IPluginLoader loader)
        {
            var pluginManager = new PluginManager(loader);
            pluginManager.ConfigureServices(services);
            services.Add(new ServiceDescriptor(typeof(IPluginManager), pluginManager));
            services.AddPluginApplicationModel();
            services.AddPluginViewLocations(pluginManager);
            return pluginManager;
        }

        public static IPluginManager AddPluginsFromConfig(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            var options = configuration.Get<PluginsOption>("Plugins");
            var loader = new DefaultPluginLoader(options.AssemblyNames??new string[0], logger);
            return services.AddPlugins(loader);
        }

        private static void AddPluginApplicationModel(this IServiceCollection services)
        {
            services.AddTransient<IApplicationModelProvider, PluginApplicationModelProvider>();
        }

        private static void AddPluginViewLocations(this IServiceCollection services, IPluginManager pluginManager)
        {
            services.Configure<RazorViewEngineOptions>(o => {
                o.ConfigurePluginsView(pluginManager);
            });
        }

        public static RazorViewEngineOptions ConfigurePluginsView( this RazorViewEngineOptions options, IPluginManager pluginManager)
        {
            options.ViewLocationExpanders.Insert(0, new PluginViewLocationExtender());
            options.FileProvider = new PluginCompositeFileProvider(pluginManager, options.FileProvider);
            return options;
        }
    }
}
