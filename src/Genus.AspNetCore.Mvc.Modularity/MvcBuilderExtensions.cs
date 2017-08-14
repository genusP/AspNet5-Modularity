using Genus.AspNetCore.Modularity;
using Genus.AspNetCore.Mvc.Modularity;
using Genus.Modularity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace Genus.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddPlugins(this IMvcBuilder builder)
        {
            builder.Services.AddPluginApplicationModel();
            builder.AddPluginViewLocations();
            return builder;
        }

        private static IMvcBuilder AddPluginViewLocations(this IMvcBuilder builder)
        {
            var pm = GetPMFromServices(builder.Services);
            return builder.AddRazorOptions(o=> o.ConfigurePluginsView(pm));
        }

        private static IPluginManager GetPMFromServices(IServiceCollection services)
        {
            return services.BuildServiceProvider().GetService<IPluginManager>();
        }

        public static RazorViewEngineOptions ConfigurePluginsView( this RazorViewEngineOptions options, IPluginManager pluginManager)
        {
            options.ViewLocationExpanders.Insert(0, new PluginViewLocationExpander());
            if (pluginManager != null)
            {
                foreach (var pluginInfo in pluginManager.LoadedPlugins)
                {
                    options.FileProviders.Add(new PluginFileProvider(pluginInfo, "Views", "Views"));
                    options.AdditionalCompilationReferences.Add(Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(pluginInfo.Assembly.Location));
                }
            }
            return options;
        }
    }
}
