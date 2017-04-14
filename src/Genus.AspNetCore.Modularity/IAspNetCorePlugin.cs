using Genus.Modularity;

namespace Genus.AspNetCore.Modularity
{
    public interface IAspNetCorePlugin:IPlugin
    {
        string UrlPrefix { get; }
        void Configure(IModularityApplicationBuilder appBuilder);
    }
}
