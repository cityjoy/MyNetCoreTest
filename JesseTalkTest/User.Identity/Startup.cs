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
            #region ���IdentityServer4�û��ڴ����� 
            //�����������ȫ�����õ��ڴ��� �����EFCore���ݿ�־û����ﲻ������ 
            //ֻ��Ҫ���� AddConfigurationStore��AddOperationalStore ���ݲִ�����
            //services.AddIdentityServer()
            //    .AddExtensionGrantValidator<SmSCodeValidator>()
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //    .AddInMemoryApiResources(Config.GetApiResources())
            //    .AddInMemoryClients(Config.GetClients());
            #endregion
            #region ��Ӷ�IdentityServer4 EF���ݿ�־û�֧�� 

            //���IdentityServer4��EFCore���ݿ��֧��
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

                options.EnableTokenCleanup = true;  //�����Token������
                options.TokenCleanupInterval = 30;  //��������ʱ��Secends
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
