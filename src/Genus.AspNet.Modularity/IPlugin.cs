using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Builder;

namespace Genus.AspNet.Modularity
{
    public interface IPlugin
    {
        string Name { get; }
        //string Url { get; }
        void ConfigureServices(IServiceCollection serviceCollection);

        void Configure(IApplicationBuilder appBuilder);
    }
}
