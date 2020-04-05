using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pipaslot.Logging.Demo.Services;
using Pipaslot.Logging.Writers;

namespace Pipaslot.Logging.Demo
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient<ServiceLevel1>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Logging with own logger implementation
            services.AddLogger(LogLevel.Trace, s => new WriterCollection()
            {
                new RequestWriter(Path.Combine(Directory.GetCurrentDirectory(),"logs"), "{Date}-requests.log"),
                //new FlatWriter(logSettings.RootDirectory, logSettings.ApplicationName + "{Date}-service1.log", typeof(ServiceLevel1).FullName, Constants.Constant.Logging.Personalizations, LogLevel.Debug),
                //new FlatWriter(logSettings.RootDirectory, logSettings.ApplicationName + "{Date}-errors.log", LogLevel.Error),
                //new ProcessWriter(logSettings.RootDirectory, logSettings.ApplicationName + "{Date}-processes-{Id}.log"),
                //new SendWriter(s.GetService<EmailSender>(), LogLevel.Critical)
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, LoggerProvider loggerProvider)
        {
            loggerFactory.AddProvider(loggerProvider);
            
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
