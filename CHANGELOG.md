# Changelog

All notable changes to Askarr will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.5.5] - 2025-11-02

### 🎨 UI/UX Enhancement - Complete Form & Interface Overhaul
- **Fixed Duplicate Labels**: Removed all duplicate label issues across the application
  - Language dropdown no longer shows label twice
  - Season Restrictions dropdown cleaned up
  - All form groups properly structured
  
- **Enhanced Version Display**: 
  - Version now shows correctly with fallback ("Askarr v2.5.5")
  - Beautiful gradient badge with teal/blue colors
  - Update notification with styled download button
  - Proper spacing and hover effects

- **Comprehensive Form Styling**: New form enhancement stylesheet
  - All inputs now have teal borders matching logo
  - Smooth focus states with glowing outline
  - Enhanced hover effects on all form elements
  - Consistent border radius (0.75rem) throughout
  
- **Button Enhancements**:
  - All buttons now use logo gradient colors
  - Smooth hover animations with lift effect
  - Disabled state styling
  - Focus states with glow effect
  
- **Card & Tab Improvements**:
  - Cards have subtle teal borders and enhanced shadows
  - Tab navigation with gradient underline animation
  - Consistent rounded corners throughout
  
- **Input Group Styling**:
  - Input group addons match teal theme
  - Icons colored with logo teal
  - Smooth transitions on focus
  
- **Alert & Badge Updates**:
  - All alerts use logo color scheme
  - Gradient backgrounds for badges
  - Better contrast and readability

- **Help Text Consistency**:
  - Uniform styling for all form help text
  - Proper color and spacing
  - Better readability

### 🎯 User Experience
- More cohesive visual language
- Better feedback on interactions
- Smoother animations throughout
- Enhanced accessibility with better focus states

## [2.5.4] - 2025-11-02

