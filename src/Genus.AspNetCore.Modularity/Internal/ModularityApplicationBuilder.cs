using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Genus.AspNetCore.Modularity.Internal
{
    internal class ModularityApplicationBuilder : IModularityApplicationBuilder
    {
        class MidlewareItem
        {
            public Type MidlewareType { get; set; }
            public Func<RequestDelegate, RequestDelegate> RequestDelegate { get; set; }
        }
        public ModularityApplicationBuilder(IApplicationBuilder baseBuilder)
        {
            _baseBuilder = baseBuilder ?? throw new ArgumentNullException(nameof(baseBuilder));            
        }

        readonly IApplicationBuilder _baseBuilder;

        List<MidlewareItem> _midlewares = new List<MidlewareItem>();

        public IServiceProvider ApplicationServices {
            get => _baseBuilder.ApplicationServices;
            set => throw new NotSupportedException();
        }

        public IFeatureCollection ServerFeatures => _baseBuilder.ServerFeatures;

        public IDictionary<string, object> Properties => _baseBuilder.Properties;

        public RequestDelegate Build()
            => throw new NotSupportedException();

        public IApplicationBuilder New()
            => _baseBuilder.New();

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
            => Use(middleware, _midlewares.Count);

        private IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware, int pos)
        {
            var midlewareType = (Type)middleware.Target.GetType().GetTypeInfo()
                                        .GetDeclaredField("middleware")
                                        ?.GetValue(middleware.Target);
            _midlewares.Insert(pos, new MidlewareItem
            {
                MidlewareType = midlewareType,
                RequestDelegate = middleware
            });
            return this;
        }


        public void Merge(IApplicationBuilder appBuilder)
        {
            foreach (var middleware in _midlewares)
            {
                appBuilder.Use(middleware.RequestDelegate);
            }
        }

        public IModularityApplicationBuilder UseBefore<TBeforeMiddleware>(Func<RequestDelegate, RequestDelegate> middleware)
        {
            var prevMidleware = _midlewares.FirstOrDefault(mi => mi.MidlewareType == typeof(TBeforeMiddleware));
            int pos = _midlewares.Count;
            if (prevMidleware != null)
                pos = _midlewares.IndexOf(prevMidleware);
            Use(middleware, pos);
            return this;
        }

        public IModularityApplicationBuilder UseAfter<TAfterMiddleware>(Func<RequestDelegate, RequestDelegate> middleware)
        {
            var prevMidleware = _midlewares.LastOrDefault(mi => mi.MidlewareType == typeof(TAfterMiddleware));
            int pos = _midlewares.Count;
            if (prevMidleware != null)
                pos = _midlewares.IndexOf(prevMidleware)+1;
            Use(middleware, pos);
            return this;
        }
    }
}
