using System;
using System.Linq;
using Genus.Modularity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Genus.AspNetCore.Modularity
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UsePluginManager(this IWebHostBuilder hostBuilder, Func<IServiceProvider, IPluginProvider> pluginProviderFactory)
            => hostBuilder.UsePluginManager<AssemblyPluginLoader<IPlugin>>(pluginProviderFactory);

        public static IWebHostBuilder UsePluginManager<TLoader>(this IWebHostBuilder hostBuilder, Func<IServiceProvider, IPluginProvider> pluginProviderFactory)
            where TLoader : IPluginLoader, new()
            => hostBuilder.UsePluginManager( sp => new PluginManager(pluginProviderFactory(sp), new TLoader(), sp.GetRequiredService<ILogger<PluginManager>>()));

        public static IWebHostBuilder UsePluginManager(this IWebHostBuilder hostBuilder, Func<IServiceProvider,PluginManager> pluginManagerFactory, 
                                                            IConfiguration configuration = null)
        {
            var startupAssemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            return hostBuilder.UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName)
                              .ConfigureServices(services =>
                              {
                                  var startup = services.FirstOrDefault(_ => _.ServiceType == typeof(IStartup));
                                  if (startup ==null)
                                      throw new InvalidOperationException("Call UseStartup before this method");

                                  Func<IServiceProvider, IStartup> factory = sp =>
                                  {
                                      var baseStartup = startup.ImplementationInstance ?? startup.ImplementationFactory(sp);
                                      return new StartupWrapper((IStartup)baseStartup, sp.GetRequiredService<PluginManager>(), configuration);
                                  };

                                  var serviceDescriptor = new ServiceDescriptor(startup.ServiceType, factory, ServiceLifetime.Singleton);
                                  services.Replace(serviceDescriptor);

                                  services.AddSingleton(pluginManagerFactory);
                              });
        }
    }
}
