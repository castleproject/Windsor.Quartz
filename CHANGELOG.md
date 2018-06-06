# Castle.Facilities.Quartz Changelog

## 1.0.0
- Quartz Facility supports now Quartz.NET 3.0.4
- C# API added
- BREAKING CHANGE: 
	- Quartz.config is not supported anymore: listeners can be attached through the C# API. Quartz configuration properties can be attached in the app.config.
	- The IJobScheduler interface dissappeared and its QuartzNetSimpleScheduler implementation. The Scheduler class replaces the QuartzNetScheduler class and also implements the Quartz' IScheduler interface
	

## 0.5.0
- Compatible with .NET Standard 2.0
- Quartz Facility supports now Quartz.NET 3.0.2
- BREAKING CHANGE: namespace is changed into Castle.Facilities.Quartz. Code remains the same