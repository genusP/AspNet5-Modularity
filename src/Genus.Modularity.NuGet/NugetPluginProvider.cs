using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Genus.Modularity;

namespace Genus.Modularity.NuGet
{
    public class NugetPluginProvider:IPluginProvider
    {
        public NugetPluginProvider(string nugetRepository, string[] pluginNames)
        {
        }

        public IEnumerable<CandidateDescriptor> CandidatePlugins
        {
            get
            {
                //SourceProvider
                //var repositoryFactory = new global::NuGet.Repositories.
                throw new NotImplementedException();
            }
        }
    }
}
