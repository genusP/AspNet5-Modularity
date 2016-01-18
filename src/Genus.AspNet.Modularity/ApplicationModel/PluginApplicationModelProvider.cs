using Genus.AspNet.Modularity;
using Microsoft.AspNet.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.AspNet.Modularity.ApplicationModel
{
    public class PluginApplicationModelProvider : IApplicationModelProvider
    {
        readonly IPluginManager _pluginManager;
        public PluginApplicationModelProvider(IPluginManager pluginManager)
        {
            if(pluginManager==null)
                throw new ArgumentNullException(nameof(pluginManager));
            _pluginManager = pluginManager;
        }
        public int Order
        {
            get
            {
                return int.MaxValue;
            }
        }

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            foreach (var item in context.Result.Controllers)
            {
                var pluginInfo = _pluginManager[item.ControllerType];
                if (pluginInfo != null)
                    item.RouteConstraints.Add(new PluginRouteConstraintProvider(pluginInfo.Name));
            }   
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
        }
    }
}
