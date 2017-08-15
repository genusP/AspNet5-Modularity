using Genus.Modularity;
using Genus.Modularity.Tests.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Genus.Modularity.Tests
{
    public class PluginManagerTests
    {
        [Fact]
        public void Ctor_argumnet_not_null()
        {
            Action act = ()=> new PluginManager(null);

            Assert.Throws<ArgumentNullException>("store", act);
        }

        [Fact]
        public void LoadedPlugins_ReturnEmpty_IfNotCallLoad()
        {
            var providerMock = new Mock<IPluginStore>();
            var loaderMock = new Mock<IPluginLoader>();
            var target = new PluginManager(providerMock.Object);

            var result = target.LoadedPlugins;

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ConfigureServices_need_CallLoadFromLoader_and_Initalize()
        {
            var plugin = new ModuleStub();

            var loaderMock = new Mock<IPluginLoader>();
            loaderMock.Setup(l => l.LoadPlugin(It.IsAny<CandidateDescriptor>(), null))
                      .Returns(new PluginDescriptor(plugin, null, null, null));
            var providerMock = new Mock<IPluginStore>();
            providerMock.Setup(p => p.PluginLoader).Returns(loaderMock.Object);
            providerMock.Setup(p => p.CandidatePlugins).Returns(new[] { new CandidateDescriptor("test", "test", "test") });

            var serviceCollection = new ServiceCollection();
            var target = new PluginManager(providerMock.Object);

            target.ConfigureServices(serviceCollection);

            loaderMock.Verify(l => l.LoadPlugin(It.IsAny<CandidateDescriptor>(), null), Times.Once);
            Assert.True(plugin.ConfigureServicesCalled, "CofigureServices not called");
        }

        [Fact]
        public void ConfigureServices_throw_many_initialize()
        {
            var loaderMock = new Mock<IPluginLoader>();
            loaderMock.Setup(l => l.LoadPlugin(It.IsAny<CandidateDescriptor>(), null)).Returns((PluginDescriptor)null);
            var providerMock = new Mock<IPluginStore>();
            providerMock.Setup(l => l.CandidatePlugins).Returns(new CandidateDescriptor[0]);
            providerMock.Setup(p => p.PluginLoader).Returns(loaderMock.Object);

            var serviceCollection = new ServiceCollection();
            var target = new PluginManager(providerMock.Object);

            target.ConfigureServices(serviceCollection);
            Action init2 = ()=> target.ConfigureServices(serviceCollection);

            Assert.Throws<InvalidOperationException>(init2);
        }
    }
}
