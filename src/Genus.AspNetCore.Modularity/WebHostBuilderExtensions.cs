using System;
using System.Linq;
using Genus.AspNetCore.Modularity.Internal;
using Genus.Modularity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Genus.AspNetCore.Modularity
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UsePluginManager(this IWebHostBuilder hostBuilder, Func<IServiceProvider,IPluginManager> pluginManagerFactory, 
                                                            IConfiguration configuration = null)
        {
            var startupAssemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            return hostBuilder
                .UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName)
                .ConfigureServices(services =>
                {
                    var startupDescriptor = services.FirstOrDefault(_ => _.ServiceType == typeof(IStartup));
                    if (startupDescriptor ==null)
                        throw new InvalidOperationException("Call UseStartup before this method");
                    Func<IServiceProvider, IStartup> factory = sp =>
                    {
                        var baseStartup = startupDescriptor.ImplementationInstance ?? startupDescriptor.ImplementationFactory(sp);
                        return new StartupWrapper((IStartup)baseStartup, sp.GetRequiredService<IPluginManager>(), configuration);
                    };

                    var serviceDescriptor = new ServiceDescriptor(startupDescriptor.ServiceType, factory, ServiceLifetime.Singleton);
                    services.Replace(serviceDescriptor);

                    services.AddSingleton(pluginManagerFactory);
                });
        }
    }
}
