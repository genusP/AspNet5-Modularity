using System.Collections.Generic;
using Genus.AspNetCore.Modularity;
using Genus.Modularity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Genus.AspNetCore.Mvc.Modularity
{
    public class PluginViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var actionDescriptor = (ControllerActionDescriptor)context.ActionContext.ActionDescriptor;
            var pluginManager = context.ActionContext.HttpContext.RequestServices.GetRequiredService<IPluginManager>();
            var pluginInfo = pluginManager[actionDescriptor.ControllerTypeInfo];
            if (pluginInfo!=null && pluginInfo.Plugin is IAspNetCorePlugin plugin)
            {
                var prefix = plugin.UrlPrefix;
                yield return "/Views/" + prefix + "/{1}/{0}.cshtml";
                yield return "/Views/" + prefix + "/Shared/{0}.cshtml";
            }
            foreach(var viewLocation in viewLocations)
                yield return viewLocation;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            
        }
    }
}
