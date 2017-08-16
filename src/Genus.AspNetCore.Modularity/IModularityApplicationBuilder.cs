using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Genus.AspNetCore.Modularity
{
    public interface IModularityApplicationBuilder : IApplicationBuilder
    {
        IModularityApplicationBuilder UseFirst(Func<RequestDelegate, RequestDelegate> middleware);
        IModularityApplicationBuilder UseBefore(Type beforeMiddelwareType, Func<RequestDelegate, RequestDelegate> middleware);
        IModularityApplicationBuilder UseAfter(Type afterMiddelwareType, Func<RequestDelegate, RequestDelegate> middleware);
    }
}
