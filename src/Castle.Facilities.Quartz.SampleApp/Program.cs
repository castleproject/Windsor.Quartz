using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Facilities.Quartz.SampleApp.Jobs;
using Castle.Facilities.Quartz.SampleApp.Listeners;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Quartz;

namespace Castle.Facilities.Quartz.SampleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            UsingCodeWithoutStartableFacility();
        }

        private static void UsingCodeWithoutStartableFacility()
        {
            using (var container = new WindsorContainer())
            {
                // Register listeners
                container.Register(Component.For<ISampleJobListener>().ImplementedBy<SampleJobListener>());
                container.Register(Component.For<ISampleSchedulerListener>().ImplementedBy<SampleSchedulerListener>());
                container.Register(Component.For<ISampleTriggerListener>().ImplementedBy<SampleTriggerListener>());

                // Register jobs
                container.Register(Component.For<SampleJob>().ImplementedBy<SampleJob>());

                // Add facilities
                container.AddFacility<QuartzFacility>(f =>
                    {
                        f.Properties = new Dictionary<string, string>
                            {
                                {"quartz.scheduler.instanceName", "QuartzSchedulerConfiguredByCode"},
                                {"quartz.threadPool.type", "Quartz.Simpl.DefaultThreadPool, Quartz"},
                                {"quartz.threadPool.threadCount", "5"},
                                {
                                    "quartz.plugin.xml.type",
                                    "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins"
                                },
                                {"quartz.plugin.xml.scanInterval", "10"},
                                {"quartz.plugin.xml.fileNames", "~/quartz_jobs.xml"}
                            };

                        f.JobListeners = new[]
                        {
                            new JobListener(container.Resolve<ISampleJobListener>())
                        };

                        f.TriggerListeners = new[]
                        {
                            new TriggerListener(container.Resolve<ISampleTriggerListener>())
                        };

                        f.SchedulerListeners = new[]
                        {
                            container.Resolve<ISampleSchedulerListener>()
                        };
                    }
                );

                Console.WriteLine("Started");

                var scheduler = container.Resolve<IScheduler>();
                scheduler.Start();
                var task = Task.Run(() => Thread.Sleep(1000 * 1000));
                task.Wait();
            }
        }
        private static void UsingCodeWithStartableFacility()
        {
            using (var container = new WindsorContainer())
            {
                // Register listeners
                container.Register(Component.For<ISampleJobListener>().ImplementedBy<SampleJobListener>());
                container.Register(Component.For<ISampleSchedulerListener>().ImplementedBy<SampleSchedulerListener>());
                container.Register(Component.For<ISampleTriggerListener>().ImplementedBy<SampleTriggerListener>());

                // Register jobs
                container.Register(Component.For<SampleJob>().ImplementedBy<SampleJob>());

                // Add facilities
                container.AddFacility<StartableFacility>();
                container.AddFacility<QuartzFacility>(f =>
                    {
                        f.Properties = new Dictionary<string, string>
                        {
                            {"quartz.scheduler.instanceName", "QuartzSchedulerConfiguredByCode"},
                            {"quartz.threadPool.type", "Quartz.Simpl.DefaultThreadPool, Quartz"},
                            {"quartz.threadPool.threadCount", "5"},
                            {
                                "quartz.plugin.xml.type",
                                "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz.Plugins"
                            },
                            {"quartz.plugin.xml.scanInterval", "10"},
                            {"quartz.plugin.xml.fileNames", "~/quartz_jobs.xml"}
                        };

                        f.JobListeners = new[]
                        {
                            new JobListener(container.Resolve<ISampleJobListener>())
                        };

                        f.TriggerListeners = new[] 
                        {
                                new TriggerListener(container.Resolve<ISampleTriggerListener>())
                        };

                        f.SchedulerListeners = new[]
                        {
                            container.Resolve<ISampleSchedulerListener>()
                        };
                    }
                );

                Console.WriteLine("Started");

                var task = Task.Run(() => Thread.Sleep(1000 * 1000));
                task.Wait();
            }
        }
    }
}