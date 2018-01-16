using Genus.Modularity;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genus.AspNetCore.Modularity
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder MapPluginsRoutes(
            this IRouteBuilder builder, 
            IPluginManager pluginManager, 
            IServiceProvider services, 
            Func<IPlugin, bool> predicate = null)
        {
            foreach (var pluginDescr in pluginManager.LoadedPlugins)
            {
                if (pluginDescr.Plugin is IRoutingPlugin routerPlugin)
                        if( predicate == null || predicate(pluginDescr.Plugin))
                            routerPlugin.MapRoute(builder, services);
            }
            return builder;
        }
    }
}
