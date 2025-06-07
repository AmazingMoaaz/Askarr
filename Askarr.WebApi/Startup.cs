using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using Askarr.WebApi.config;
using Askarr.WebApi.AskarrBot.ChatClients.Discord;
using Askarr.WebApi.AskarrBot.DownloadClients.Ombi;
using Askarr.WebApi.AskarrBot.DownloadClients.Radarr;
using Askarr.WebApi.AskarrBot.DownloadClients.Sonarr;
using Askarr.WebApi.AskarrBot;
using Askarr.WebApi.AskarrBot.TvShows;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Askarr.WebApi.AskarrBot.DownloadClients.Overseerr;
using Microsoft.AspNetCore.Authentication;
using Askarr.WebApi.Controllers.Configuration;
using Askarr.WebApi.Controllers.DownloadClients;
using Askarr.WebApi.Controllers.ChatClients;
using Askarr.WebApi.Controllers.Authentication;
using Askarr.WebApi.AskarrBot.Movies;
using Askarr.WebApi.AskarrBot.Locale;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http.Features;
using Askarr.WebApi.AskarrBot.DownloadClients.Lidarr;
using Askarr.WebApi.AskarrBot.Music;
using Askarr.WebApi.AskarrBot.ChatClients.Telegram;
using Askarr.WebApi.AskarrBot.Notifications.Movies;
using Askarr.WebApi.AskarrBot.Notifications.TvShows;
using Askarr.WebApi.AskarrBot.Notifications.Music;

namespace Askarr.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Build configuration with environment variables
            var builder = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddEnvironmentVariables(prefix: "ASKARR_");
                
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public IFeatureCollection ServerFeatures => throw new NotImplementedException();

        public IServiceProvider Services => throw new NotImplementedException();

        private ChatBot _AskarrBot;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine($"Starting  Askarr - build '{(string.IsNullOrWhiteSpace(Language.BuildVersion) ? "Unknown" : Language.BuildVersion)}'");

            services.AddControllersWithViews();
            services.AddHttpClient();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = Program.CombindPath("ClientApp/build");
            });

            var authenticationSettings = Configuration.GetSection("Authentication");
            var applicationSettings = Configuration.Get<ApplicationSettings>();

            // Override settings with environment variables if set
            var envPort = Configuration["PORT"];
            if (!string.IsNullOrEmpty(envPort) && int.TryParse(envPort, out int port))
            {
                applicationSettings.Port = port;
            }

            var envBaseUrl = Configuration["BASE_URL"];
            if (!string.IsNullOrEmpty(envBaseUrl))
            {
                applicationSettings.BaseUrl = envBaseUrl;
            }

            var envDisableAuth = Configuration["DISABLE_AUTHENTICATION"];
            if (!string.IsNullOrEmpty(envDisableAuth) && bool.TryParse(envDisableAuth, out bool disableAuth))
            {
                applicationSettings.DisableAuthentication = disableAuth;
            }

            if (applicationSettings.DisableAuthentication)
            {
                services.AddAuthentication("Disabled")
                    .AddScheme<AuthenticationSchemeOptions, DisabledAuthenticationHandler>("Disabled", options => { });
            }
            else
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = " Askarr",
                            ValidAudience = " Askarr",
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.GetValue<string>("PrivateKey"))),
                        };
                    });
            }

            services.AddSingleton<OmbiClient, OmbiClient>();
            services.AddSingleton<RadarrClient, RadarrClient>();
            services.AddSingleton<SonarrClient, SonarrClient>();
            services.AddSingleton<LidarrClient, LidarrClient>();
            services.AddSingleton<Askarr.WebApi.AskarrBot.DownloadClients.Overseerr.OverseerrClient, Askarr.WebApi.AskarrBot.DownloadClients.Overseerr.OverseerrClient>();
            services.AddSingleton<DiscordSettingsProvider>();
            services.AddSingleton<TvShowsSettingsProvider>();
            services.AddSingleton<MoviesSettingsProvider>();
            services.AddSingleton<MusicSettingsProvider>();
            services.AddSingleton<OmbiSettingsProvider>();
            services.AddSingleton<OverseerrSettingsProvider>();
            services.AddSingleton<BotClientSettingsProvider>();
            services.AddSingleton<ChatClientsSettingsProvider>();
            services.AddSingleton<DownloadClientsSettingsProvider>();
            services.AddSingleton<ApplicationSettingsProvider>();
            services.AddSingleton<AuthenticationSettingsProvider>();
            services.AddSingleton<RadarrSettingsProvider>();
            services.AddSingleton<SonarrSettingsProvider>();
            services.AddSingleton<LidarrSettingsProvider>();
            services.AddSingleton<TelegramSettingsProvider>();
            services.AddSingleton<Askarr.WebApi.AskarrBot.Notifications.Movies.MovieNotificationsRepository>();
            services.AddSingleton<Askarr.WebApi.AskarrBot.Notifications.TvShows.TvShowNotificationsRepository>();
            services.AddSingleton<Askarr.WebApi.AskarrBot.Notifications.Music.MusicNotificationsRepository>();
            services.AddSingleton<Askarr.WebApi.AskarrBot.Movies.MovieWorkflowFactory>();
            services.AddSingleton<Askarr.WebApi.AskarrBot.TvShows.TvShowWorkflowFactory>();
            services.AddSingleton<Askarr.WebApi.AskarrBot.Music.MusicWorkflowFactory>();
            services.AddSingleton<AskarrBot.ChatBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "ClientApp/build")),
                RequestPath = !string.IsNullOrWhiteSpace(Program.BaseUrl) ? Program.BaseUrl : string.Empty
            });

            if (!string.IsNullOrWhiteSpace(Program.BaseUrl))
            {
                app.UsePathBase(Program.BaseUrl);
            }

            app.UseRouting();

            app.UseSpaStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "ClientApp/build/static")),
                RequestPath = "/static"
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            _AskarrBot = (AskarrBot.ChatBot)app.ApplicationServices.GetService(typeof(AskarrBot.ChatBot));
            _AskarrBot.Start();
        }
    }
}