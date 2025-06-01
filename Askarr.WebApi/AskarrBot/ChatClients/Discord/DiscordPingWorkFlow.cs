﻿using System;
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

        public DiscordPingWorkFlow(InteractionContext context)
        {
            _context = context;
        }

        public async Task HandlePingAsync() 
        {
            await _context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral(true).WithContent(Language.Current.DiscordCommandPingResponse));
        }
    }
}