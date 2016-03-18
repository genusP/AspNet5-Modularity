using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Genus.AspNetCore.Modularity.Tests.Stubs
{
    class ModuleStub : IPlugin
    {
        public string Name
        {
            get
            {
                return typeof(ModuleStub).Name;
            }
        }

        public bool ConfigureServicesCalled { get; private set; } = false;

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            ConfigureServicesCalled = true;
        }


        public bool ConfigureCalled { get; private set; } = false;

        public string UrlPrefix
        {
            get;
            set;
        }

        public void Configure(IApplicationBuilder appBuilder)
        {
            ConfigureCalled = true;
        }
    }
}
