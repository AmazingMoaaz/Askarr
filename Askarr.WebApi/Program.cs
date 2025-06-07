using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using  Askarr.WebApi. AskarrBot;
using  Askarr.WebApi. AskarrBot.Locale;

namespace  Askarr.WebApi
{
    public class Program
    {
        public static int Port = 4545;
        public static string BaseUrl = string.Empty;

        public static void Main(string[] args)
        {
            string cliBaseUrl = null;
            int cliPort = -1;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--help":
                    case "-h":
                        Console.WriteLine($" Askarr version: {Language.BuildVersion}");
                        Console.WriteLine("Description:");
                        Console.WriteLine("  A chatbot used to connectservices like Sonarr/Radarr/Overseerr/Ombi to Discord\n");
                        Console.WriteLine("Options:");
                        Console.WriteLine("  -h, --help           Displays the help message and exits the program");
                        Console.WriteLine("  -c, --config-dir     Change the config folder");
                        Console.WriteLine("                       Example:  Askarr.WebApi.exe -c \"C:\\ Askarr\\config\"");
                        Console.WriteLine("                                 Askarr.WebApi -c /opt/ Askarr/config");
                        Console.WriteLine("                                 Askarr.WebApi.exe -c ./config");
                        Console.WriteLine("  -p, --port           Change the port of  Askarr, this will update the config file");
                        Console.WriteLine("                       This allows for the changing of the port used for  Askarr, eg: http://localhost:port");
                        Console.WriteLine("                       Example:  Askarr.WebApi.exe -p 4546");
                        Console.WriteLine("                                 Askarr.WebApi --port 4547");
                        Console.WriteLine("  -u, --base-url       Change the base URL of  Askarr, this will update the config file");
                        Console.WriteLine("                       This allows the changing of the base URL to access  Askarr, eg: http://localhost:4545/baseURL");
                        Console.WriteLine("                       Example:  Askarr.WebApi.exe -u \"/ Askarr\"");
                        Console.WriteLine("                                 Askarr.WebApi --base-url \"/\"");
                        Console.WriteLine("                                 Askarr.WebApi.exe -u \"\"");
                        return;
                    case "--config-dir":
                    case "-c":
                        try
                        {
                            SettingsFile.SettingsFolder = args[++i];
                            SettingsFile.CommandLineSettings = true;
                        }
                        catch
                        {
                            Console.WriteLine("Error: Missing argument, config director path missing");
                            return;
                        }
                        break;
                    case "--port":
                    case "-p":
                        try
                        {
                            cliPort = int.Parse(args[++i]);
                            if (cliPort < 0 || cliPort > 65535)
                                throw new Exception("Invalid port number");
                        }
                        catch
                        {
                            Console.WriteLine("Error: Missing argument, port needs to include a number between 0 to 65535");
                            return;
                        }
                        break;
                    case "--base-url":
                    case "-u":
                        try
                        {
                            cliBaseUrl = args[++i];
                            if (cliBaseUrl == "/")
                                cliBaseUrl = string.Empty;
                            else if (cliBaseUrl[cliBaseUrl.Length - 1] == '/')
                                throw new Exception("End slash");
                        }
                        catch (Exception ex)
                        when (ex.Message == "End slash")
                        {
                            Console.WriteLine("Error: Base URL cannot end in a slash '/'");
                            return;
                        }
                        catch
                        {
                            Console.WriteLine("Error: Missing argument, URL is missing");
                            return;
                        }
                        break;
                }
            }

            // Check environment variables for configuration
            string envPort = Environment.GetEnvironmentVariable("ASKARR_PORT");
            string envBaseUrl = Environment.GetEnvironmentVariable("ASKARR_BASE_URL");
            string envConfigDir = Environment.GetEnvironmentVariable("ASKARR_CONFIG_DIR");

            // If config directory is set via environment variable and not via command line
            if (!string.IsNullOrEmpty(envConfigDir) && !SettingsFile.CommandLineSettings)
            {
                SettingsFile.SettingsFolder = envConfigDir;
                SettingsFile.CommandLineSettings = true;
                Console.WriteLine($"Using config directory from environment variable: {envConfigDir}");
            }

