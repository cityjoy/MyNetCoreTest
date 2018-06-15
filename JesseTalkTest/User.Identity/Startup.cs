using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.Identity.Services;
using System.Net.Http;
using User.Identity.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace User.Identity
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
            const string connectionString = @"Data Source=(local);Initial Catalog=BlogDbContext;User ID=llj;password=pwd123456;Integrated Security=false;";
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            #region 添加IdentityServer4用户内存数据 
            //添加配置数据全部配置到内存中 如果有EFCore数据库持久化这里不会配置 
            //只需要配置 AddConfigurationStore、AddOperationalStore 数据仓储服务
            //services.AddIdentityServer()
            //    .AddExtensionGrantValidator<SmSCodeValidator>()
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddInMemoryApiResources(Config.GetApiResources())
            //    .AddInMemoryClients(Config.GetClients());
            #endregion
            #region 添加对IdentityServer4 EF数据库持久化支持 

            //添加IdentityServer4对EFCore数据库的支持
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = (builder) =>
                    {
                        builder.UseSqlServer(connectionString,
                            builderoptions =>
                            {
                                builderoptions.MigrationsAssembly(migrationsAssembly);
                            });
                    };
                })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = (builder) =>
                {
                    builder.UseSqlServer(connectionString, builderoptions =>
                    {
                        builderoptions.MigrationsAssembly(migrationsAssembly);
                    });

                };

                options.EnableTokenCleanup = true;  //允许对Token的清理
                options.TokenCleanupInterval = 30;  //清理周期时间Secends
            });
            #endregion


            services.AddSingleton(new HttpClient());
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthCodeService, AuthCodeService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseIdentityServer();
            app.UseMvc();
        }
    }
}
