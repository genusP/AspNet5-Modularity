using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Genus.AspNet.Modularity
{
    public interface IPluginLoader
    {
        IEnumerable<IPlugin> LoadPlugins();
    }
}
