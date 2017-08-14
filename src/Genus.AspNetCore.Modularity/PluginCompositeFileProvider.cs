using System.Collections.Generic;
using System.Linq;
using Genus.Modularity;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Genus.AspNetCore.Modularity
{
    public class PluginCompositeFileProvider : IFileProvider
    {
        private readonly CompositeFileProvider _fileProvider;
        public PluginCompositeFileProvider(IPluginManager pluginManager, IFileProvider defaultFileProvider, string subfolder = null)
        {
            var pluginsFileProviders = new List<IFileProvider>(pluginManager.LoadedPlugins.Count() + 1);
            pluginsFileProviders.Add(defaultFileProvider);
            pluginsFileProviders.AddRange(GetPluginFileProviders(pluginManager, subfolder));
            _fileProvider = new CompositeFileProvider(pluginsFileProviders);
        }

        private IEnumerable<IFileProvider> GetPluginFileProviders(IPluginManager pluginManager, string subfolder)
            => pluginManager.LoadedPlugins
                    .Select(pluginInfo => new PluginFileProvider(pluginInfo, subfolder, null));

        public IDirectoryContents GetDirectoryContents(string subpath)
            => _fileProvider.GetDirectoryContents(subpath);

        public IFileInfo GetFileInfo(string subpath)
            => _fileProvider.GetFileInfo(subpath);

        public IChangeToken Watch(string filter)
            => _fileProvider.Watch(filter);
    }
}
