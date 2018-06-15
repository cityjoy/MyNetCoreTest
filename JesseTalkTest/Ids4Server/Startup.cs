using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Consul;
using System;

namespace Ids4Server
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
            //const string connectionString = @"Data Source=(local);Initial Catalog=BlogDbContext;User ID=llj;password=pwd123456;Integrated Security=false;";
            //var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            #region 添加Ids4Server4用户内存数据 
            //添加配置数据全部配置到内存中 
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients());
            #endregion

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            lifetime.ApplicationStarted.Register(ServiceRegister);

            app.UseIdentityServer();
            app.UseMvc();
        }
        private static void ServiceRegister()
        {
            var client = new ConsulClient();
            var result = client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                Address = "127.0.0.1",
                ID = "identieyService",
                Name = "identieyService",

                Port = 5000,
                Check = new AgentServiceCheck()
                {
                    HTTP = "http://127.0.0.1:5000/api/check",
                    Interval = new TimeSpan(0, 0, 10),
                    DeregisterCriticalServiceAfter = new TimeSpan(0, 1, 0),
                }
            }).ConfigureAwait(false);
        }
    }
}
