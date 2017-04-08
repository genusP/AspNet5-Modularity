using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.Modularity.Tests.Stubs
{
    public class ModuleStub : IPlugin
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
    }
}
