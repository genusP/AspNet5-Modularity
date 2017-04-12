using System;
using System.Linq;
using System.Reflection;
using Genus.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genus.AspNetCore.Modularity
{
    class StartupWrapper : IStartup
    {
        public StartupWrapper(IStartup startup, IPluginManager pluginManager, IConfiguration configuration)
        {
            Startup =       startup       ?? throw new ArgumentNullException(nameof(startup));
            PluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));
            Configuration = configuration ?? GetConfigurationFromStartup();

            SetConfiguration();
        }

        IStartup Startup { get; }
        IPluginManager PluginManager { get; }
        IConfiguration Configuration { get; }

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

        private IConfiguration GetConfigurationFromStartup()
        {
            var type = Startup.GetType().GetTypeInfo();
            object configuration = null;
            var confProperty = type.GetDeclaredProperty("Configuration") ?? type.GetDeclaredProperty("configuration");
            if (confProperty != null)
                configuration = confProperty.GetValue(Startup);
            else
            {
                var confField = type.GetDeclaredField("Configuration") ?? type.GetDeclaredField("configuration");
                if (confField != null)
                    configuration = confField.GetValue(Startup);
            }
            return configuration as IConfiguration;
        }

        private void SetConfiguration()
        {
            foreach (var item in PluginManager.LoadedPlugins.OfType<IPluginWithConfiguration>())
            {
                item.Configuration = Configuration;
            }
        }
    }
}
