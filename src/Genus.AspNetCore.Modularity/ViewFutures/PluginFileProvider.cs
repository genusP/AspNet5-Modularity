using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.IO;

namespace Genus.AspNetCore.Modularity.ViewFutures
{
    public class PluginFileProvider : IFileProvider
    {
        class NotFoundDirectoryContents : IDirectoryContents
        {
            public NotFoundDirectoryContents()
            {
            }

            public bool Exists
            {
                get { return false; }
            }

            public IEnumerator<IFileInfo> GetEnumerator()
            {
                return Enumerable.Empty<IFileInfo>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        class NotFoundFileInfo : IFileInfo
        {
            private readonly string _name;

            public NotFoundFileInfo(string name)
            {
                _name = name;
            }

            public bool Exists
            {
                get { return false; }
            }

            public bool IsDirectory
            {
                get { return false; }
            }

            public DateTimeOffset LastModified
            {
                get { return DateTimeOffset.MinValue; }
            }

            public long Length
            {
                get { return -1; }
            }

            public string Name
            {
                get { return _name; }
            }

            public string PhysicalPath
            {
                get { return null; }
            }

            public System.IO.Stream CreateReadStream()
            {
                throw new System.IO.FileNotFoundException(string.Format("The file {0} does not exist.", Name));
            }
        }

        class NoopChangeToken : IChangeToken
        {
            class EmptyDisposable : IDisposable
            {
                public static EmptyDisposable Instance { get; } = new EmptyDisposable();

                private EmptyDisposable()
                {
                }

                public void Dispose()
                {
                }
            }
            public static NoopChangeToken Singleton { get; } = new NoopChangeToken();

            private NoopChangeToken()
            {
            }

            public bool HasChanged => false;

            public bool ActiveChangeCallbacks => false;

            public IDisposable RegisterChangeCallback(Action<object> callback, object state)
            {
                return EmptyDisposable.Instance;
            }
        }

        private readonly IFileProvider _fileProvider;
        private readonly string _pathPrefix;
        public PluginFileProvider(PluginInfo plugin, string physSubFolder, string virtSubFolder)
        {
            string path = plugin.PluginRoot;
            if (!string.IsNullOrEmpty(physSubFolder))
                path = System.IO.Path.Combine(path, physSubFolder);
            if (Directory.Exists(path))
            {
                _fileProvider = new PhysicalFileProvider(path);
                _pathPrefix = "/" + plugin.Plugin.UrlPrefix + "/";
                if (!string.IsNullOrWhiteSpace(virtSubFolder))
                    _pathPrefix = "/" + virtSubFolder + _pathPrefix;
            }
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var res = ExecuteIfValidPath<IDirectoryContents>(subpath, p => _fileProvider.GetDirectoryContents(p));
            return res ?? new NotFoundDirectoryContents();
        }

        private TRet ExecuteIfValidPath<TRet>(string subpath, Func<string, TRet> action) where TRet: class
        {
            if (_fileProvider!=null && subpath.StartsWith(_pathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var relativePath = subpath.Substring(_pathPrefix.Length);
                return action(relativePath);
            }
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var res = ExecuteIfValidPath<IFileInfo>(subpath, p => _fileProvider.GetFileInfo(p));
            return res ?? new NotFoundFileInfo(subpath);
        }

        public IChangeToken Watch(string filter)
        {
            var res = ExecuteIfValidPath<IChangeToken>(filter, p => _fileProvider.Watch(p));
            return res ?? NoopChangeToken.Singleton;
        }
    }
}
