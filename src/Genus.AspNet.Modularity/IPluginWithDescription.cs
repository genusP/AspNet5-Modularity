using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.AspNet.Modularity
{
    public interface IPluginWithDescription
    {
        string Description { get; }
    }
}
