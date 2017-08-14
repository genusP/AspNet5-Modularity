using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;

namespace Genus.Modularity.NuGet
{
    public class FolderPackageStrore : IPackageStore
    {
        readonly string _storePath;
        public FolderPackageStrore(string storePath)
        {
            if (!Directory.Exists(storePath))
                throw new DirectoryNotFoundException(storePath);
            _storePath = Path.GetFullPath(storePath);
        }

        public PackageDescriptor GetPackageDescriptor(AssemblyName assemblyName)
        {
            //skip if load System.IO.FileSystem
            if (assemblyName.Name.StartsWith( typeof(File).GetTypeInfo().Assembly.GetName().Name))
                return null;

            var packagePath = GetPackagePath(assemblyName);
            if (!string.IsNullOrEmpty(packagePath))
            {
                var libPath = Path.Combine(packagePath, "lib");
                var contentPath = GetContentPath(packagePath);
                var fileName = assemblyName.Name + ".dll";
                var assemblyPath = Path.Combine(libPath, fileName);
                if (!File.Exists(assemblyPath))
                {
                    assemblyPath = null;

                    string frameworkName = Assembly.GetEntryAssembly()
                                                .GetCustomAttributes<System.Runtime.Versioning.TargetFrameworkAttribute>()
                                                .Select(x => x.FrameworkName)
                                                .FirstOrDefault();
                    var currentFramework = NuGetFramework.ParseFrameworkName(frameworkName, new DefaultFrameworkNameProvider());

                    foreach (var libFrameworkPath in Directory.GetDirectories(libPath))
                    {
                        var ngFwName = Path.GetFileName(libFrameworkPath);
                        var packageFramework = NuGetFramework.ParseFolder(ngFwName);
                        if (packageFramework != null
                           && DefaultCompatibilityProvider.Instance.IsCompatible(currentFramework, packageFramework))
                        {
                            assemblyPath = Path.Combine(libFrameworkPath, fileName);
                            break;
                        }
                    }
                }
                return new PackageDescriptor
                {
                    AssemblyPath = assemblyPath,
                    ContentPath = contentPath
                };
            }
            return null;
        }

        private static string GetContentPath(string packagePath)
        {
            var contentPath = Path.Combine(packagePath, "content");
            if (Directory.Exists(contentPath))
                return contentPath;
            return null;
        }

        private string GetPackagePath(AssemblyName assemblyName)
        {
            var packagePath = Path.Combine(_storePath, assemblyName.Name);
            var packageVersions = GetPackageVersions(packagePath).OrderByDescending(_ => _.Value.Key);

            if (assemblyName.Version == null) //if no version info return max version
                return packageVersions.First().Value.Value;

            else
                return packageVersions.FirstOrDefault(v => IsCompatibilityVersion(assemblyName.Version, v.Value.Key))?.Value;
        }

        private IEnumerable<KeyValuePair<Version, string>?> GetPackageVersions(string packagePath)
        {
            foreach (var p in Directory.GetDirectories(packagePath))
            {
                if (Version.TryParse(Path.GetFileName(p), out var version))
                    yield return new KeyValuePair<Version, string>(version, p);
            }
        }

        private static bool IsCompatibilityVersion(Version v1, Version v2)
        {
            Func<int,int> defRevision = (a) => a == -1 ? 0 : a;
            return v1.Major == v2.Major && v1.Minor == v2.Minor 
                && (
                    v2.MajorRevision>v1.MajorRevision 
                    || (
                          defRevision(v2.MajorRevision) == defRevision(v1.MajorRevision) 
                       && defRevision(v2.MinorRevision) >= defRevision(v1.MinorRevision)
                       )
                    );
        }
    }
}
