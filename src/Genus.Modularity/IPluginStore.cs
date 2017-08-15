using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Genus.Modularity
{
    public interface IPluginStore
    {
        IEnumerable<CandidateDescriptor> CandidatePlugins { get; }
        IPluginLoader PluginLoader { get; }
    }
}
