using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Genus.AspNet.Modularity.Tests.Stubs;

namespace Genus.AspNet.Modularity.Tests
{
    public class DefaultPluginLoaderTests
    {
        

        ILogger logger = new Mock<ILogger>().Object;

        [Fact]
        public void ctor_notNul_argument()
        {
            var optionsMock = new Mock<IOptions<PluginsOption>>();

            //act
            Action act = () => new DefaultPluginLoader(null, null);
            Action act2 = () => new DefaultPluginLoader(new[] { "test" }, null);

            //assert
            Assert.Throws<ArgumentNullException>("assemblyNames", act);
            Assert.Throws<ArgumentNullException>("logger", act2);
        }

        [Fact]
        public void LoadModules()
        {
            var optionsMock = new Mock<IOptions<PluginsOption>>();
            var assemblyName = System.Reflection.MethodInfo.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            var target = new DefaultPluginLoader(new[] { assemblyName }, logger);

            var result = target.LoadPlugins();

            Assert.Equal(1, result.Count());
            Assert.IsType<ModuleStub>(result.First());
        }
    }
}
