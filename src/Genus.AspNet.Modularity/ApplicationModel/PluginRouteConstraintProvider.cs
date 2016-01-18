using Microsoft.AspNet.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Routing;

namespace Genus.AspNet.Modularity.ApplicationModel
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
