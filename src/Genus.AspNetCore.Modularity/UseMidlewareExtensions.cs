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

        public static IModularityApplicationBuilder UseBefore<TMiddleware>(this IModularityApplicationBuilder builder, Func<RequestDelegate, RequestDelegate> middleware)
            => builder.UseBefore(typeof(TMiddleware), middleware);

        public static IModularityApplicationBuilder UseAfter<TMiddleware>(this IModularityApplicationBuilder builder, Func<RequestDelegate, RequestDelegate> middleware)
            => builder.UseAfter(typeof(TMiddleware), middleware);

        public static IModularityApplicationBuilder UseFirst(
                this IModularityApplicationBuilder builder,
                Action<IApplicationBuilder> useAction)
        {
            builder.internalUseMiddleware(useAction, (b, m) => b.UseFirst(m));
            return builder;
        }

        public static IModularityApplicationBuilder UseBefore<TBeforeMiddleware>(
                this IModularityApplicationBuilder builder,
                     Action<IApplicationBuilder> useAction)
        {
            builder.internalUseMiddleware(useAction, (b, m) => b.UseBefore<TBeforeMiddleware>(m));
            return builder;
        }

        public static IModularityApplicationBuilder UseAfter<TAfterMiddleware>(
               this IModularityApplicationBuilder builder,
                    Action<IApplicationBuilder> useAction)
        {
            builder.internalUseMiddleware( useAction,(b, m) => b.UseAfter<TAfterMiddleware>(m));
            return builder;
        }

        public static IModularityApplicationBuilder UseMiddlewareFirst<TMiddleware>(
                this IModularityApplicationBuilder builder,
              params object[] args)
        {
            internalUseMiddleware<TMiddleware>(
                builder,
                (b, m) => b.UseFirst(m),
                args);
            return builder;
        }

        public static IModularityApplicationBuilder UseMiddlewareBefore<TMiddleware, TBeforeMiddleware>( 
                this IModularityApplicationBuilder builder, 
              params object[] args)
        {
            builder.internalUseMiddleware<TMiddleware>(
                (b, m) => b.UseBefore<TBeforeMiddleware>(m),
                args
            );
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
            => builder.internalUseMiddleware(ab => ab.UseMiddleware<TMiddleware>(args), useAction);

        private static void internalUseMiddleware(
            this IModularityApplicationBuilder builder,
                 Action<InternalAppBuilder> middlewareFactory,
                 Action<IModularityApplicationBuilder, Func<RequestDelegate, RequestDelegate>> useAction,
          params object[] args)
        {
            var internalAB = new InternalAppBuilder(builder);
            middlewareFactory(internalAB);
            foreach (var item in internalAB.Middlewares)
            {
                useAction(builder, item);
            }
        }
    }
}
