using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;

namespace Genus.Modularity
{
    public static class PlatformInformation
    {
        private static TargetFrameworkAttribute _frameworkAttribute;
        private static IFrameworkNameProvider _frameworkNameProvider = new DefaultFrameworkNameProvider();
        private static Lazy<NuGetFramework> _currentNugetFramework = new Lazy<NuGetFramework>(()=>NuGetFramework.ParseFrameworkName(PlatformName, _frameworkNameProvider));
        public static string PlatformName {
            get
            {
                if (_frameworkAttribute == null)
                    _frameworkAttribute = Assembly.GetEntryAssembly().GetCustomAttribute<TargetFrameworkAttribute>();
                return _frameworkAttribute.FrameworkName;
            }
        }

        public static bool IsCompatibleFramework(NuGetFramework nugetFramework)
            => DefaultCompatibilityProvider.Instance.IsCompatible(_currentNugetFramework.Value, nugetFramework);

        public static bool IsCompatibleFramework(string folderName)
            => IsCompatibleFramework(NuGetFramework.ParseFolder(folderName));
    }
}
