using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Castle.Facilities.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using LTMCompanyNameFree.YoyoCmsTemplate.Authentication.JwtBearer;
using LTMCompanyNameFree.YoyoCmsTemplate.Configuration;
using LTMCompanyNameFree.YoyoCmsTemplate.Identity;

using Abp.AspNetCore.SignalR.Hubs;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;

namespace LTMCompanyNameFree.YoyoCmsTemplate.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // MVC
            services.AddMvc(
                options => options.Filters.Add(new CorsAuthorizationFilterFactory(_defaultCorsPolicyName))
            );

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddSignalR();

            // 只有 Debug 的时候才使用跨域
#if DEBUG 
            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );
#endif


            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "YoyoCmsTemplate API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("bearerAuth", new ApiKeyScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                // Assign scope requirements to operations based on AuthorizeAttribute
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {

                configuration.RootPath = "ClientApp/dist";
            });

            // Configure Abp and Dependency Injection
            return services.AddAbp<YoyoCmsTemplateWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {


            app.Map("/test", testApp =>
             {
                 //testApp.UseSpaAbsolute(env, "/", @"D:\develop\staneee\AngularApps\FreeAppTest");
                 testApp.UseSpaStaticFiles();
                 testApp.UseSpaAbsolute(env, "/test", @"D:\develop\staneee\AngularApps\FreeAppTest");
                 // Use SPA Static Files
                 testApp.UseSpa(spa =>
                 {
                     // 开发模式使用反向代理
                     if (env.IsDevelopment())
                     {
                         spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:4201");
                     }
                 });

                 testApp.Run(async (context) =>
                 {
                     await context.Response.WriteAsync("");
                 });

             });

            app.Map("/abc", abcApp =>
            {
                abcApp.UseSpaAbsolute(env, "/abc", @"D:\develop\staneee\AngularApps\FreeAppAbc");
                abcApp.UseSpa(spa =>
                {
                    // 开发模式使用反向代理
                    if (env.IsDevelopment())
                    {
                        spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:4202");
                    }
                });

            });

            app.Map("", mpa =>
            {
                // Initializes ABP framework.
                mpa.UseAbp(options => { options.UseAbpRequestLocalization = false; });

                // Enable CORS!
                mpa.UseCors(_defaultCorsPolicyName); 
                mpa.UseStaticFiles();

                mpa.UseAuthentication();

                mpa.UseAbpRequestLocalization();

                mpa.UseSignalR(routes =>
                {
                    routes.MapHub<AbpCommonHub>("/signalr");
                });

                mpa.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "area",
                        template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });

                mpa.UseSwagger();
                // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
                mpa.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "YoyoCmsTemplate API V1");
                    options.IndexStream = () => Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("LTMCompanyNameFree.YoyoCmsTemplate.Web.Host.wwwroot.swagger.ui.index.html");
                }); // URL: /swagger

            });

        }
    }
}
