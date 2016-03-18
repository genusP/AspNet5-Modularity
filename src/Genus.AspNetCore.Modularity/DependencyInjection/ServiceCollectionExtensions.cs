using Genus.AspNetCore.Modularity;
using Genus.AspNetCore.Modularity.ApplicationModel;
using Genus.AspNetCore.Modularity.ViewFutures;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Razor;
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

            return services.AddPlugins(
                new DefaultPluginLoader(logger));
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
            foreach (var pluginInfo in pluginManager.LoadedPlugins)
            {
                options.FileProviders.Add(new PluginFileProvider(pluginInfo, "Views", "Views"));
            }
            return options;
        }
    }
}
