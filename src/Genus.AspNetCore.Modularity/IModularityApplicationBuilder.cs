using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Genus.AspNetCore.Modularity
{
    public interface IModularityApplicationBuilder : IApplicationBuilder
    {
        IModularityApplicationBuilder UseBefore<TMiddleware>(Func<RequestDelegate, RequestDelegate> middleware);
        IModularityApplicationBuilder UseAfter<TMiddleware>(Func<RequestDelegate, RequestDelegate> middleware);
    }
}