### 🎨 UI Improvements - Dropdown Enhancement
- **Completely Redesigned Dropdowns**: All dropdown menus now match logo color scheme
  - Teal (#4FD1C5) borders and hover effects
  - Blue (#63B3ED) gradient on selected items
  - Glassmorphic dropdown panels with backdrop blur
  - Custom scrollbar with gradient styling
- **Fixed Duplicate Labels**: Removed redundant label repetition in dropdowns
- **Enhanced Interactions**:
  - Smooth hover animations with translateX effect
  - Focus states with glowing teal outline
  - Better padding and spacing
  - Improved placeholder text
- **Better UX**:
  - Clearer visual feedback on selection
  - More intuitive dropdown handle with icon background
  - Improved readability with darker text
  - Maximum height with smooth scrolling

## [2.5.3] - 2025-11-02

### 🎨 Visual Redesign - Logo Color Integration
- **Complete UI Color Scheme Overhaul**: Redesigned entire application to match logo colors
  - Primary color: **#4FD1C5** (Teal/Turquoise) - from logo's main "A" accent
  - Secondary color: **#63B3ED** (Blue) - from logo's chat bubble
  - Accent color: **#F6E05E** (Yellow/Gold) - from logo's signal waves
  - Dark theme: **#2D3748** (Dark Gray-Blue) - from logo's background circle
- Updated all gradients, buttons, badges, and UI components with new color palette
- Enhanced Login & Register pages with glassmorphic design matching new colors
- Applied consistent color theming across all pages and components:
  - Chat Clients page
  - Movies, TV Shows, Music pages
  - Account & Settings pages
  - Modern headers and cards
- Updated success/error/warning/info states to use new color scheme
- Refined background gradients with logo-inspired overlay effects
- Enhanced all hover states and animations with teal/blue accents

### 🔧 Improvements
- Better color contrast and accessibility
- More cohesive brand identity throughout the application
- Modernized glassmorphism effects with vibrant teal/blue gradients
- Updated all stat cards, client cards, and form elements with new colors
- Version display now properly shows current version number

## [2.5.2] - 2025-11-02

### 🔧 Critical Fixes

#### Settings Persistence
- **FIXED**: Settings being cleared after Docker container updates
- **NEW**: Added VOLUME directives for `/root/config` and `/root/tmp`
- **NEW**: docker-compose.yml example with proper volume mounts
- **NEW**: docker-run-example.sh for easy deployment
- Settings now persist across container recreations

#### Version Checker
- **FIXED**: Version not displaying in sidebar
- **FIXED**: VersionActions now dispatches fallback data on API errors
- Sidebar always shows version number (even when offline)
- Update notifications work reliably

### 🎨 UI/UX Improvements

#### Consistent Modern Design
- **UPDATED**: Account page now uses ModernHeader with glassmorphism
- **UPDATED**: Settings page now uses ModernHeader with glassmorphism
- **IMPROVED**: All pages now have consistent epic gradient headers
- **IMPROVED**: Unified glassmorphic card styling across all views

### 🤖 Discord Commands

#### Troubleshooting Guide
- **NEW**: Comprehensive Discord commands troubleshooting documentation
- Explains command propagation delays (5-60 minutes)
- Provides step-by-step resolution guide
- Documents common issues and solutions

### 📦 Docker Improvements

#### Volume Management
- Config folder now persists via Docker volumes
- Temporary files folder properly volumized
- Version file location fixed (now in /root/)
- Updated Docker labels with correct GitHub URLs

### 🚀 Deployment

#### New Files
- `docker-compose.yml` - Ready-to-use compose file
- `docker-run-example.sh` - Example Docker run command
- Settings persist automatically with volume mounts

---

## [2.5.1] - 2025-11-02

### 🐛 Bug Fixes

#### Docker Build Fix
- **FIXED**: Docker build failing with "Cannot find module 'node:path'" error
- **FIXED**: Node.js version mismatch in Dockerfile
- **CHANGED**: Updated Dockerfile to install Node.js 18.x from NodeSource
  - Previously was using Node.js v12 from apt-get
  - React dependencies require Node >= 14, some require >= 18
- **RESOLVED**: All EBADENGINE warnings during npm install
- **RESOLVED**: ESLint compilation errors in Docker builds

#### Technical Details
- Replaced old `apt-get install nodejs npm` with proper Node.js 18.x installation
- Used official NodeSource repository for Node.js
- Ensures compatibility with all React Scripts, Sass, Tailwind, and other modern dependencies

### 🚀 Deployment
- Docker builds now complete successfully
- All GitHub Actions workflows will now work correctly

---

## [2.5.0] - 2025-11-02

### 🎨 Major UI/UX Transformation

#### Epic Visual Redesign
- **NEW**: Stunning purple gradient background with animated particle effects
- **NEW**: Glassmorphism design language throughout the application
- **NEW**: Modern card system with frosted glass effects and backdrop blur
- **NEW**: 3D depth effects with multi-layer shadows
- **NEW**: Smooth cubic-bezier animations for premium feel

#### Enhanced Components

**ClientCard Component:**
- **NEW**: 75px gradient icons with custom colors per client type
- **NEW**: Shine animation effect that sweeps across icons
- **NEW**: Animated gradient border for active state
- **NEW**: Gradient text titles with purple-to-violet effect
- **NEW**: Enhanced success badge with gradient and glow
- **IMPROVED**: Hover effects with 10px lift and subtle rotation

**StatsCard Component:**
- **NEW**: 56px rounded gradient icon containers
- **NEW**: Gradient value text for all statistics
- **NEW**: Rotating and scaling icon animations on hover
- **NEW**: Radial gradient overlay effects
- **IMPROVED**: 3D card transforms with enhanced shadows

**ModernHeader Component:**
- **NEW**: 80px glassmorphic icon container with backdrop blur
- **NEW**: Interactive icon with rotation and scale on hover
- **NEW**: Gradient text effect on page titles
- **NEW**: Staggered fade-in animations for title and subtitle
- **IMPROVED**: Enhanced text shadows for better depth and readability

#### Modern Button Styles
- **NEW**: Gradient backgrounds for all button types (Primary, Info, Success, Danger, Warning)
- **NEW**: Ripple effect animation on click
- **NEW**: 3D transform effects (scale and lift)
- **NEW**: Color-matched glowing shadows
- **IMPROVED**: Enhanced hover states with smooth transitions

#### New Animation System
- **NEW**: `fadeInUp` - Elements slide up while fading in
- **NEW**: `fadeInDown` - Elements slide down while fading in
- **NEW**: `pulse` - Gentle breathing effect for emphasis
- **NEW**: `float` - Floating motion for subtle movement
- **NEW**: `glow` - Pulsing shadow glow for buttons and badges
- **NEW**: `shine` - Light sweep animation across elements

#### Utility Classes
- **NEW**: `.glass-effect` - Apply glassmorphism to any element
- **NEW**: `.gradient-text` - Gradient text color effect
- **NEW**: `.gradient-border` - Animated gradient border
- **NEW**: `.hover-lift` - Standard hover lift effect
- **NEW**: `.pulse-animation` - Continuous pulse effect
- **NEW**: `.float-animation` - Continuous float effect
- **NEW**: `.badge-gradient-*` - Gradient badge variants

#### Color System
- **NEW**: Epic gradient palette for all UI states:
  - Primary: Purple to Violet (`#667eea` → `#764ba2`)
  - Success: Green to Cyan (`#2dce89` → `#2dcecc`)
  - Info: Cyan to Blue (`#11cdef` → `#1171ef`)
  - Danger: Red to Orange-Red (`#f5365c` → `#f56036`)
  - Warning: Orange to Yellow (`#fb6340` → `#fbb140`)

#### Responsive Design
- **IMPROVED**: Mobile-optimized with adaptive sizing
- **IMPROVED**: Touch-friendly hover states
- **IMPROVED**: Smooth transitions across all breakpoints
- **IMPROVED**: Adaptive spacing and padding for smaller screens

### 🤖 Discord Bot Enhancements

#### Command Structure Changes
- **CHANGED**: Removed command group wrapper for cleaner commands
  - Commands are now `/movie`, `/tv`, `/music` instead of `/request movie`, etc.
  - Provides more intuitive command structure
  - Matches original bot behavior

#### Enhanced Help Message
- **IMPROVED**: Better formatted help message with examples
- **NEW**: Shows command examples: `/movie Deadpool 2`, `/tv Breaking Bad`
- **IMPROVED**: Dynamic command list based on enabled features
- **IMPROVED**: Better visual formatting with bold text and separators
- **IMPROVED**: Clearer issue reporting instructions

### 🎯 Chat Clients Page Enhancements
- **IMPROVED**: Client selection cards now feature gradient icons with shine effects
- **IMPROVED**: Active state cards display animated gradient borders
- **IMPROVED**: Tab interface with modern styling
- **IMPROVED**: Form inputs with enhanced focus states
- **IMPROVED**: All buttons now use epic gradient styles

### 🔧 Technical Changes

#### Modified Files
- `modern-layout.scss` - Complete redesign with glassmorphism and gradients
- `ModernHeader.jsx` - Enhanced with glassmorphic icon and animations
- `ClientCard.jsx` - Completely redesigned with gradient effects
- `StatsCard.jsx` - Modernized with gradient values and animations
- `SlashCommands.txt` - Removed command group wrapper for root-level commands
- `english.json` - Enhanced help message format with examples

### 📱 Browser Compatibility
- **MAINTAINED**: Full support for modern browsers
- **MAINTAINED**: Fallback styles for older browsers
- **NEW**: Enhanced backdrop-filter support detection

### 🚀 Performance
- **OPTIMIZED**: CSS animations use GPU acceleration
- **OPTIMIZED**: Transform and opacity animations for smooth 60fps
- **OPTIMIZED**: Efficient cubic-bezier easing functions

---

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

#### Modified Components
- `ChatBot.cs` - Fixed dual platform startup logic
- `TelegramBot.cs` - Enhanced help system with inline keyboards
- `Sidebar.jsx` - Dynamic version display and resource links
- `index.js` - Added VersionReducer to Redux store

### 📚 Documentation
- **NEW**: Comprehensive CHANGELOG.md
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
- `X.Y.Z`: Specific version (e.g., `2.5.0`)
- `X.Y`: Major.Minor version (e.g., `2.5`)

---

## Links

- [GitHub Repository](https://github.com/AmazingMoaaz/Askarr)
- [Releases](https://github.com/AmazingMoaaz/Askarr/releases)
- [Issues](https://github.com/AmazingMoaaz/Askarr/issues)
- [Wiki](https://github.com/AmazingMoaaz/Askarr/wiki)
- [Docker Hub](https://hub.docker.com/r/amazingmoaaz/askarr)
