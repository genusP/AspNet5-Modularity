using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Genus.AspNetCore.Modularity.ApplicationModel
{
    public class PluginRouteValueProvider : IRouteValueProvider
    {
        public PluginRouteValueProvider(string name)
        {
            RouteValue = name;
            RouteKey = "plugin";
            BlockNonAttributedActions = false;
        }

        public bool BlockNonAttributedActions
        {
            get;
        }

        public string RouteKey
        {
            get;
        }

        public string RouteValue
        {
            get;
        }
    }
}
