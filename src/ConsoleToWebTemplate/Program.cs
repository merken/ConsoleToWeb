using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using FuncR;
using System;
using System.Linq;

namespace ConsoleToWebTemplate
{
    interface IFooService
    {
        string Foo(int numberOfFoos);
    }

    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static bool IsRootRequest(HttpRequest request)
            => request.Path.Value.Equals("/");

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScopedFunction<IFooService>
                (nameof(IFooService.Foo))
                .Runs<int, string>(numberOfFoos =>
                {
                    var foos = Enumerable.Range(1, numberOfFoos).Select(n => "Foo");
                    return $"{String.Join(", ", foos)}";
                });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureServices)
                .ConfigureWebHost(
                    webHost => webHost
                        .UseKestrel(kestrelOptions => { kestrelOptions.ListenAnyIP(5005); }) // HTTP
                        .Configure(app => app
                            .Run(
                                async context =>
                                {
                                    if (!IsRootRequest(context.Request))
                                        return; // Any other request then '/'
                                        
                                    var fooService = context.RequestServices.GetRequiredService<IFooService>();
                                    await context.Response.WriteAsync(fooService.Foo(4));
                                }
                            )));
    }
}