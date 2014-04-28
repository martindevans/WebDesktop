using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Conventions;
using Nancy.Json;
using Nancy.Serialization.JsonNet;
using Nancy.ViewEngines.Razor;
using Newtonsoft.Json;
using Ninject;
using TransparentWindow.Extensions;

namespace TransparentWindow.Nancy
{
    public class Bootstrapper
        :NinjectNancyBootstrapper
    {
        private readonly IKernel _kernel;

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(c => c.Serializers.Insert(0, typeof(JsonNetSerializer)));
            }
        }

        public Bootstrapper(IKernel kernel)
        {
            _kernel = kernel;

            StaticConfiguration.DisableErrorTraces = false;
            StaticConfiguration.EnableRequestTracing = true;

            ApplicationPipelines.AfterRequest.AddItemToEndOfPipeline(x => x.Response.WithHeader("Access-Control-Allow-Origin", "*"));
            ApplicationPipelines.AfterRequest.AddItemToEndOfPipeline(x => x.Response.WithHeader("Access-Control-Allow-Methods", "DELETE, GET, HEAD, POST, PUT, OPTIONS, PATCH"));
            ApplicationPipelines.AfterRequest.AddItemToEndOfPipeline(x => x.Response.WithHeader("Access-Control-Allow-Headers", "Content-Type"));
            ApplicationPipelines.AfterRequest.AddItemToEndOfPipeline(x => x.Response.WithHeader("Accept", "application/json"));

            //Default format to JSON
            ApplicationPipelines.BeforeRequest.AddItemToStartOfPipeline(x =>
            {
                x.Request.Headers.Accept = x.Request.Headers.Accept.Append(new Tuple<string, decimal>("application/json", 1.05m));
                return null;
            });
        }

        protected override IKernel GetApplicationContainer()
        {
            _kernel.Load<FactoryModule>();
            return _kernel;
        }

        protected override void ConfigureApplicationContainer(IKernel container)
        {
            base.ConfigureApplicationContainer(container);
        }

        protected override void ApplicationStartup(IKernel container, IPipelines pipelines)
        {
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("Nancy/views/", context.ModuleName, "/", viewName));
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("Nancy/", context.ModuleName, "/", viewName));
            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat("Nancy/Modules/", context.ModuleName, "/", viewName));

            JsonSettings.MaxJsonLength = int.MaxValue;

            base.ApplicationStartup(container, pipelines);
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("views", @"Nancy/views"));
        }
    }

    public class RazorConfiguration : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            return new string[0];
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            return new string[0];
        }

        public bool AutoIncludeModelNamespace
        {
            get { return true; }
        }
    }
}
