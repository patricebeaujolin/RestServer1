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
using System.Linq.Expressions;
using Microsoft.VisualBasic.CompilerServices;
using log4net;
using Microsoft.Extensions.Logging.Console;

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

            // Initialize the ApplicationSettings singleton
            var applicationSettings = new ApplicationSettings()
            {
                Logger = new ServiceSettings()
                {
                    DataImplementation = this.Configuration.GetSection("Logger:DataImplementation").Value,
                    OnStartup = this.Configuration.GetSection("Logger:OnStartup").Value
                },

                Mongo = new MongoSettings()
                {
                    ConnectionString = this.Configuration.GetSection("MongoConnection:ConnectionString").Value,
                    Database = this.Configuration.GetSection("MongoConnection:Database").Value,
                    Collection = this.Configuration.GetSection("MongoConnection:Collection").Value
                }
            };

            // Bind specific services to be used with ASP.NET DI
            services.AddSingleton<RestServer1.Domain.Abstract.IApplicationSettings>(applicationSettings);
            services.AddSingleton<RestServer1.Core.Abstract.IEventLogger, EventLogger>();

            switch (applicationSettings.Logger.DataImplementation)
            {
                case "memory":
                    services.AddSingleton<RestServer1.DAL.Abstract.ILoggerData, MemoryLoggerData>();
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
