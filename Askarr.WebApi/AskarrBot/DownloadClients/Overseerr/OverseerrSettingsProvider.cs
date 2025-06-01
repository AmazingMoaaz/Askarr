﻿namespace Askarr.WebApi.AskarrBot.DownloadClients.Overseerr
{
    public class OverseerrSettingsProvider
    {
        public OverseerrSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new OverseerrSettings
            {
                ApiKey = settings.DownloadClients.Overseerr.ApiKey,
                Movies = settings.DownloadClients.Overseerr.Movies.ToObject<OverseerrMovieSettings>(),
                TvShows = settings.DownloadClients.Overseerr.TvShows.ToObject<OverseerrTvShowSettings>(),
                Hostname = settings.DownloadClients.Overseerr.Hostname,
                Port = settings.DownloadClients.Overseerr.Port,
                UseSSL = (bool)settings.DownloadClients.Overseerr.UseSSL,
                UseMovieIssue = (bool)settings.DownloadClients.Overseerr.UseMovieIssue,
                UseTVIssue = (bool)settings.DownloadClients.Overseerr.UseTVIssue,
                Version = settings.DownloadClients.Overseerr.Version,
            };
        }
    }
}