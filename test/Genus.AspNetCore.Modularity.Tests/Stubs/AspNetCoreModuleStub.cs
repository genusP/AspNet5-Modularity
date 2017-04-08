using Genus.AspNetCore.Modularity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.AspNetCore.Modularity.Tests.Stubs
{
    public class AspNetCoreModuleStub : Genus.Modularity.Tests.Stubs.ModuleStub, IAspNetCorePlugin
    {
        public bool ConfigureCalled { get; private set; } = false;

        public string UrlPrefix { get; set; }

        public void Configure(IApplicationBuilder builder)
        {
            ConfigureCalled = true;
        }
    }
}
