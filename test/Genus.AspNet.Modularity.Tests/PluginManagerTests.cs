using Genus.AspNet.Modularity.Tests.Stubs;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genus.AspNet.Modularity.Tests
{
    public class PluginManagerTests
    {
        [Fact]
        public void ctor_argumnet_not_null()
        {
            Action act = ()=> new PluginManager(null);

            Assert.Throws<ArgumentNullException>("loader", act);
        }

        [Fact]
        public void LoadedPlugins_ReturnEmpty_IfNotCallLoad()
        {
            var loaderMock = new Mock<IPluginLoader>();
            var target = new PluginManager(loaderMock.Object);

            var result = target.LoadedPlugins;

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ConfigureServices_need_CallLoadFromLoader_and_Initalize()
        {
            var plugin = new ModuleStub();

            var loaderMock = new Mock<IPluginLoader>();
            loaderMock.Setup(l => l.LoadPlugins()).Returns(new[] { plugin });

            var serviceCollection = new ServiceCollection();
            var target = new PluginManager(loaderMock.Object);

            target.ConfigureServices(serviceCollection);

            loaderMock.Verify(l => l.LoadPlugins(), Times.Once);
            Assert.True(plugin.ConfigureServicesCalled, "CofigureServices not called");
        }

        [Fact]
        public void ConfigureServices_throw_many_initialize()
        {
            var loaderMock = new Mock<IPluginLoader>();
            loaderMock.Setup(l => l.LoadPlugins()).Returns(new IPlugin[0]);

            var serviceCollection = new ServiceCollection();
            var target = new PluginManager(loaderMock.Object);

            target.ConfigureServices(serviceCollection);
            Action init2 = ()=> target.ConfigureServices(serviceCollection);

            Assert.Throws<InvalidOperationException>(init2);
        }

        [Fact]
        public void Configure_throw_argument_null()
        {
            var loaderMock = new Mock<IPluginLoader>();
            var target = new PluginManager(loaderMock.Object);

            Action init2 = () =>target.Configure(null);

            Assert.Throws<ArgumentNullException>("applicationBuilder", init2);
        }

        [Fact]
        public void Configure_need_ConfigureServicesBefore()
        {
            var loaderMock = new Mock<IPluginLoader>();
            var target = new PluginManager(loaderMock.Object);

            Action init2 = () =>target.Configure(new Mock<IApplicationBuilder>().Object);

            Assert.Throws<InvalidOperationException>(init2);
        }

        [Fact]
        public void Configure_need_CallConfigureInPlugin()
        {
            var plugin = new ModuleStub();

            var loaderMock = new Mock<IPluginLoader>();
            loaderMock.Setup(l => l.LoadPlugins()).Returns(new[] { plugin });

            var serviceCollection = new ServiceCollection();
            var target = new PluginManager(loaderMock.Object);

            target.ConfigureServices(serviceCollection);
            target.Configure(new Mock<IApplicationBuilder>().Object);

            Assert.True(plugin.ConfigureCalled, "Cofigure not called");
        }
    }
}
