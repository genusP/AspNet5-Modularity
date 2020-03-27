using System;
using System.IO;
using Genus.Modularity;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Genus.AspNetCore.Modularity
{
    public class PluginFileProvider : IFileProvider
    {
        private readonly IFileProvider _fileProvider;
        private readonly string _pathPrefix;
        public PluginFileProvider(PluginDescriptor pluginDescriptor, string physSubFolder, string virtPrefix)
        {
            string path = pluginDescriptor.PluginRoot;
            if (!string.IsNullOrEmpty(physSubFolder))
                path = Path.Combine(path, physSubFolder);
            if (Directory.Exists(path) && pluginDescriptor.Plugin is IAspNetCorePlugin plugin)
            {
                _fileProvider = new PhysicalFileProvider(path);
                _pathPrefix = virtPrefix;
            }
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var res = ExecuteIfValidPath(subpath, p => _fileProvider.GetDirectoryContents(p));
            return res ?? new NotFoundDirectoryContents();
        }

        private TRet ExecuteIfValidPath<TRet>(string subpath, Func<string, TRet> action) where TRet: class
        {
            var prefix = _pathPrefix ?? "";
            if (subpath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                var relativePath = subpath.Substring(prefix.Length);
                return action(relativePath);
            }
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var res = ExecuteIfValidPath(subpath, p => _fileProvider.GetFileInfo(p));
            return res ?? new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            var res = ExecuteIfValidPath(filter, p => _fileProvider.Watch(p));
            return res ?? NullChangeToken.Singleton;
        }
    }
}
