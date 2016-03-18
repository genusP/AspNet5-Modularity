﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Genus.AspNetCore.Modularity
{
    public interface IPlugin
    {
        string Name { get; }

        string UrlPrefix { get; }

        void ConfigureServices(IServiceCollection serviceCollection);

        void Configure(IApplicationBuilder appBuilder);
    }
}
