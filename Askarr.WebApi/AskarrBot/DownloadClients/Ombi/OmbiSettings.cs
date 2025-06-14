namespace Askarr.WebApi.AskarrBot.DownloadClients.Ombi
{
    public class OmbiSettings
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string ApiKey { get; set; }
        public string ApiUsername { get; set; }
        public string BaseUrl { get; set; }
        public string Version { get; set; }

        public bool UseMovieIssue { get; set; }
        public bool UseTVIssue { get; set; }
    }
}