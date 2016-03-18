using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Genus.AspNetCore.Modularity.ApplicationModel
{
    public class PluginRouteConstraintProvider : IRouteConstraintProvider
    {
        public PluginRouteConstraintProvider(string name)
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

        public RouteKeyHandling RouteKeyHandling
        {
            get;
        }

        public string RouteValue
        {
            get;
        }
    }
}
