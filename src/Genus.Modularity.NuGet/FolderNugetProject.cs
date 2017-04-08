using NuGet.Packaging;
using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Genus.AspNetCore.Modularity.NuGet
{
    public class FolderNugetProject
    {
        public FolderNugetProject(/*string root, PackagePathResolver packagePathResolver*/)
        {
            //if (root != null)
            //    throw new ArgumentNullException(nameof(root));
            //Root = root;
            //PackagePathResolver = packagePathResolver;
        }

        //public string Root { get; set; }

        //private PackagePathResolver PackagePathResolver { get; }

        public Task/*<IEnumerable<PackageReference>>*/ GetInstalledPackagesAsync(CancellationToken token)
        {
            throw new NotImplementedException();
            //return Task.FromResult(Enumerable.Empty<PackageReference>());
        }

        public Task<bool> InstallPackageAsync(
            //PackageIdentity packageIdentity,
            //DownloadResourceResult downloadResourceResult,
            //INuGetProjectContext nuGetProjectContext,
            CancellationToken cancellationToken)

        {
            throw new NotImplementedException();
            //if (packageIdentity == null)
            //    throw new ArgumentNullException(nameof(packageIdentity));

            //if (downloadResourceResult == null)
            //    throw new ArgumentNullException(nameof(downloadResourceResult));

            //if (nuGetProjectContext == null)
            //    throw new ArgumentNullException(nameof(nuGetProjectContext));

            //if (!downloadResourceResult.PackageStream.CanSeek)
            //    throw new ArgumentException("Need seekable stream");

            //var packageDirectory = this.PackagePathResolver.GetInstallPath(packageIdentity);

            ////return ConcurrencyUtilities.ExecuteWithFileLockedAsync(
            ////    packageDirectory,
            ////    action: cancellationToken =>
            ////    {
            //// 1. Set a default package extraction context, if necessary.
            //PackageExtractionContext packageExtractionContext = nuGetProjectContext.PackageExtractionContext;
            //if (packageExtractionContext == null)
            //    packageExtractionContext = new PackageExtractionContext(new LoggerAdapter(nuGetProjectContext));

            //// 2. Check if the Package already exists at root, if so, return false

            //if (PackageExists(packageIdentity, packageExtractionContext.PackageSaveMode))
            //{
            //    //nuGetProjectContext.Log(MessageLevel.Info, Strings.PackageAlreadyExistsInFolder, packageIdentity, Root);
            //    return Task.FromResult(false);
            //}

            ////nuGetProjectContext.Log(MessageLevel.Info, Strings.AddingPackageToFolder, packageIdentity, Path.GetFullPath(Root));

            //// 3. Call PackageExtractor to extract the package into the root directory of this FileSystemNuGetProject
            //downloadResourceResult.PackageStream.Seek(0, SeekOrigin.Begin);
            //var addedPackageFilesList = new List<string>();
            //if (downloadResourceResult.PackageReader != null)
            //{
            //    addedPackageFilesList.AddRange(
            //        PackageExtractor.ExtractPackage(
            //            downloadResourceResult.PackageReader,
            //            downloadResourceResult.PackageStream,
            //            PackagePathResolver,
            //            packageExtractionContext,
            //            cancellationToken));
            //}
            //else
            //{
            //    addedPackageFilesList.AddRange(
            //        PackageExtractor.ExtractPackage(
            //            downloadResourceResult.PackageStream,
            //            PackagePathResolver,
            //            packageExtractionContext,
            //            cancellationToken));
            //}

            //var packageSaveMode = PackageSaveMode.Defaultv3;
            //if (packageSaveMode.HasFlag(PackageSaveMode.Nupkg))
            //{
            //    var packageFilePath = GetInstalledPackageFilePath(packageIdentity);
            //    if (File.Exists(packageFilePath))
            //    {
            //        addedPackageFilesList.Add(packageFilePath);
            //    }

            //}

            //// Pend all the package files including the nupkg file
            //FileSystemUtility.PendAddFiles(addedPackageFilesList, Root, nuGetProjectContext);
            ////nuGetProjectContext.Log(MessageLevel.Info, Strings.AddedPackageToFolder, packageIdentity, Path.GetFullPath(Root));

            //// Extra logging with source for verbosity detailed
            //// Used by external tool CoreXT to track package provenance
            ////if (!string.IsNullOrEmpty(downloadResourceResult.PackageSource))
            ////{
            ////    nuGetProjectContext.Log(MessageLevel.Debug, Strings.AddedPackageToFolderFromSource, packageIdentity, Path.GetFullPath(Root), downloadResourceResult.PackageSource);
            ////}

            //return Task.FromResult(true);

            ////},
            ////token: token);

        }

        //public string GetInstalledPackageFilePath(PackageIdentity packageIdentity)
        //    =>GetInstalledFilePath(packageIdentity, PackagePathResolver.GetPackageFileName(packageIdentity));

        //public string GetInstalledManifestFilePath(PackageIdentity packageIdentity)
        //    =>GetInstalledFilePath(packageIdentity, PackagePathResolver.GetManifestFileName(packageIdentity));

        //private string GetInstalledFilePath(PackageIdentity packageIdentity, string fileName)
        //{
        //    // Check the expected location before searching all directories
        //    var packageDirectory = PackagePathResolver.GetInstallPath(packageIdentity);

        //    var installPath = Path.GetFullPath(Path.Combine(packageDirectory, fileName));

        //    // Keep the previous optimization of just going by the existance of the file if we find it.
        //    if (File.Exists(installPath))
        //        return installPath;
        //    return string.Empty;
        //}

        public bool PackageExists(/*PackageIdentity packageIdentity, PackageSaveMode packageSaveMode*/)
        {
            throw new NotImplementedException();
            //var packageExists = !string.IsNullOrEmpty(GetInstalledPackageFilePath(packageIdentity));
            //var manifestExists = !string.IsNullOrEmpty(GetInstalledManifestFilePath(packageIdentity));

            //// A package must have either a nupkg or a nuspec to be valid
            //var result = packageExists || manifestExists;

            //// Verify nupkg present if specified
            //if ((packageSaveMode & PackageSaveMode.Nupkg) == PackageSaveMode.Nupkg)
            //    result &= packageExists;

            //// Verify nuspec present if specified
            //if ((packageSaveMode & PackageSaveMode.Nuspec) == PackageSaveMode.Nuspec)
            //    result &= manifestExists;

            //return result;

        }
    }
}
