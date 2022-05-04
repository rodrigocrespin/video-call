using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Serilog;
using VideoCall.Api.Config;
using VideoCall.Application;

namespace VideoCall.Api
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<Startup> logger;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;

            var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());
            logger = loggerFactory.CreateLogger<Startup>();

            logger.LogInformation("Version " +
                                  Assembly.GetEntryAssembly()!
                                      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                                      .InformationalVersion);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.SetupCors(configuration, logger);

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddHealthChecks();

            services.AddOpenApiDocument(cfg => cfg.PostProcess = d => d.Info.Title = "Video Call Api");

            services.Configure<TwilioConfig>(configuration.GetSection(TwilioConfig.ConfigKey));

            services.AddScoped<IRoomMapper, RoomMapper>();
            services.AddScoped<IVideoCallService, VideoCallService>();

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();

            app.UseRouting();

            app.UseOpenApi();

            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                // The readiness check uses all registered checks with the 'ready' tag.
                endpoints.MapHealthChecks("/health/ready",
                    new HealthCheckOptions() { Predicate = (check) => check.Tags.Contains("ready"), });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions()
                {
                    // Exclude all checks and return a 200-Ok.
                    Predicate = (_) => false
                });

                endpoints.MapHub<NotificationHub>("/notificationHub");
            });
        }
    }
}
