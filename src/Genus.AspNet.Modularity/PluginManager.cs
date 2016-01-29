using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using System.Reflection;
using Microsoft.AspNet.Builder;

namespace Genus.AspNet.Modularity
{
    public sealed class PluginManager : IPluginManager
    {
        private readonly IPluginLoader _loader;
        private IList<PluginInfo> pluginInfoList;

        public PluginManager(IPluginLoader loader)
        {
            if (loader == null)
                throw new ArgumentNullException(nameof(loader));
            _loader = loader;
        }

        public IEnumerable<PluginInfo> LoadedPlugins
        {
            get
            {
                if (pluginInfoList == null)
                    return Enumerable.Empty<PluginInfo>();
                return pluginInfoList.Select(p => p);
            }
        }

        public PluginInfo this[TypeInfo type]
        {
            get
            {
                return pluginInfoList.SingleOrDefault(t => t.Assembly == type.Assembly);
            }
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            if (this.pluginInfoList != null)
                throw new InvalidOperationException("Alredy inizialized");
            pluginInfoList = new List<PluginInfo>();
            foreach (var item in _loader.LoadPlugins())
            {
                var pluginTypeInfo = item.GetType().GetTypeInfo();
                var pi = new PluginInfo(item, GetPluginDescription(item), GetPluginRoot(item), pluginTypeInfo.Assembly);
                pluginInfoList.Add(pi);
                item.ConfigureServices(serviceCollection);
            }
        }

        private string GetPluginDescription(IPlugin plugin)
        {
            return (plugin as IPluginWithDescription)?.Description;
        }

        private string GetPluginRoot(IPlugin item)
        {
            var library = PlatformServices.Default.LibraryManager.GetLibrary(item.GetType().GetTypeInfo().Assembly.GetName().Name);
            var path = library.Path;
            if (library.Type == "Project")
                path = System.IO.Path.GetDirectoryName(path);
            return path;
            //var asmUri = new Uri(item.GetType().Assembly.CodeBase);
            //var asmPath = asmUri.LocalPath;
            //return System.IO.Path.GetDirectoryName(asmPath);
        }

        public void Configure(IApplicationBuilder applicationBuilder)
        {
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));
            if (pluginInfoList == null)
                throw new InvalidOperationException("Call ConfigureServices before");
            foreach (var pluginInfo in pluginInfoList)
            {
                pluginInfo.Plugin.Configure(applicationBuilder);
            }
        }
    }
}
