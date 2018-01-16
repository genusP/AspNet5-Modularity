using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Modularity.NuGet
{
    public class FolderPackageStrore : IPackageStore
    {
        struct CandidateItem {
            public CandidateItem(PackageDescriptor package, int priority)
            {
                Package = package ?? throw new ArgumentNullException(nameof(package));
                Priority = priority;
            }
            public PackageDescriptor Package { get; }
            public int Priority { get; set; }
        }

        readonly string _storePath;
        public FolderPackageStrore(string storePath)
        {
            new NuGetVersion("1"); //for preload assembly
            if (!Directory.Exists(storePath))
                throw new DirectoryNotFoundException(storePath);
            _storePath = Path.GetFullPath(storePath);
        }

        public PackageDescriptor GetPackageDescriptor(AssemblyName assemblyName)
        {
            //skip if load System.IO.FileSystem
            if (assemblyName.Name.StartsWith( typeof(File).GetTypeInfo().Assembly.GetName().Name))
                return null;

            var packagePath = Path.Combine(_storePath, assemblyName.Name);
            if (Directory.Exists(packagePath))
            {
                var packageVersions = GetPackageVersions(packagePath).OrderBy(_ => _.Item1);
                CandidateItem[] candidates = null;
                
                //if no version info return max version
                if (assemblyName.Version == null) 
                {
                    var path = packageVersions.First().Item2;
                    var pDescriptor = FindPackage(assemblyName, path);
                    candidates = new CandidateItem[] { new CandidateItem(pDescriptor, 0) };
                }
                else
                {
                    var asmVersion = new NuGetVersion(assemblyName.Version);
                    //if exist package with version full equals of assembly version
                    var eqPkgVersion = packageVersions.FirstOrDefault(pv => pv.Item1 == asmVersion);
                    if (eqPkgVersion != null)
                    {
                        var path = eqPkgVersion.Item2;
                        var pDescriptor = FindPackage(assemblyName, path);
                        candidates = new CandidateItem[] { new CandidateItem(pDescriptor, 0) };
                    }
                    else
                    {
                        var asmVersionWithoutRevision = new NuGetVersion(asmVersion.Major, asmVersion.Minor, asmVersion.Patch);
                        //find all packages
                        candidates = (from pv in packageVersions
                                      let path = pv.Item2
                                      where CompareVersion(pv.Item1, asmVersionWithoutRevision) >=0
                                      let pDescriptor = FindPackage(assemblyName, path)
                                      let priority = CompareVersion(new NuGetVersion(pDescriptor.AssemblyVersion), asmVersion)
                                      where priority >=0
                                      select new CandidateItem(
                                          pDescriptor,
                                          priority
                                          )
                            ).ToArray();
                    }
                }
                if (candidates.Length > 0)
                {
                    var candidate = candidates.OrderBy(_ => _.Priority)
                                              .ThenByDescending(_ => _.Package.AssemblyVersion)
                                              .First();
                    return candidate.Package;
                }
            }
            return null;
        }

        private PackageDescriptor FindPackage(AssemblyName assemblyName, string packagePath)
        {
            var libPath = Path.Combine(packagePath, "lib");
            var contentPath = GetContentPath(packagePath);
            var fileName = assemblyName.Name + ".dll";
            var assemblyPath = Path.Combine(libPath, fileName);
            if (!File.Exists(assemblyPath))
            {
                assemblyPath = null;

                foreach (var libFrameworkPath in Directory.GetDirectories(libPath))
                {
                    var ngFwName = Path.GetFileName(libFrameworkPath);
                    if (PlatformInformation.IsCompatibleFramework(ngFwName))
                    {
                        assemblyPath = Path.Combine(libFrameworkPath, fileName);
                        break;
                    }
                }
            }
            return new PackageDescriptor
            {
                AssemblyPath = assemblyPath,
                ContentPath = contentPath,
                AssemblyVersion = GetAssemblyVersion(assemblyPath)
            };
        }

#if !NET451
        private static Version GetAssemblyVersion(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                using (var fileStream = File.OpenRead(path))
                {
                    using (var peReader = new System.Reflection.PortableExecutable.PEReader(fileStream))
                    {
                        var mdMemoryBlock = peReader.GetMetadata();
                        unsafe
                        {
                            var metadataReader = new System.Reflection.Metadata.MetadataReader(
                                mdMemoryBlock.Pointer,
                                mdMemoryBlock.Length);

                            var asmDef = metadataReader.GetAssemblyDefinition();
                            return asmDef.Version;
                        }
                    }
                }
            }
            return null;
        }
#else
        private static Version GetAssemblyVersion(string path)
            => Assembly.ReflectionOnlyLoadFrom(path).GetName().Version;
#endif

        private static string GetContentPath(string packagePath)
        {
            var contentPath = Path.Combine(packagePath, "content");
            if (Directory.Exists(contentPath))
                return contentPath;
            return null;
        }

        private IEnumerable<Tuple<NuGetVersion, string>> GetPackageVersions(string packagePath)
        {
            foreach (var p in Directory.GetDirectories(packagePath))
            {
                if (NuGetVersion.TryParse(Path.GetFileName(p), out var version))
                    yield return new Tuple<NuGetVersion, string>(version, p);
            }
        }

        private int CompareVersion(NuGetVersion v1, NuGetVersion v2)
            => VersionComparer.Compare(v1, v2, VersionComparison.Version);
    }
}
