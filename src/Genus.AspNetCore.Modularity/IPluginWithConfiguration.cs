using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Genus.AspNetCore.Modularity
{
    public interface IPluginWithConfiguration
    {
        IConfiguration Configuration { get; set; }
    }
}
