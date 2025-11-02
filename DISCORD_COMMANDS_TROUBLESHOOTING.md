# Discord Commands Troubleshooting

## Commands Not Showing Up?

If your Discord slash commands (`/movie`, `/tv`, `/music`) aren't working, follow these steps:

### 1. **Restart the Bot**
Stop and start the Askarr bot completely. Discord needs to re-register commands after code changes.

```bash
# If using Docker:
docker restart askarr

# Or stop and start:
docker stop askarr
docker start askarr
```

### 2. **Wait for Propagation**
Discord can take **5-60 minutes** to propagate slash command changes globally. Be patient!

### 3. **Check Bot Permissions**
Ensure your bot has the `applications.commands` scope in the OAuth2 URL:

```
https://discord.com/api/oauth2/authorize?client_id=YOUR_CLIENT_ID&permissions=8&scope=bot%20applications.commands
```

### 4. **Clear Discord Cache**
Sometimes Discord caches old commands. Try:
- Press `Ctrl + R` (Windows/Linux) or `Cmd + R` (Mac) to reload Discord
- Or fully restart your Discord client

### 5. **Force Re-invite the Bot**
1. Kick the bot from your server
2. Re-invite it using the invite link with `applications.commands` scope
3. Wait a few minutes

### 6. **Check Bot Configuration**
Verify in your Askarr web interface (Chat Clients page):
- Discord is selected as chat client
- Bot token is correct
- Client ID is correct
- At least one movie/TV download client is enabled (Radarr/Sonarr/Overseerr/Ombi)

### 7. **Check Logs**
Look at your bot logs for errors:

```bash
# If using Docker:
docker logs askarr

# Look for lines like:
# "Building slash commands..."
# "Registering global slash commands..." or "Registering guild-specific slash commands..."
# "Refreshing commands..."
# "Discord bot ready!"
```

### 8. **Manual Discord Developer Portal Clear**
If commands still don't work:
1. Go to https://discord.com/developers/applications
2. Select your bot application
3. Go to "OAuth2" → "General"
4. Click "Reset Token" if needed
5. Go to a test server and try `/` to see if commands appear

## Common Issues

### Commands show `/request movie` instead of `/movie`
- Old command structure is cached
- Follow steps 1-5 above
- Can take up to 1 hour for global commands to update

### "This interaction failed"
- Download clients (Radarr/Sonarr) not configured
- Check your Chat Clients settings

### Commands not responding
- Check if download clients are online and accessible
- Verify API keys are correct
- Check bot logs for errors

## Need More Help?

- Check the logs: `docker logs askarr`
- Report issues: https://github.com/AmazingMoaaz/Askarr/issues
- Read the wiki: https://github.com/AmazingMoaaz/Askarr/wiki

