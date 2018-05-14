# Quartz.NET Facility

## Using the C# API

### Configuring the facility using quartz properties (C#)

By using following code sample, you will configure the scheduler with Quartz properties provided by code.
You don't need Quartz properties in the app.config for this way of working.
You are not forced to use the StartableFacility. In case you don't want to use the StartableFacility, you need to call the IScheduler.Start() method to start your scheduler manually.

```csharp
var container = new WindsorContainer();
container.AddFacility<StartableFacility>(f => f.DeferredStart());
container.AddFacility<QuartzFacility>(q =>
	q
		.SetProperties(new Dictionary<string, string>
		{
			{"quartz.scheduler.instanceName", "QuartzSchedulerConfiguredByCode"},
			{"quartz.threadPool.type", "Quartz.Simpl.DefaultThreadPool, Quartz"},
			{"quartz.threadPool.threadCount", "5"}
		})
);
```

### Configuring the facility using quartz properties (app.config)

By using following code sample, you will configure the scheduler with Quartz properties provided in the app.config.
You only need to add the facility to the container. The Quartz properties will be retrieved from the app.config automatically
You are not forced to use the StartableFacility. In case you don't want to use the StartableFacility, you need to call the IScheduler.Start() method to start your scheduler manually.

```csharp
var container = new WindsorContainer();
container.AddFacility<StartableFacility>(f => f.DeferredStart());
container.AddFacility<QuartzFacility>();
```

Add this configuration to the app.config xml file:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="quartz" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.5000.0,Culture=neutral, PublicKeyToken=b77a5c561934e089" />
  </configSections>
  <quartz>
    <add key="quartz.scheduler.instanceName" value="JobScheduler"/>
    <add key="quartz.jobStore.type" value=" Quartz.Simpl.RAMJobStore, Quartz"/>
    <add key="quartz.threadPool.threadCount" value="5"/>
  </quartz>
</configuration>
```


### Attaching job-, trigger- and scheduler-listeners

You can attach job-, trigger- and schedulerlisteners by the C# API
Use following code sample:

```csharp
var container = new WindsorContainer();
container.AddFacility<QuartzFacility>(q =>
	q
		.SetJobListeners(new JobListener(container.Resolve<ISampleJobListener>()))
		.SetTriggerListeners(new TriggerListener(container.Resolve<ISampleTriggerListener>()))
		.SetSchedulerListeners(container.Resolve<ISampleSchedulerListener>())
);
```

The SetJobListeners en SetTriggerListeners-methods have also a second parameter to restrict the execution of the listener for certain jobs or triggers.
```csharp
	q
		.SetJobListeners(new JobListener(Container.Resolve<IJobListener>(), new IMatcher<JobKey>[] { KeyMatcher<JobKey>.KeyEquals(new JobKey("OnlyListenToJob1", "JobGroup")) }))
```



## Configuring Quartz.NET jobs (quartz_jobs.xml)
You can use the XMLSchedulingDataProcessorPlugin to schedule jobs with triggers.

- Create an xml quartz_jobs.xml :

```xml
<?xml version="1.0" encoding="utf-8"?>

<job-scheduling-data xmlns="http://quartznet.sourceforge.net/JobSchedulingData" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" version="2.0">

  <processing-directives>
    <overwrite-existing-data>true</overwrite-existing-data>
  </processing-directives>

  <schedule>
    <job>
      <name>hello-world</name>
      <job-type>Castle.Facilities.Quartz.SampleApp.SampleJob, Castle.Facilities.Quartz.SampleApp</job-type>
      <durable>true</durable>
      <recover>true</recover>
    </job>
    <trigger>
      <simple>
        <name>sample-trigger</name>
        <job-name>hello-world</job-name>
        <start-time>1982-06-28T12:24:00.0Z</start-time>
        <repeat-count>-1</repeat-count>
        <repeat-interval>2000</repeat-interval>
        <!-- 2 seconds -->
      </simple>
    </trigger>
  </schedule>
</job-scheduling-data>
```

- Add following quartz properties to the facility through the C# API or add them in the app.config:
```xml
    <add key="quartz.plugin.xml.type" value="Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins" />
    <add key="quartz.plugin.xml.scanInterval" value="10" />
    <add key="quartz.plugin.xml.fileNames" value="~/quartz_jobs.xml" />
```