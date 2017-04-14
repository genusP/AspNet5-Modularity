using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Genus.AspNetCore.Modularity
{
    public static class UseMidlewareExtensions
    {
        class InternalAppBuilder : IApplicationBuilder
        {
            public InternalAppBuilder(IApplicationBuilder baseBuilder)
            {
                _baseBuilder = baseBuilder;
            }

            readonly IApplicationBuilder _baseBuilder;

            public IServiceProvider ApplicationServices
            {
                get => _baseBuilder.ApplicationServices;
                set => throw new NotSupportedException();
            }

            public IFeatureCollection ServerFeatures => _baseBuilder.ServerFeatures;

            public IDictionary<string, object> Properties => _baseBuilder.Properties;

            public RequestDelegate Build()
            {
                throw new NotSupportedException();
            }

            public IApplicationBuilder New()
            {
                throw new NotSupportedException();
            }

            public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
            {
                Middlewares.Add(middleware);
                return this;
            }

            public List<Func<RequestDelegate, RequestDelegate>> Middlewares { get; } = new List<Func<RequestDelegate, RequestDelegate>>();
        }

        public static IModularityApplicationBuilder UseMiddlewareBefore<TMiddleware, TBeforeMiddleware>( 
                this IModularityApplicationBuilder builder, 
              params object[] args)
        {
            internalUseMiddleware<TMiddleware>(
                builder,
                (b, m) => b.UseBefore<TBeforeMiddleware>(m),
                args);
            return builder;
        }

        public static IModularityApplicationBuilder UseMiddlewareAfter<TMiddleware, TAfterMiddleware>(
                this IModularityApplicationBuilder builder,
              params object[] args)
        {
            internalUseMiddleware<TMiddleware>(
                builder,
                (b, m) => b.UseAfter<TAfterMiddleware>(m),
                args);
            return builder;
        }

        private static void internalUseMiddleware<TMiddleware>(
            this IModularityApplicationBuilder builder,
                 Action<IModularityApplicationBuilder, Func<RequestDelegate, RequestDelegate>> useAction,
          params object[] args)
        {
            var internalAB = new InternalAppBuilder(builder);
            internalAB.UseMiddleware<TMiddleware>(args);
            foreach (var item in internalAB.Middlewares)
            {
                useAction(builder, item);
            }
        }
    }
}
