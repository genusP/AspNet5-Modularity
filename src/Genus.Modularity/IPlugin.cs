using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Genus.Modularity
{
    public interface IPlugin
    {
        string Name { get; }

        void ConfigureServices(IServiceCollection serviceCollection);

        //void Configure(IApplicationBuilder appBuilder);
    }
}