            // Parse port from environment variable if set and not specified via command line
            if (!string.IsNullOrEmpty(envPort) && cliPort == -1)
            {
                if (int.TryParse(envPort, out int parsedPort))
                {
                    if (parsedPort >= 0 && parsedPort <= 65535)
                    {
                        cliPort = parsedPort;
                        Console.WriteLine($"Using port from environment variable: {parsedPort}");
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Invalid port in environment variable (must be 0-65535): {envPort}");
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Could not parse port from environment variable: {envPort}");
                }
            }

            // Use base URL from environment variable if set and not specified via command line
            if (!string.IsNullOrEmpty(envBaseUrl) && cliBaseUrl == null)
            {
                if (envBaseUrl == "/")
                {
                    cliBaseUrl = string.Empty;
                }
                else if (envBaseUrl.EndsWith("/"))
                {
                    Console.WriteLine($"Warning: Base URL in environment variable ends with a slash, removing: {envBaseUrl}");
                    cliBaseUrl = envBaseUrl.TrimEnd('/');
                }
                else
                {
                    cliBaseUrl = envBaseUrl;
                }
                Console.WriteLine($"Using base URL from environment variable: {cliBaseUrl}");
            }

            try
            {
                if (!SettingsFile.CommandLineSettings)
                {
                    var config = new ConfigurationBuilder()
#if DEBUG
                        .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
#else
                        .AddJsonFile(CombindPath("appsettings.json"), optional: false, reloadOnChange: true)
#endif
                        .Build();
                    SettingsFile.SettingsFolder = config.GetValue<string>("ConfigFolder");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading config folder location.  Using default location.");
                Console.WriteLine(e.Message);
                SettingsFile.SettingsFolder = "./config";
            }

            UpdateSettingsFile();
            SetLanguage();

            if (cliPort != -1 && cliPort != (int)SettingsFile.Read().Port)
            {
                Console.WriteLine("Changing port from cli arguments...");
                SettingsFile.Write(settings =>
                {
                    settings.Port = cliPort;
                });
            }

            if (cliBaseUrl != null && cliBaseUrl != SettingsFile.Read().BaseUrl)
            {
                Console.WriteLine("Changing base url from cli arguments...");
                SettingsFile.Write(settings =>
                {
                    settings.BaseUrl = cliBaseUrl;
                });
            }

            Port = (int)SettingsFile.Read().Port;
            BaseUrl = SettingsFile.Read().BaseUrl;

            CreateWebHostBuilder(args).Build().Run();
        }

        private static void UpdateSettingsFile()
        {
            if (!Directory.Exists(SettingsFile.SettingsFolder))
            {
                Console.WriteLine("No config folder found, creating one...");
                Directory.CreateDirectory(SettingsFile.SettingsFolder);
            }

            try
            {
                if (!File.Exists(SettingsFile.FilePath))
                {
                    File.WriteAllText(SettingsFile.FilePath, File.ReadAllText(CombindPath("SettingsTemplate.json")).Replace("[PRIVATEKEY]", Guid.NewGuid().ToString()));
                }
                else
                {
                    SettingsFileUpgrader.Upgrade(SettingsFile.FilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to config folder: {ex.Message}");
                throw new Exception($"No config file to load and cannot create one.  Bot cannot start.");
            }


            if (!File.Exists(NotificationsFile.FilePath))
            {
                File.WriteAllText(NotificationsFile.FilePath, File.ReadAllText(CombindPath("NotificationsTemplate.json")));
            }
        }


        /// <summary>
        /// Combinds the pasted in path and connects to the location of the executable
        /// and returns the full path for the directory.
        /// </summary>
        /// <param name="path">String of the path relitive to the executable</param>
        /// <returns>Returns the full path to the file/directory</returns>
        public static string CombindPath(string path)
        {
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path));
        }

        private static void SetLanguage()
        {
            string path = CombindPath($"locales/{SettingsFile.Read().ChatClients.Language}.json");
            Language.Current = JsonConvert.DeserializeObject<Language>(File.ReadAllText(path));
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
#if !DEBUG
                //Used to link local files relitive to the executable, not the executed directory of the user.
                .UseContentRoot(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
#endif
                .UseUrls($"http://*:{Port}")
                .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile(SettingsFile.FilePath, optional: false, reloadOnChange: true);
            });
    }
}
