namespace Askarr.WebApi.AskarrBot.TvShows
{

    public class TvShowsSettingsProvider
    {
        public TvShowsSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new TvShowsSettings
            {
                Client = settings.TvShows.Client,
                Restrictions = settings.TvShows.Restrictions,
            };
        }
    }
}