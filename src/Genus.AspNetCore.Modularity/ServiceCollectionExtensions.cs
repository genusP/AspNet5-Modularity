using Genus.AspNetCore.Modularity.ApplicationModel;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace Genus.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPluginApplicationModel(this IServiceCollection services)
        {
            services.AddTransient<IApplicationModelProvider, PluginApplicationModelProvider>();
        }
    }
}
