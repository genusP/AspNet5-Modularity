using System;
using System.Linq;
using System.Reflection;
using Genus.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genus.AspNetCore.Modularity.Internal
{
    internal class StartupWrapper : IStartup
    {
        public StartupWrapper(IStartup startup, IPluginManager pluginManager, IConfiguration configuration)
        {
            Startup =       startup       ?? throw new ArgumentNullException(nameof(startup));
            PluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));

            SetConfiguration( configuration ?? GetConfigurationFromStartup());
        }

        IStartup Startup { get; }
        IPluginManager PluginManager { get; }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            var modularityAppBuilder = new ModularityApplicationBuilder(applicationBuilder);
            Startup.Configure(modularityAppBuilder);

            var applicationPartManager = applicationBuilder.ApplicationServices.GetService<ApplicationPartManager>();
            foreach (var pluginInfo in PluginManager.LoadedPlugins)
            {
                var plugin = pluginInfo.Plugin as IAspNetCorePlugin;
                plugin?.Configure(modularityAppBuilder);
                if (applicationPartManager != null)
                    applicationPartManager.ApplicationParts.Add(new AssemblyPart(pluginInfo.Assembly));
            }


            modularityAppBuilder.Merge(applicationBuilder);
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Replace(new ServiceDescriptor(typeof(IPluginManager), PluginManager));
            Startup.ConfigureServices(services);

            PluginManager.ConfigureServices(services);

            return services.BuildServiceProvider();
        }

        private IConfiguration GetConfigurationFromStartup()
        {
            object startup = Startup;
            var istartupType = Startup.GetType().GetTypeInfo();
            if (istartupType.Name == "ConventionBasedStartup")
            {
                var fieldInfo = istartupType.GetDeclaredField("_methods");
                var methods = fieldInfo.GetValue(Startup);
                var configurePI = methods.GetType().GetProperty("ConfigureDelegate");
                var configureDelegate = (Action<IApplicationBuilder>)configurePI.GetValue(methods);
                startup = configureDelegate.Target.GetType().GetField("instance").GetValue(configureDelegate.Target);
            }
            var type = startup.GetType().GetTypeInfo();
            object configuration = null;
            var confProperty = type.GetDeclaredProperty("Configuration") ?? type.GetDeclaredProperty("configuration");
            if (confProperty != null)
                configuration = confProperty.GetValue(startup);
            else
            {
                var confField = type.GetDeclaredField("Configuration") ?? type.GetDeclaredField("configuration");
                if (confField != null)
                    configuration = confField.GetValue(startup);
            }
            return configuration as IConfiguration;
        }

        private void SetConfiguration(IConfiguration configuration)
        {
            foreach (var item in PluginManager.LoadedPlugins)
            {
                if(item.Plugin is IPluginWithConfiguration plugin)
                plugin.Configuration = configuration;
            }
        }
    }
}
