using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Modularity.NuGet
{
    public class PackageDescriptor
    {
        public string AssemblyPath { get; set; }
        public string ContentPath { get; set; }
        public Version AssemblyVersion { get; set; }
    }
}
