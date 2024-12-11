# Changelog

All notable changes to this package will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/) and this package adheres to [Semantic Versioning](https://semver.org/)

## [3.2.1]
### Updated
- Updated dependencies.
- Updated documentation.

## [3.2.0]
### Updated
- Updated dependencies to editor utilities, publishing tools and runtime utilities.

## [3.1.1]
### Updated
- Updated dependencies to editor utilities, publishing tools and runtime utilities.

## [3.1.0] - 2021-12-31
### Added
- Added Examples 

### Updated
- Updated publishing tools dependency to 2.0.2
- Updated runtime utilities dependency to 1.1.1

## [3.0.0] - 2021-12-03
### Updated
- Updated editor utilities dependency to 3.0.0
- Updated runtime utilities dependency to 1.1.0
- InternetStatusHandler is now a static class called InternetStatusManager
- The package now supports multiple ping targets
  - The editor window is updated to support this.
  - The public API is updated to support this.

## [2.0.0] - 2021-11-23
### Updated
- Updated editor utilities dependency to 2.0.0
- Updated runtime utilities dependency to 1.0.0
- Updated singleton core dependency to 6.0.0
- Updated publishing tools dependency to 1.0.0

## [1.1.0] - 2021-11-15
### Added
- Added a readme describing the basic setup of the package.
- Added an internals exposer for the test scripts.

### Changed
- Changed editor code to make use of internal access modifiers.
- Made code improvements according to the UCP conventions.

### Fixed
- Connection status not updating when opening the window from playmode.