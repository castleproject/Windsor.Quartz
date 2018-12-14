# Castle Facility for Quartz.NET Changelog

## Unreleased

Breaking Changes:
- Removed `quartz.config` support, listeners can be attached through the C# API. Quartz configuration properties can be attached in the `app.config`.
- The `IJobScheduler` interface and its `QuartzNetSimpleScheduler` implementation was removed. The `Scheduler` class replaces the `QuartzNetScheduler` class and also implements the Quartz `IScheduler` interface

Enhancements:
- Added C# API
- Upgraded to Quartz.NET 3.0.4

## 0.5.0 (2018-02-26)

Breaking Changes:
- Changed namespace to `Castle.Facilities.Quartz`

Enhancements:
- Added .NET Standard 2.0 support
- Upgraded to Quartz.NET 3.0.2
