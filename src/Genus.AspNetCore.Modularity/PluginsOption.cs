using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Genus.AspNetCore.Modularity
{
    public class PluginsOption
    {
        public PluginsOption()
        {
            //Folder = "Plugins";
        }
        //public string Folder { get; set; }

        public string[] AssemblyNames { get; set; }
    }
}
