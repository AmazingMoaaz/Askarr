namespace Askarr.WebApi.AskarrBot.Movies
{
    public class MoviesSettingsProvider
    {
        public MoviesSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new MoviesSettings
            {
                Client = settings.Movies.Client,
            };
        }
    }
}