﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Consul;
using ConsulSharp.Agent;
using ConsulSharp.Agent.Service;
using ConsulSharp.Agent.Check;

namespace API01   
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
            services.AddAuthentication("Bearer")//配置IdentityServer授权服务
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";//授权服务器地址
                    options.RequireHttpsMetadata = false;

                    options.ApiName = "api01";
                });
            services.AddMvc();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            lifetime.ApplicationStarted.Register(ServiceRegister);//向consul注册服务
            app.UseAuthentication();
            app.UseMvc();
        }

        /// <summary>
        /// Register Service
        /// </summary>
        private static void RegisterService()
        {
            var agentGovern = new AgentGovern();
            var result = agentGovern.RegisterServices(new RegisterServiceParmeter
            {
                ID = "test0001",
                Name = "values_api",
                Address = "127.0.0.1",
                Port = 1645
            }).GetAwaiter().GetResult();
            
        }

        private static void ServiceRegister()
        {
            var client = new ConsulClient();
            var result = client.Agent.ServiceRegister(new AgentServiceRegistration()
            {
                Address = "127.0.0.1",
                ID = "values_api01",
                Name = "values_api1",

                Port = 1645,
                Check = new AgentServiceCheck()
                {
                    HTTP = "http://127.0.0.1:1645/api/check",
                    Interval = new TimeSpan(0, 0, 10),
                    DeregisterCriticalServiceAfter = new TimeSpan(0, 1, 0),
                }
            }).ConfigureAwait(false);
        }
    }
}
