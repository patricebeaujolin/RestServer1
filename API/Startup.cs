using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestServer1.Domain;
using RestServer1.Core.Abstract;
using RestServer1.Core;
using RestServer1.DAL;

namespace RestServer1.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Initialize the ServiceSettings singleton
            var serviceSettings = new ServiceSettings()
            {
                MongoConnectionString = this.Configuration.GetSection("MongoConnection:ConnectionString").Value,
                MongoDatabase = this.Configuration.GetSection("MongoConnection:Database").Value,
                MongoCollection = this.Configuration.GetSection("MongoConnection:Collection").Value,
                MongoResetEvents = bool.Parse(this.Configuration.GetSection("MongoConnection:ResetEvents").Value)
            };

            // Bind specific services to be used with ASP.NET DI
            services.AddSingleton<RestServer1.Domain.Abstract.IServiceSettings>(serviceSettings);
            services.AddSingleton<RestServer1.Core.Abstract.IEventLogger, EventLogger>();

            switch (this.Configuration.GetSection("Startup:LoggerDataImplementation").Value)
            {
                case "fake":
                    services.AddSingleton<RestServer1.DAL.Abstract.ILoggerData, FakeLoggerData>();
                    break;
                case "mongo":
                    services.AddSingleton<RestServer1.DAL.Abstract.ILoggerData, MongoDbLoggerData>();
                    break;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
