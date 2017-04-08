using Genus.Modularity;
using Microsoft.AspNetCore.Builder;

namespace Genus.AspNetCore.Modularity
{
    public interface IAspNetCorePlugin:IPlugin
    {
        string UrlPrefix { get; }
        void Configure(IApplicationBuilder appBuilder);
    }
}
