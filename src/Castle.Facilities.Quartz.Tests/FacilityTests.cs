using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Quartz;

namespace Castle.Facilities.Quartz.Tests
{
    [TestFixture]
    public class FacilityTests
    {
        private class DisposableJob : IJob, IDisposable
        {
            public static bool Disposed;

            public void Dispose()
            {
                Console.WriteLine("Dispose");
                Disposed = true;
            }

            public async Task Execute(IJobExecutionContext context)
            {
                await Task.Run(() => Console.WriteLine("Execute"));
            }
        }

        [Test]
        public void Basic()
        {
            using (var c = new WindsorContainer())
            {
                var facilityConfig = new MutableConfiguration("facility");
                var quartzConfig = facilityConfig.CreateChild("quartz")
                    .Attribute("id", "quartznet");

                quartzConfig.CreateChild("item", "qwe").Attribute("key", "qq");

                c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", facilityConfig);

                c.AddFacility("quartz", new QuartzFacility());

                c.Resolve<IJobScheduler>();
                c.Resolve<IScheduler>();
            }
        }

        [Test]
        public void DisposableJobIsDisposed()
        {
            using (var c = new WindsorContainer())
            {
                c.Register(Component.For<DisposableJob>().LifeStyle.Transient);
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz");
                c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);

                c.AddFacility("quartz", new QuartzFacility());

                var scheduler = c.Resolve<IScheduler>();
                var jobDetail = JobBuilder.Create<DisposableJob>().WithIdentity("somejob").Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("sometrigger")
                    .WithSimpleSchedule(s => s.WithIntervalInSeconds(1))
                    .Build();

                var task = scheduler.ScheduleJob(jobDetail, trigger);
                task.Wait();
                var dateTimeOffset = task.Result;

                Assert.IsFalse(DisposableJob.Disposed);
                scheduler.Start();
                Thread.Sleep(1000);
                Assert.IsTrue(DisposableJob.Disposed);
            }
        }

        [Test]
        public void FacilityRegistersReleasingJobListener()
        {
            using (var c = new WindsorContainer())
            {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz");
                c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
                c.AddFacility("quartz", new QuartzFacility());
                var scheduler = c.Resolve<IScheduler>();
                //Assert.IsNotNull(scheduler.GlobalJobListeners);
                //Assert.AreEqual(2, scheduler.GlobalJobListeners.Count);

                Assert.IsAssignableFrom(typeof(ReleasingJobListener),
                    scheduler.ListenerManager.GetJobListeners().ToArray()[0]);
            }
        }

        [Test]
        public void GlobalJobListeners()
        {
            using (var c = new WindsorContainer())
            {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                config.CreateChild("globalJobListeners").CreateChild("item", "${jobli}");
                c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
                c.Register(Component.For<IJobListener>().ImplementedBy<SomeJobListener>().Named("jobli"));
                c.AddFacility("quartz", new QuartzFacility());
                var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
                foreach (var l in scheduler.ListenerManager.GetJobListeners())
                    Console.WriteLine(l.Name);
                Assert.AreEqual(2, scheduler.ListenerManager.GetJobListeners().Count);
            }
        }

        [Test]
        public void GlobalJobListeners_with_no_registration_throws()
        {
            Assert.Throws(typeof(HandlerException), () =>
            {
                using (var c = new WindsorContainer())
                {
                    var config = new MutableConfiguration("facility");
                    config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                    config.CreateChild("globalJobListeners").CreateChild("item", "${jobli}");
                    c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
                    c.AddFacility("quartz", new QuartzFacility());
                    c.Resolve<IScheduler>();
                }
            });
        }

        [Test]
        public void GlobalTriggerListeners()
        {
            using (var c = new WindsorContainer())
            {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                config.CreateChild("globalTriggerListeners").CreateChild("item", "${jobli}");
                c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
                c.Register(Component.For<ITriggerListener>().ImplementedBy<SomeTriggerListener>().Named("jobli"));
                c.AddFacility("quartz", new QuartzFacility());

                var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
                foreach (var l in scheduler.ListenerManager.GetTriggerListeners())
                    Console.WriteLine(l.Name);
                Assert.AreEqual(1, scheduler.ListenerManager.GetTriggerListeners().Count);
            }
        }

        [Test]
        public void JobListeners()
        {
            using (var c = new WindsorContainer())
            {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                var listenerConfig = config.CreateChild("jobListeners");
                listenerConfig
                    .CreateChild("job")
                    .Attribute("name", "someJob")
                    .CreateChild("listener", "${jobli}");
                c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);

                c.Register(Component.For<IJobListener>().ImplementedBy<SomeJobListener>().Named("jobli"));
                c.AddFacility("quartz", new QuartzFacility());

                var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
                foreach (var l in scheduler.ListenerManager.GetJobListeners())
                    Console.WriteLine(l);
                var jobli = scheduler.ListenerManager.GetJobListener(typeof(SomeJobListener).AssemblyQualifiedName);
                Assert.IsNotNull(jobli);
            }
        }

        [Test]
        public void NoConfig_throws()
        {
            Assert.Throws(typeof(FacilityException), () =>
            {
                using (var c = new WindsorContainer())
                {
                    c.AddFacility("quartz", new QuartzFacility());
                }
            });
        }

        [Test]
        public void NoProps_throws()
        {
            Assert.Throws(typeof(FacilityException), () =>
            {
                using (var c = new WindsorContainer())
                {
                    c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz",
                        new MutableConfiguration("facility"));
                    c.AddFacility("quartz", new QuartzFacility());
                }
            });
        }

        [Test]
        public void TriggerListeners()
        {
            using (var c = new WindsorContainer())
            {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                var listenerConfig = config.CreateChild("triggerListeners");
                listenerConfig
                    .CreateChild("job")
                    .Attribute("name", "someJob")
                    .CreateChild("listener", "${trigli}");
                c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);

                c.Register(Component.For<ITriggerListener>().ImplementedBy<SomeTriggerListener>().Named("trigli"));
                c.AddFacility("quartz", new QuartzFacility());

                var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
                foreach (var l in scheduler.ListenerManager.GetTriggerListeners())
                    Console.WriteLine(l);
                var trigli =
                    scheduler.ListenerManager.GetTriggerListener(typeof(SomeTriggerListener).AssemblyQualifiedName);
                Assert.IsNotNull(trigli);
            }
        }
    }
}