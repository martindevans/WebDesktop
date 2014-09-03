using System.Collections.Generic;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Bootstrappers.Ninject;
using Nancy.Conventions;
using Nancy.Json;
using Nancy.ViewEngines.Razor;
using Ninject;

namespace TransparentWindow.Nancy
{
    public class Bootstrapper
        :NinjectNancyBootstrapper
    {
        private readonly IKernel _kernel;

        public Bootstrapper(IKernel kernel)
        {
            _kernel = kernel;

            StaticConfiguration.DisableErrorTraces = false;
            StaticConfiguration.EnableRequestTracing = true;

            ApplicationPipelines.AfterRequest.AddItemToEndOfPipeline(x => x.Response.WithHeader("Access-Control-Allow-Origin", "*"));
            ApplicationPipelines.AfterRequest.AddItemToEndOfPipeline(x => x.Response.WithHeader("Access-Control-Allow-Methods", "DELETE, GET, HEAD, POST, PUT, OPTIONS, PATCH"));
            ApplicationPipelines.AfterRequest.AddItemToEndOfPipeline(x => x.Response.WithHeader("Access-Control-Allow-Headers", "Content-Type"));
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
