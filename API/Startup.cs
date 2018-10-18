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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestServer1.Domain;
using RestServer1.Core;
using RestServer1.DAL;
using RestServer1.Core.Abstract;
using RestServer1.API.Modules;

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
            services.AddOptions();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<ServiceSettings>(options =>
            {
                options.MongoConnectionString = this.Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.MongoDatabase = this.Configuration.GetSection("MongoConnection:Database").Value;
                options.MongoCollection = this.Configuration.GetSection("MongoConnection:Collection").Value;
            });

            // Bind specific services
            services.AddSingleton<RestServer1.Core.Abstract.ILogger, Logger>();
            services.AddSingleton<RestServer1.DAL.Abstract.ILoggerData, MongoDbLoggerData>();
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
