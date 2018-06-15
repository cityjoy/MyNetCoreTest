using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using User.API.Data;
using Microsoft.EntityFrameworkCore;
using User.API.Models;

namespace User.API
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
            services.AddDbContextPool<UserContext>(options => options.UseMySQL("Server=localhost;Port=3306;Database=MyTestDB; User=jerry;Password=pwd123456;")); //配置mariadb连接字符串}
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            InitUserContext(app);
        }
        public void InitUserContext(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UserContext>();

                context.Database.EnsureCreated();

                if (!context.Users.Any())
                {
                    context.Add(new AppUser
                    {
                        Name = "jerry",
                        Phone="10085",
                        Company="中国移动"
                    });
                    context.Add(new AppUser
                    {
                        Name = "sam",
                        Phone = "10010",
                        Company = "中国联通"
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}

