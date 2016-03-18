using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Genus.AspNetCore.Modularity
{
    public interface IPluginLoader
    {
        IEnumerable<IPlugin> LoadPlugins();
    }
}
