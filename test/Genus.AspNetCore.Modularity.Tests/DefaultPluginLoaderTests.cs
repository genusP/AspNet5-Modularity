using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Genus.AspNetCore.Modularity.Tests.Stubs;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Genus.AspNetCore.Modularity.Tests
{
    public class DefaultPluginLoaderTests
    {
        

        ILogger logger = new Mock<ILogger>().Object;

        [Fact]
        public void ctor_notNul_argument()
        {
            var optionsMock = new Mock<IOptions<PluginsOption>>();

            //act
            Action act = () => new DefaultPluginLoader(new Mock<ILogger>().Object);

            //assert
            Assert.Throws<ArgumentNullException>("logger", act);
        }

        [Fact]
        public void LoadModules()
        {
            var optionsMock = new Mock<IOptions<PluginsOption>>();

            var target = new DefaultPluginLoader(logger);

            var result = target.LoadPlugins();

            Assert.Equal(1, result.Count());
            Assert.IsType<ModuleStub>(result.First());
        }
    }
}
