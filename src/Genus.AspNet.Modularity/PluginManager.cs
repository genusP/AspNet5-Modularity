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
        private IList<Tuple<Assembly, PluginInfo, IPlugin>> plugins;

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
                if (plugins == null)
                    return Enumerable.Empty<PluginInfo>();
                return plugins.Select(p => p.Item2);
            }
        }

        public PluginInfo this[TypeInfo type]
        {
            get
            {
                return plugins.SingleOrDefault(t => t.Item1 == type.Assembly)?.Item2;
            }
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            if (this.plugins != null)
                throw new InvalidOperationException("Alredy inizialized");
            plugins = new List<Tuple<Assembly, PluginInfo, IPlugin>>();
            foreach (var item in _loader.LoadPlugins())
            {
                var pi = new PluginInfo(item.Name, null, GetPluginRoot(item));
                plugins.Add(new Tuple<Assembly, PluginInfo, IPlugin>(item.GetType().GetTypeInfo().Assembly, pi, item));
                item.ConfigureServices(serviceCollection);
            }
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
            if (plugins == null)
                throw new InvalidOperationException("Call ConfigureServices before");
            foreach (var plugin in plugins)
            {
                plugin.Item3.Configure(applicationBuilder);
            }
        }
    }
}
