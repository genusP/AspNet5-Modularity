using Genus.AspNetCore.Modularity.Tests.Stubs;
using Genus.AspNetCore.Modularity.ViewFutures;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Genus.AspNetCore.Modularity.Tests.ViewFutures
{
    public class PluginViewLocationExtenderTests
    {
        public static IEnumerable<object[]> TestDataWithExpected
        {
            get
            {
                yield return new object[]{
                    "testPlug", //plugin
                    true, //eists
                    new string[] { //viewLoctations
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml"
                    },
                    new string[] { //expected
                        //"/testPlug/Views/{1}/{0}.cshtml",
                        "/Views/testPlug/{1}/{0}.cshtml",
                        //"/testPlug/Views/Shared/{0}.cshtml",
                        "/Views/testPlug/Shared/{0}.cshtml",
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml"
                    }
                };yield return new object[]{
                    "PluginForTest", //plugin
                    true, //eists
                    new string[] { //viewLoctations
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml"
                    },
                    new string[] { //expected
                        //"/PluginForTest/Views/{1}/{0}.cshtml",
                        "/Views/PluginForTest/{1}/{0}.cshtml",
                        //"/PluginForTest/Views/Shared/{0}.cshtml",
                        "/Views/PluginForTest/Shared/{0}.cshtml",
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml"
                    }
                };
                yield return new object[]{
                    "PluginForTest", //plugin
                    false, //eists
                    new string[] { //viewLoctations
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml"
                    },
                    new string[] {
                        "/Views/{1}/{0}.cshtml",
                        "/Views/Shared/{0}.cshtml"
                    }
                };
            }
        }
        [Theory]
        [MemberData(nameof(TestDataWithExpected))]
        public void ExpandViewLocations_SpecificPlugin(
            string pluginName, 
            bool exists,
            IEnumerable<string> viewLocations,
            IEnumerable<string> expectedViewLocations)
        {
            var pluginManagerMock = new Mock<IPluginManager>();
            if(exists)
                pluginManagerMock.Setup(pm => pm[It.IsAny<TypeInfo>()])
                    .Returns(new PluginInfo(new ModuleStub { UrlPrefix = pluginName }, null, null, null));;

            var services = new ServiceCollection();
            services.Add(new ServiceDescriptor(typeof(IPluginManager), pluginManagerMock.Object));

            var target = new PluginViewLocationExtender();
            var actionContext = new ActionContext { HttpContext = new DefaultHttpContext { RequestServices = services.BuildServiceProvider() } };
            actionContext.ActionDescriptor = new ControllerActionDescriptor { ControllerTypeInfo = typeof(object).GetTypeInfo() };
            var context = new ViewLocationExpanderContext(
               actionContext,
               "testView",
               "test-controller",
               "",
               false);

            var result = target.ExpandViewLocations(context, viewLocations);

            Assert.Equal(expectedViewLocations, result);
        }
    }
}
