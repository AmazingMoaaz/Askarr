using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Askarr.WebApi.AskarrBot.Locale;

namespace Askarr.WebApi.AskarrBot.ChatClients.Discord
{
    public class DiscordPingWorkFlow
    {
        private readonly InteractionContext _context;
        private readonly DiscordSettings _settings;

        public DiscordPingWorkFlow(InteractionContext context, DiscordSettingsProvider settingsProvider = null)
        {
            _context = context;
            _settings = settingsProvider?.Provide() ?? new DiscordSettings { AutomaticallyPurgeCommandMessages = false };
        }

        public async Task HandlePingAsync() 
        {
            await _context.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource, 
                new DiscordInteractionResponseBuilder()
                    .AsEphemeral(_settings.UsePrivateResponses)
                    .WithContent(Language.Current.DiscordCommandPingResponse));
        }
    }
}