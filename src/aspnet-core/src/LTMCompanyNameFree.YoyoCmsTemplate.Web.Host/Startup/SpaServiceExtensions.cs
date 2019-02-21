using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LTMCompanyNameFree.YoyoCmsTemplate.Web.Host.Startup
{
    public static class SpaServiceExtensions
    {
        /// <summary>
        /// 相对路径添加单页应用
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="relativeRequestPath">
        /// 映射到静态资源的相对请求路径( 如 http://12.0.0.1/test中的test )
        /// </param>
        /// <param name="staticFilesRelativeDirectoryPath">
        /// 静态文件目录，相对于程序路径所在位置的目录路径
        /// </param>
        /// <returns></returns>
        public static IApplicationBuilder UseSpaRelative(this IApplicationBuilder app, IHostingEnvironment env, string relativeRequestPath, string staticFilesRelativeDirectoryPath)
        {
            // Use SPA Static Files
            app.UseSpaStaticFiles(new StaticFileOptions(new Microsoft.AspNetCore.StaticFiles.Infrastructure.SharedOptions()
            {
                RequestPath = relativeRequestPath,
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, staticFilesRelativeDirectoryPath)),
            }));


            return app;
        }

        /// <summary>
        /// 相对路径添加单页应用
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="relativeRequestPath">
        /// 映射到静态资源的相对请求路径( 如 http://12.0.0.1/test中的test )
        /// </param>
        /// <param name="staticFilesAbsoluteDirectoryPath">
        /// 静态文件目录，绝对路径
        /// </param>
        /// <returns></returns>
        public static IApplicationBuilder UseSpaAbsolute(this IApplicationBuilder app, IHostingEnvironment env, string relativeRequestPath, string staticFilesAbsoluteDirectoryPath)
        {
            // Use SPA Static Files
            app.UseSpaStaticFiles(new StaticFileOptions(new Microsoft.AspNetCore.StaticFiles.Infrastructure.SharedOptions()
            {
                RequestPath = relativeRequestPath,
                FileProvider = new PhysicalFileProvider(staticFilesAbsoluteDirectoryPath),
            }));


            return app;
        }
    }
}
