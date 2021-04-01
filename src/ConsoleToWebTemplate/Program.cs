using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleToWebTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static bool IsRootRequest(HttpRequest request)
            => request.Path.Value.Equals("/");

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHost(
                    webHost => webHost
                        .UseKestrel(kestrelOptions => { kestrelOptions.ListenAnyIP(5005); }) // HTTP
                        .Configure(app => app
                            .Run(
                                async context =>
                                {
                                    if (!IsRootRequest(context.Request))
                                        return; // Any other request then '/'

                                    await context.Response.WriteAsync("Hello World!");
                                }
                            )));
    }
}