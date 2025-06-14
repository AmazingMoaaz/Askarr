﻿namespace Askarr.WebApi.AskarrBot.Music
{
    public class MusicSettingsProvider
    {
        public MusicSettings Provide()
        {
            dynamic settings = SettingsFile.Read();

            return new MusicSettings
            {
                Client = settings.Music.Client
            };
        }
    }
}
