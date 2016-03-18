using Genus.AspNetCore.Modularity.ApplicationModel;
using Genus.AspNetCore.Modularity.Tests.Stubs;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            var typeInfo1 = typeof(PluginInfo).GetTypeInfo();
            var typeInfo2 = typeof(PluginManager).GetTypeInfo();
            var pluginManagerMock = new Mock<IPluginManager>();
            pluginManagerMock.Setup(pm => pm[It.Is<TypeInfo>(ti=>ti==typeInfo1)])
                .Returns(new PluginInfo(new ModuleStub { UrlPrefix = pluginName }, null, null, null));
            var target = new PluginApplicationModelProvider(pluginManagerMock.Object);
            var context = new ApplicationModelProviderContext(new[] { typeInfo1, typeInfo2 });
            context.Result.Controllers.Add( new ControllerModel(typeInfo1, new object[0]));
            context.Result.Controllers.Add( new ControllerModel(typeInfo2, new object[0]));

            target.OnProvidersExecuted(context);

            Assert.Equal(2, context.Result.Controllers.Count);
            var controllerInPlugin = context.Result.Controllers.Single(cm => cm.ControllerType == typeInfo1);
            Assert.Equal(1, controllerInPlugin.RouteConstraints.Count);
            Assert.Equal(pluginName, controllerInPlugin.RouteConstraints[0].RouteValue);
            Assert.Equal("plugin", controllerInPlugin.RouteConstraints[0].RouteKey);

            var controllerNotInPlugin = context.Result.Controllers.Single(cm => cm.ControllerType == typeInfo2);
            Assert.Equal(0, controllerNotInPlugin.RouteConstraints.Count);
        }
    }
}
