# Quartz.NET Facility vNext Plans

This page discusses the requirements, design and implementation of a future release of the Quartz.Net facility. The objective is to eventually replace the current Castle.Components.Scheduler with this facility (which is currently a contrib project), as Castle.Components.Scheduler is no longer maintained.

Requirements:

* XML configuration (current facility already does this)
* Fluent API
* Windsor-managed global and per-job listeners (current facility already does this)
* Windsor-managed global and per-trigger listeners (current facility already does this)
* Windsor-managed scheduler listeners (current facility already does this)
* Define job/triggers via external quartz_jobs.xml (current facility already does this)
* Define job/triggers via XML config
* Define job/triggers via fluent API

Nice to have:

* Windsor-managed IJobStore
* Windsor-managed ISchedulerPlugin

Other:

* Make the code comply with the Castle code style guidelines.
* Test whatever can be tested.

Quartz features I'm not familiar with (don't know how/if they are to be integrated):

* JobDataMaps
* Clustering

Fluent API draft:

```csharp
container.AddFacility("quartz", new QuartzFacility()
    .AddGlobalTriggerListeners("myListenerKey1", "myListenerKey2") // reference registered trigger listeners by component key
    .AddGlobalTriggerListeners<MyTriggerListener, MyTriggerListener2>() // reference registered trigger listeners by component service/type
    .AddGlobalTriggerListeners(typeof(MyOtherTriggerListener), typeof(MyOtherTriggerListener2)) // non-generic overload
    .AddTriggerListeners(Listener.ForTrigger("trigger1").Service<MyTriggerListener3>()) // register a trigger listener
    .AddJobListeners(Listener.ForJob("job1").Named("jl1"))); // register a job listener
```

Hints:

* The current facility uses StdSchedulerFactory, but DirectSchedulerFactory seems more flexible, it might be easier to use it to implement the fluent API