using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Reflection;

namespace Genus.Modularity.Tests
{
    public class DirectoryPluginProviderTests
    {
        
        [Fact]
        public void LoadModules()
        {
            var pluginDir = System.IO.Directory.GetCurrentDirectory();
            var pluginName = GetType().GetTypeInfo().Assembly.GetName().Name;
            var target = new DirectoryPluginStore<IPlugin>( pluginDir, pluginName);

            var result = target.CandidatePlugins;

            Assert.Equal(1, result.Count());
            var candidateDescriptor = result.First();
            Assert.Equal(pluginName, candidateDescriptor.PluginName);
            Assert.Equal(pluginDir, candidateDescriptor.ContentRoot);
            Assert.NotEmpty(candidateDescriptor.AssemblyPath);
        }
    }
}
