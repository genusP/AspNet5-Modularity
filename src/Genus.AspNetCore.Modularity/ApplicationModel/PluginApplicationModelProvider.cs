using Genus.Modularity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.AspNetCore.Modularity.ApplicationModel
{
    public class PluginApplicationModelProvider : IApplicationModelProvider
    {
        readonly IPluginManager _pluginManager;
        public PluginApplicationModelProvider(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));
        }
        public int Order => int.MaxValue;

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            foreach (var item in context.Result.Controllers)
            {
                var pluginInfo = _pluginManager[item.ControllerType];
                if (pluginInfo != null && pluginInfo.Plugin is IAspNetCorePlugin plugin)
                    item.RouteValues.Add("plugin",plugin.UrlPrefix);
            }   
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }
    }
}
