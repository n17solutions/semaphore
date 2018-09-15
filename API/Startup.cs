using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using N17Solutions.Semaphore.Data.Extensions;
using N17Solutions.Semaphore.Handlers.Extensions;
using N17Solutions.Semaphore.Requests.Extensions;

namespace N17Solutions.Semaphore.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseConnectionString = Environment.GetEnvironmentVariable("SEMAPHORE_DB") ?? _configuration.GetConnectionString("Semaphore");
            if (string.IsNullOrEmpty(databaseConnectionString))
                throw new InvalidOperationException("No database connection string, please either set the SEMAPHORE_DB Environment Variable or add a Semaphore Connection String.");
            
            services
                .AddRequests()
                .AddHandlers()
                .AddDatabaseContext(databaseConnectionString)
                .AddMvc();
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

            app.UseMvc();
        }
    }
}