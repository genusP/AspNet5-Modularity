using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Modularity.NuGet
{
    public struct PackageCandidateItem
    {
        public PackageCandidateItem(PackageDescriptor package, int priority)
        {
            Package = package ?? throw new ArgumentNullException(nameof(package));
            Priority = priority;
        }
        public PackageDescriptor Package { get; }
        public int Priority { get; set; }
    }
}
