using System;
using System.Linq;
using Genus.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genus.AspNetCore.Modularity
{
    class StartupWrapper : IStartup
    {
        public StartupWrapper(IStartup startup, PluginManager pluginManager)
        {
            Startup = startup ?? throw new ArgumentNullException(nameof(startup));
            PluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));
        }

        IStartup Startup { get; }
        PluginManager PluginManager { get; }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            Startup.Configure(applicationBuilder);

            if (!PluginManager.LoadedPlugins.Any())
                throw new InvalidOperationException("Call ConfigureServices before");
            foreach (var pluginInfo in PluginManager.LoadedPlugins)
            {
                var plugin = pluginInfo.Plugin as IAspNetCorePlugin;
                plugin?.Configure(applicationBuilder);
            }
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Replace(new ServiceDescriptor(typeof(PluginManager), PluginManager));
            Startup.ConfigureServices(services);

            PluginManager.ConfigureServices(services);

            return services.BuildServiceProvider();
        }
    }
}
