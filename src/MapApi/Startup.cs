using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MapApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connString = Configuration.GetSection("Database").GetValue<string>("ConnectionString");
            var store = MartenHelper.GetStore(connString);

            // For debugging
            MartenHelper.Clean(store);
            MartenHelper.Init(store);

            var hub = new MessageHub();
            var service = new MapService(hub, store);
            var hsl = new HslLocation(service);

            // We don't mind if background process is killed. Should replace with e.g. Hangfire
            Task.Run(async () =>
            {
                while (true)
                {
                    await hsl.Update();
                    await Task.Delay(1000 * 15);
                }
            });

            // Add framework services.
            services.AddMvc();

            services.AddSingleton<MessageHub>(hub);
            services.AddSingleton<WebSocketMiddleware>();
            services.AddSingleton<HslLocation>(hsl);
            services.AddTransient<MapService, MapService>();

            // http://jasperfx.github.io/marten/getting_started/
            // Only one IDocumentStore per application and IDocumentSession per transaction (typically http request)
            services.AddSingleton<IDocumentStore>(store);
            services.AddScoped<IDocumentSession>(provider => ((IDocumentStore)provider.GetService(typeof(IDocumentStore))).LightweightSession());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var socketMiddleware = app.ApplicationServices.GetService<WebSocketMiddleware>();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Serve default static files from wwwroot
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.Map("/ws", (a) =>
            {
                a.UseWebSockets();
                a.Use(socketMiddleware.Handle);
            });

            app.UseMvc();
        }
    }
}
