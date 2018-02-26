# Quartz.NET Facility Configuration

## Configuring Quartz.NET jobs (quartz_jobs.xml)
Integration is achieved through a Windsor Facility. Currently, the only way to configure this facility is using XML configuration. Jobs are usually registered and configured in Quartz using the quartz_jobs.xml file. For example:

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

## Configuring facility

### Basic configuration

The job class (in this case `SampleJob`) type needs to be registered as a component in Castle Windsor. Then, Quartz parameters have to be defined in the facility configuration, for example:

```xml
<castle>
  <facilities>
    <facility id="startable.facility" type="Castle.Facilities.Startable.StartableFacility, Castle.Windsor" />
    <facility id="quartznet" type="Castle.Facilities.Quartz.QuartzFacility, Castle.Facilities.Quartz">
      <quartz>
        <item key="quartz.scheduler.instanceName">XmlConfiguredInstance</item>
        <item key="quartz.threadPool.type">Quartz.Simpl.DefaultThreadPool, Quartz</item>
        <item key="quartz.threadPool.threadCount">5</item>
        <item key="quartz.plugin.xml.type">Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins</item>
        <item key="quartz.plugin.xml.scanInterval">10</item>
        <item key="quartz.plugin.xml.fileNames">~/quartz_jobs.xml</item>
      </quartz>
    </facility>
  </facilities>
</castle>
```

Note the use of the Startable Facility, it lets the Quartz scheduler start automatically.

### Configuration with listeners

The facility also lets Castle Windsor configure Quartz listeners. 
To do this you have to register your listeners as components, then reference them **by component name** in the facility configuration. 
Here's a sample facility configuration enhanced with several listeners:

```xml
<castle>
  <facilities>
    <facility id="startable.facility" type="Castle.Facilities.Startable.StartableFacility, Castle.Windsor" />
    <facility id="quartznet" type="Castle.Facilities.Quartz.QuartzFacility, Castle.Facilities.Quartz">
      <globalJobListeners>
        <item>${globalJobListener}</item>
      </globalJobListeners>
      <globalTriggerListeners>
        <item>${globalTriggerListener}</item>
      </globalTriggerListeners>
      <schedulerListeners>
        <item>${sampleSchedulerListener}</item>
      </schedulerListeners>
      <jobListeners>
        <job name="hello-world">
          <listener>${sampleJobListener}</listener>
        </job>
      </jobListeners>
      <triggerListeners>
        <trigger name="sample-trigger">
          <listener>${sampleTriggerListener}</listener>
        </trigger>
      </triggerListeners>
      <quartz>
        <item key="quartz.scheduler.instanceName">XmlConfiguredInstance</item>
        <item key="quartz.threadPool.type">Quartz.Simpl.DefaultThreadPool, Quartz</item>
        <item key="quartz.threadPool.threadCount">5</item>
        <item key="quartz.plugin.xml.type">Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins</item>
        <item key="quartz.plugin.xml.scanInterval">10</item>
        <item key="quartz.plugin.xml.fileNames">~/quartz_jobs.xml</item>
      </quartz>
    </facility>
  </facilities>
  <components>
    <component id="globalJobListener" type="Castle.Facilities.Quartz.SampleApp.SampleJobListener, Castle.Facilities.Quartz.SampleApp">
      <parameters>
        <name>Global job listener</name>
      </parameters>
    </component>
    <component id="sampleJobListener" type="Castle.Facilities.Quartz.SampleApp.SampleJobListener, Castle.Facilities.Quartz.SampleApp" />
    <component id="globalTriggerListener" type="Castle.Facilities.Quartz.SampleApp.SampleTriggerListener, Castle.Facilities.Quartz.SampleApp">
      <parameters>
        <name>Global trigger listener</name>
      </parameters>
    </component>
    <component id="sampleTriggerListener" type="Castle.Facilities.Quartz.SampleApp.SampleTriggerListener, Castle.Facilities.Quartz.SampleApp" />
    <component id="sampleSchedulerListener" type="Castle.Facilities.Quartz.SampleApp.SampleSchedulerListener, Castle.Facilities.Quartz.SampleApp" />
    <component id="sampleJob" type="Castle.Facilities.Quartz.SampleApp.SampleJob, Castle.Facilities.Quartz.SampleApp" />
  </components>
</castle>
```

Note that to associate a listeners with a particular job or trigger, the Quartz job or trigger name is used, and not the job's component name.