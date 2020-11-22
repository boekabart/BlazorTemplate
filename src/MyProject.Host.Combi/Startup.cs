using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyProject.Backend.Controller.Hubs;
using MyProject.Common;
using System.Linq;

namespace MyProject.Host.Combi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services
                .AddControllers()
                .AddApplicationPart(typeof(Backend.Controller.Controllers.WeatherForecastController).Assembly);
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddSingleton<IWeatherForecastService, Backend.WeatherForecastService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.Map("/serverside", ssb =>
            {
                ssb.UseExceptionHandler("/Error");
                ssb.UseStaticFiles();
                ssb.UseRouting();
                ssb.UseEndpoints(endpoints =>
                    {
                        endpoints.MapBlazorHub();
                        endpoints.MapFallbackToPage("/_Host");
                    });
                }
            );

            app.Map("/wasm", wasm =>
            {
                wasm.UseRewriter(new RewriteOptions().AddRedirect(@"^$", "/home", 301));

                wasm.UseBlazorFrameworkFiles();
                wasm.UseStaticFiles();

                wasm.UseRouting();

                wasm.UseEndpoints(endpoints =>
                {
                    endpoints.MapFallbackToFile("index.html");
                });
            });

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CounterHub>("/api/hubs/counter");
                endpoints.MapFallbackToFile("choose.html");
            });
        }
}
}
