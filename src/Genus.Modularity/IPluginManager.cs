using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Genus.Modularity
{
    public interface IPluginManager
    {
        IEnumerable<PluginDescriptor> LoadedPlugins { get; }

        void ConfigureServices(IServiceCollection serviceCollection);

        PluginDescriptor this[TypeInfo type]{ get; }
    }
}