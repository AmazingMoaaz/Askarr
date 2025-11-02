# Changelog

All notable changes to Askarr will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2025-11-02

### 🎉 Major Features

#### Dynamic Version Management & Update Checking
- **NEW**: Automatic version checking against GitHub releases
- **NEW**: Update notifications in sidebar when new version is available
- **NEW**: One-click download button for latest release
- **NEW**: Version API endpoint (`/api/version`) for version information
- **CHANGED**: Sidebar now shows dynamic version with update status
- **CHANGED**: All resource links now dynamically fetched from GitHub

#### Enhanced Telegram Bot Experience
- **NEW**: Interactive help menu with inline keyboard buttons
- **NEW**: Dynamic help command that adapts to enabled features
- **NEW**: Quick action buttons for common commands (Movie, TV, Music, Status)
- **IMPROVED**: Help message formatting with HTML for better readability
- **IMPROVED**: Contextual help responses via button clicks

### 🐛 Bug Fixes

#### Critical Fixes
- **FIXED**: Telegram bot not starting when "Both" (Discord + Telegram) is selected
  - Changed bot client check from `"Both"` to `"Discord,Telegram"` to match frontend format
  - Both bots now start correctly when "Both" option is selected

### 🎨 UI/UX Improvements

#### Sidebar Enhancements
- **CHANGED**: Removed static support links (Donate, Discord server)
- **CHANGED**: Renamed "Support" section to "Resources"
- **NEW**: Dynamic resource links from GitHub:
  - Wiki (automatically pulled from repository)
  - GitHub repository link
  - Report Issue (GitHub issues)
- **NEW**: Visual update indicator with version comparison
- **IMPROVED**: Better visual hierarchy for version information

#### Chat Clients Interface
- **MAINTAINED**: Modern card-based client selection
- **MAINTAINED**: Clean tabbed interface for platform settings
- **MAINTAINED**: Test buttons for connection validation

### 🔧 Technical Changes

#### New Components
- `VersionController.cs` - Backend API for version management
- `VersionActions.jsx` - Redux actions for version state
- `VersionReducer.jsx` - Redux reducer for version information
- `bump-version.ps1` - PowerShell script for version bumping

#### Modified Components
- `ChatBot.cs` - Fixed dual platform startup logic
- `TelegramBot.cs` - Enhanced help system with inline keyboards
- `Sidebar.jsx` - Dynamic version display and resource links
- `index.js` - Added VersionReducer to Redux store

### 📚 Documentation
- **NEW**: Comprehensive CHANGELOG.md
- **NEW**: Version bumping script with documentation
- **UPDATED**: README with accurate repository information

### 🔄 Breaking Changes
- **CHANGED**: Sidebar resource links are now dynamic (requires internet connection to GitHub)
- **CHANGED**: Version checking occurs on application startup

### 🚀 Deployment Notes
- Both `version.txt` files updated to 2.0.0
- GitHub Actions workflows ready for automated releases
- Docker images will be tagged with v2.0.0

---

## [1.0.6] - Previous Release

### Features
- Discord bot integration with slash commands
- Telegram bot with basic command support
- Sonarr, Radarr, Lidarr integration
- Overseerr and Ombi support
- Multi-language support
- Web-based configuration interface

---

## Version Format

Askarr follows Semantic Versioning (MAJOR.MINOR.PATCH):

- **MAJOR**: Incompatible API changes
- **MINOR**: Backward-compatible functionality additions
- **PATCH**: Backward-compatible bug fixes
- **HOTFIX**: Emergency fixes (format: X.Y.Z-hfN)

### Docker Tags
- `latest`: Latest stable release
- `X.Y.Z`: Specific version (e.g., `2.0.0`)
- `X.Y`: Major.Minor version (e.g., `2.0`)

---

## Links

- [GitHub Repository](https://github.com/AmazingMoaaz/Askarr)
- [Releases](https://github.com/AmazingMoaaz/Askarr/releases)
- [Issues](https://github.com/AmazingMoaaz/Askarr/issues)
- [Wiki](https://github.com/AmazingMoaaz/Askarr/wiki)
- [Docker Hub](https://hub.docker.com/r/amazingmoaaz/askarr)

