using Genus.AspNetCore.Modularity;
using Genus.AspNetCore.Modularity.ApplicationModel;
using Genus.AspNetCore.Modularity.ViewFutures;
using Genus.Modularity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        public static void AddPlugins(this IServiceCollection services, IPluginManager pluginManager)
        {
            services.AddPluginApplicationModel();
            services.AddPluginViewLocations(pluginManager);
        }

        public static void AddPluginApplicationModel(this IServiceCollection services)
        {
            services.AddTransient<IApplicationModelProvider, PluginApplicationModelProvider>();
        }

        private static void AddPluginViewLocations(this IServiceCollection services, IPluginManager pluginManager)
        {
            services.Configure<RazorViewEngineOptions>(o => o.ConfigurePluginsView(pluginManager));
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
