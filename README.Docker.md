# Askarr

<div align="center">
  <img src="https://raw.githubusercontent.com/AmazingMoaaz/Askarr/master/Logos/logo.svg" width="200" alt="Askarr Logo">
  
  <h3>Advanced Media Request Management via Chat Integration</h3>
  <p>Enterprise-grade solution for Sonarr, Radarr, Lidarr, Overseerr and Ombi</p>
  
  [![GitHub Stars](https://img.shields.io/github/stars/amazingmoaaz/askarr?style=flat-square&logo=github&color=E3B341&logoColor=white)](https://github.com/AmazingMoaaz/Askarr)
  [![License](https://img.shields.io/github/license/amazingmoaaz/askarr?style=flat-square&color=22B455&logoColor=white)](https://github.com/AmazingMoaaz/Askarr/blob/master/LICENSE)
  [![Version](https://img.shields.io/github/v/release/amazingmoaaz/askarr?style=flat-square&logo=github&color=35B44C&logoColor=white)](https://github.com/AmazingMoaaz/Askarr/releases)
</div>

---

## 🐳 Docker Quick Start

### Pull the Latest Image

```bash
docker pull amazingmoaaz/askarr:latest
```

### Run Askarr

```bash
docker run -d \
  --name askarr \
  -p 4545:4545 \
  -v /path/to/config:/root/config \
  --restart=unless-stopped \
  amazingmoaaz/askarr:latest
```

### Docker Compose

```yaml
version: '3.8'
services:
  askarr:
    image: amazingmoaaz/askarr:latest
    container_name: askarr
    ports:
      - "4545:4545"
    volumes:
      - ./config:/root/config
    restart: unless-stopped
```

Access the web interface at: `http://localhost:4545`

---

## ✨ Features

### Chat Platform Integration

| Platform | Features |
|----------|----------|
| 🎮 **Discord** | Slash commands, Interactive buttons, Rich embeds, Role-based permissions |
| 💬 **Telegram** | Command interface, Inline keyboards, Group & private chat, User permissions |

### Media Server Support

- 📺 **Sonarr** - TV show management (V2-V4)
- 🎬 **Radarr** - Movie management (V2-V5)
- 🎵 **Lidarr** - Music management (V1-V2)
- 🎯 **Overseerr** - Request management with advanced features
- 📋 **Ombi** - V3/V4 compatible with quota management

### Key Capabilities

✅ Multi-platform chat bot support  
✅ Quality profile management  
✅ Automated notifications  
✅ Request tracking and management  
✅ Issue reporting system  
✅ Multi-language support  
✅ Role-based access control  
✅ **NEW in v2.0**: Dynamic version checking with update notifications  
✅ **NEW in v2.0**: Interactive Telegram help menu  

---

## 📦 Available Tags

| Tag | Description |
|-----|-------------|
| `latest` | Latest stable release |
| `2.0.0` | Specific version |
| `2.0` | Major.Minor version |

### Version Selection

```bash
# Latest stable
docker pull amazingmoaaz/askarr:latest

# Specific version
docker pull amazingmoaaz/askarr:2.0.0

# Major.Minor (receives patch updates)
docker pull amazingmoaaz/askarr:2.0
```

---

## ⚙️ Configuration

### Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `ASPNETCORE_URLS` | `http://*:4545` | Server binding URL |

### Volume Mounts

| Path | Description |
|------|-------------|
| `/root/config` | Configuration files and database |

### Ports

| Port | Protocol | Description |
|------|----------|-------------|
| `4545` | TCP | Web interface and API |

---

## 🚀 Setup Guide

### 1. First Run

On first run, Askarr will create the necessary configuration files:

```bash
docker run -d \
  --name askarr \
  -p 4545:4545 \
  -v ./askarr-config:/root/config \
  amazingmoaaz/askarr:latest
```

### 2. Access Web Interface

Navigate to `http://localhost:4545` and complete the setup wizard.

### 3. Configure Chat Clients

**Discord Setup:**
1. Create a bot at [Discord Developer Portal](https://discord.com/developers/applications)
2. Copy Bot Token and Client ID
3. Configure in Askarr web interface
4. Invite bot to your server

**Telegram Setup:**
1. Create a bot with [@BotFather](https://t.me/botfather)
2. Copy the bot token
3. Configure in Askarr web interface
4. Start chatting with your bot

### 4. Connect Media Servers

Configure your Sonarr, Radarr, Lidarr, Overseerr, or Ombi instances in the web interface.

---

## 📊 System Requirements

### Minimum Requirements

- **RAM**: 256 MB
- **CPU**: 1 core
- **Storage**: 100 MB
- **Docker**: 20.10+

### Recommended

- **RAM**: 512 MB+
- **CPU**: 2 cores
- **Storage**: 500 MB+
- **Network**: Stable internet connection

---

## 🔄 Updating

### Pull Latest Version

```bash
# Stop container
docker stop askarr

# Remove old container
docker rm askarr

# Pull latest image
docker pull amazingmoaaz/askarr:latest

# Start new container
docker run -d \
  --name askarr \
  -p 4545:4545 \
  -v /path/to/config:/root/config \
  --restart=unless-stopped \
  amazingmoaaz/askarr:latest
```

### Using Docker Compose

```bash
docker-compose pull
docker-compose up -d
```

---

## 🐛 Troubleshooting

### Check Logs

```bash
docker logs askarr
```

### Follow Logs in Real-time

```bash
docker logs -f askarr
```

### Access Container Shell

```bash
docker exec -it askarr /bin/bash
```

### Common Issues

**Port Already in Use:**
```bash
# Use a different port
docker run -d -p 5000:4545 amazingmoaaz/askarr:latest
```

**Permission Issues:**
```bash
# Check volume permissions
ls -la /path/to/config
```

---

## 🔗 Links

- 📚 [Documentation](https://github.com/AmazingMoaaz/Askarr/wiki)
- 🐛 [Report Issues](https://github.com/AmazingMoaaz/Askarr/issues)
- 💬 [Discussions](https://github.com/AmazingMoaaz/Askarr/discussions)
- 📋 [Changelog](https://github.com/AmazingMoaaz/Askarr/blob/master/CHANGELOG.md)
- 🏠 [GitHub Repository](https://github.com/AmazingMoaaz/Askarr)

---

## 📄 License

Askarr is licensed under the [MIT License](https://github.com/AmazingMoaaz/Askarr/blob/master/LICENSE).

---

## 🙏 Acknowledgements

Built upon the foundation of Requestrr by [@darkalfx](https://github.com/darkalfx) and [@thomst08](https://github.com/thomst08).

---

<div align="center">
  <p>
    <strong>⭐ If you find Askarr useful, please consider giving it a star on GitHub! ⭐</strong>
  </p>
  <p>
    <a href="https://github.com/AmazingMoaaz/Askarr">
      <img src="https://img.shields.io/github/stars/amazingmoaaz/askarr?style=social" alt="GitHub stars">
    </a>
  </p>
</div>

