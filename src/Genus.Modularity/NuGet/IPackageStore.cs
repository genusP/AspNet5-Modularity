using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Modularity.NuGet
{
    public interface IPackageStore
    {
        IEnumerable<PackageCandidateItem> GetCandidates(AssemblyName assemblyName);
    }
}
