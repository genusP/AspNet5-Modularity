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
            Action act = ()=> new PluginManager(null, new Mock<IPluginLoader>().Object);
            Action act2 = ()=> new PluginManager(new Mock<IPluginProvider>().Object, null);

            Assert.Throws<ArgumentNullException>("provider", act);
            Assert.Throws<ArgumentNullException>("loader", act2);
        }

        [Fact]
        public void LoadedPlugins_ReturnEmpty_IfNotCallLoad()
        {
            var providerMock = new Mock<IPluginProvider>();
            var loaderMock = new Mock<IPluginLoader>();
            var target = new PluginManager(providerMock.Object, loaderMock.Object);

            var result = target.LoadedPlugins;

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void ConfigureServices_need_CallLoadFromLoader_and_Initalize()
        {
            var plugin = new ModuleStub();

            var providerMock = new Mock<IPluginProvider>();
            providerMock.Setup(p => p.CandidatePlugins).Returns(new[] { new CandidateDescriptor("test", "test", "test") });
            var loaderMock = new Mock<IPluginLoader>();
            loaderMock.Setup(l => l.LoadPlugin(It.IsAny<CandidateDescriptor>(), null))
                      .Returns(new PluginDescriptor(plugin, null, null, null));

            var serviceCollection = new ServiceCollection();
            var target = new PluginManager(providerMock.Object, loaderMock.Object);

            target.ConfigureServices(serviceCollection);

            loaderMock.Verify(l => l.LoadPlugin(It.IsAny<CandidateDescriptor>(), null), Times.Once);
            Assert.True(plugin.ConfigureServicesCalled, "CofigureServices not called");
        }

        [Fact]
        public void ConfigureServices_throw_many_initialize()
        {
            var providerMock = new Mock<IPluginProvider>();
            var loaderMock = new Mock<IPluginLoader>();
            providerMock.Setup(l => l.CandidatePlugins).Returns(new CandidateDescriptor[0]);
            loaderMock.Setup(l => l.LoadPlugin(It.IsAny<CandidateDescriptor>(), null)).Returns((PluginDescriptor)null);

            var serviceCollection = new ServiceCollection();
            var target = new PluginManager(providerMock.Object ,loaderMock.Object);

            target.ConfigureServices(serviceCollection);
            Action init2 = ()=> target.ConfigureServices(serviceCollection);

            Assert.Throws<InvalidOperationException>(init2);
        }
    }
}
