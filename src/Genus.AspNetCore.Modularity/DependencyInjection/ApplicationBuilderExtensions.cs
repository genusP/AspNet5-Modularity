using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.AspNetCore.Modularity.DependencyInjection
{
    public class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PluginsConfigure(IApplicationBuilder app)
        {
            var pm = app.ApplicationServices.GetRequiredService<IPluginManager>() as PluginManager;
            if (pm != null)
                pm.Configure(app);
            return app;
        }
    }
}
