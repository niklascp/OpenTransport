﻿using Microsoft.AspNet.Builder;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Runtime;

using OpenTransport.EventBroker;

namespace EventBroker
{
    public class Startup
    {
        public Startup(IApplicationEnvironment applicationEnvironment, IRuntimeEnvironment runtimeEnvironment)
        {

        }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISubscriptionManager>(x => new InMemorySubscriptionManager());
            services.AddSingleton<EventTestProvider>();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerfactory)
        {
            app.UseSignalR();
            app.UseStaticFiles();
        }
    }
}
