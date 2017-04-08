using System;
using System.Linq;
using System.Reflection;
using Genus.AspNetCore.Modularity.ApplicationModel;
using Genus.AspNetCore.Modularity.Tests.Stubs;
using Genus.Modularity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Moq;
using Xunit;

namespace Genus.AspNetCore.Modularity.Tests
{
    public class PluginApplicationModelProviderTests
    {
        public PluginApplicationModelProviderTests()
        {
        }

        [Fact]
        public void ctor_throw_for_null_argument()
        {
            Action act = () => new PluginApplicationModelProvider(null);

            Assert.Throws<ArgumentNullException>("pluginManager", act);
        }

        [Fact]
        public void OnProvidersExecuted_SetRouteConstaraintIfControllerInPlugin()
        {
            var pluginName = "testP";
            var typeInfo1 = typeof(PluginDescriptor).GetTypeInfo();
            var typeInfo2 = typeof(PluginManager).GetTypeInfo();
            var pluginManagerMock = new Mock<IPluginManager>();
            pluginManagerMock.Setup(pm => pm[It.Is<TypeInfo>(ti=>ti==typeInfo1)])
                .Returns(new PluginDescriptor(new AspNetCoreModuleStub { UrlPrefix = pluginName }, null, null, null));
            var target = new PluginApplicationModelProvider(pluginManagerMock.Object);
            var context = new ApplicationModelProviderContext(new[] { typeInfo1, typeInfo2 });
            context.Result.Controllers.Add( new ControllerModel(typeInfo1, new object[0]));
            context.Result.Controllers.Add( new ControllerModel(typeInfo2, new object[0]));

            target.OnProvidersExecuted(context);

            Assert.Equal(2, context.Result.Controllers.Count);
            var controllerInPlugin = context.Result.Controllers.Single(cm => cm.ControllerType == typeInfo1);
            Assert.Equal(1, controllerInPlugin.RouteValues.Count);
            Assert.Equal(pluginName, controllerInPlugin.RouteValues["plugin"]);

            var controllerNotInPlugin = context.Result.Controllers.Single(cm => cm.ControllerType == typeInfo2);
            Assert.Equal(0, controllerNotInPlugin.RouteValues.Count);
        }
    }
}
