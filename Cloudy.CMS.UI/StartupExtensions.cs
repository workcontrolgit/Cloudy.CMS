﻿using Microsoft.Extensions.DependencyInjection;
using Poetry;
using Cloudy.CMS;
using Cloudy.CMS.DocumentSupport;
using Cloudy.CMS.ContentSupport;
using Cloudy.CMS.ContentSupport.RepositorySupport;
using Cloudy.CMS.ContentSupport.Serialization;
using Cloudy.CMS.ContentTypeSupport;
using Cloudy.CMS.ContentTypeSupport.PropertyMappingSupport;
using Cloudy.CMS.Core;
using Cloudy.CMS.Core.ContentSupport.RepositorySupport;
using Cloudy.CMS.Mvc.Routing;
using Cloudy.CMS.Reflection;
using Cloudy.CMS.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Poetry.AspNetCore;
using Poetry.UI.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Poetry.InitializerSupport;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Poetry.ComponentSupport;
using Cloudy.CMS.UI.PortalSupport;
using Cloudy.CMS.UI;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;

namespace Cloudy.CMS
{
    public static class StartupExtensions
    {
        public static CloudyConfigurator AddAdmin(this CloudyConfigurator configurator)
        {
            configurator.AddComponent<CloudyAdminComponent>();

            return configurator;
        }

        public static void UseCloudyAdmin(this IApplicationBuilder app, Action<CloudyAdminConfigurator> configure)
        {
            if (app.ApplicationServices.GetService(typeof(IComponentTypeProvider)) == null)
            {
                throw new Exception("Please add Cloudy services first by doing: services.AddCloudy(...)");
            }

            if (!((IComponentTypeProvider)app.ApplicationServices.GetService(typeof(IComponentTypeProvider))).GetAll().Contains(typeof(CloudyAdminComponent)))
            {
                throw new Exception("Please add Cloudy Admin services first by doing: services.AddCloudy(cloudy => cloudy.AddAdmin())");
            }

            var options = new CloudyAdminOptions();
            var configurator = new CloudyAdminConfigurator(options);

            configure(configurator);

            if (options.StaticFilesBaseUri == null && options.StaticFilesFileProvider == null)
            {
                configurator.WithStaticFilesFromVersion(Assembly.GetExecutingAssembly().GetName().Version);
            }

            if (!options.AllowUnauthenticatedUsers && options.AuthorizeOptions == null)
            {
                throw new ArgumentException($"You have called neither {nameof(CloudyAdminConfigurator.Authorize)}() or {nameof(CloudyAdminConfigurator.Unprotect)}(). You probably want to use the first one");
            }

            if (options.AllowUnauthenticatedUsers && options.AuthorizeOptions != null)
            {
                throw new ArgumentException($"You have called both {nameof(CloudyAdminConfigurator.Authorize)}() and {nameof(CloudyAdminConfigurator.Unprotect)}(), they are mutually exclusive. You probably want to remove the latter");
            }

            app.Map(new PathString(options.BasePath), branch => app.ApplicationServices.GetService<IPipelineBuilder>().Build(branch, options));
        }
    }
}