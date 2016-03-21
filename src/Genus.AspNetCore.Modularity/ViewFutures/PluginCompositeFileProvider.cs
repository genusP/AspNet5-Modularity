using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Genus.AspNetCore.Modularity.ViewFutures
{
    public class PluginCompositeFileProvider : IFileProvider
    {
        //TODO:Временно добавлены исходники в каталог решения, при выходе новой версии удалить исходники
        private readonly CompositeFileProvider _fileProvider;
        public PluginCompositeFileProvider(IPluginManager pluginManager, IFileProvider defaultFileProvider, string subfolder=null)
        {
            var pluginsFileProviders = new List<IFileProvider>(pluginManager.LoadedPlugins.Count()+1);
            pluginsFileProviders.Add(defaultFileProvider);
            pluginsFileProviders.AddRange(GetPluginFileProviders(pluginManager, subfolder));
            _fileProvider = new CompositeFileProvider(pluginsFileProviders);
        }

        private IEnumerable<IFileProvider> GetPluginFileProviders(IPluginManager pluginManager, string subfolder)
        {
            foreach (var pluginInfo in pluginManager.LoadedPlugins)
            {
                yield return new PluginFileProvider(pluginInfo, subfolder, null);
            }
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return _fileProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            return _fileProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            return _fileProvider.Watch(filter);
        }
    }
}
