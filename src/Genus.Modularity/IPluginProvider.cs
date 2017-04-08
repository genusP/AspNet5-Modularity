using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Genus.Modularity
{
    public interface IPluginProvider
    {
        IEnumerable<CandidateDescriptor> CandidatePlugins { get; }
    }
}
