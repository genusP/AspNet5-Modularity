using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.AspNetCore.Modularity
{
    public interface IPluginWithDescription
    {
        string Description { get; }
    }
}
