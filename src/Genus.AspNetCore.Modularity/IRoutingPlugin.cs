using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genus.AspNetCore.Modularity
{
    public interface IRoutingPlugin
    {
        void MapRoute(IRouteBuilder builder, IServiceProvider services);
    }
}
