using System;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using NUnit.Framework;
using Quartz;
using System.Threading;

namespace Castle.Facilities.QuartzIntegration.Tests {
    [TestFixture]
    public class FacilityTests {
        [Test]
        [ExpectedException(typeof (FacilityException))]
        public void NoConfig_throws() {
            using (var c = new WindsorContainer())
                c.AddFacility("quartz", new QuartzFacility());
        }

        [Test]
        [ExpectedException(typeof (FacilityException))]
        public void NoProps_throws() {
            using (var c = new WindsorContainer()) {
                c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", new MutableConfiguration("facility"));
                c.AddFacility("quartz", new QuartzFacility());
            }
        }

        [Test]
        public void Basic() {
            using (var c = new WindsorContainer()) {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                c.AddFacility<QuartzFacility>(f => f.Configuration = config);
                c.Resolve<IJobScheduler>();
                c.Resolve<IScheduler>();
            }
        }

        [Test]
        [ExpectedException(typeof(HandlerException))]
        public void GlobalJobListeners_with_no_registration_throws() {
            using (var c = new WindsorContainer()) {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                config.CreateChild("globalJobListeners").CreateChild("item", "${jobli}");
                c.AddFacility<QuartzFacility>(f => f.Configuration = config);
                c.Resolve<IScheduler>();
            }
        }

        [Test]
        public void GlobalJobListeners() {
            using (var c = new WindsorContainer()) {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                config.CreateChild("globalJobListeners").CreateChild("item", "${jobli}");
                c.Register(Component.For<IJobListener>().ImplementedBy<SomeJobListener>().Named("jobli"));
                c.AddFacility<QuartzFacility>(f => f.Configuration = config);
                var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
                foreach (IJobListener l in scheduler.ListenerManager.GetJobListeners())
                    Console.WriteLine(l.Name);
                Assert.AreEqual(2, scheduler.ListenerManager.GetJobListeners().Count);
            }
        }

        [Test]
        public void GlobalTriggerListeners() {
            using (var c = new WindsorContainer()) {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                config.CreateChild("globalTriggerListeners").CreateChild("item", "${jobli}");
                c.Register(Component.For<ITriggerListener>().ImplementedBy<SomeTriggerListener>().Named("jobli"));
                c.AddFacility<QuartzFacility>(f => f.Configuration = config);

                var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
                foreach (ITriggerListener l in scheduler.ListenerManager.GetTriggerListeners())
                    Console.WriteLine(l.Name);
                Assert.AreEqual(1, scheduler.ListenerManager.GetTriggerListeners().Count);
            }
        }

        [Test]
        public void JobListeners() {
            using (var c = new WindsorContainer()) {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                var listenerConfig = config.CreateChild("jobListeners");
                listenerConfig
                    .CreateChild("job")
                    .Attribute("name", "someJob")
                    .CreateChild("listener", "${jobli}");
                //c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);

                c.Register(Component.For<IJobListener>().ImplementedBy<SomeJobListener>().Named("jobli"));
                c.AddFacility<QuartzFacility>(f => f.Configuration = config);

                var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
                foreach (var l in scheduler.ListenerManager.GetJobListeners())
                    Console.WriteLine(l);
                var jobli = scheduler.ListenerManager.GetJobListener(typeof(SomeJobListener).AssemblyQualifiedName);
                Assert.IsNotNull(jobli);
            }
        }

        [Test]
        public void TriggerListeners() {
            using (var c = new WindsorContainer()) {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
                var listenerConfig = config.CreateChild("triggerListeners");
                listenerConfig
                    .CreateChild("job")
                    .Attribute("name", "someJob")
                    .CreateChild("listener", "${trigli}");
                
                c.Register(Component.For<ITriggerListener>().ImplementedBy<SomeTriggerListener>().Named("trigli"));
                c.AddFacility<QuartzFacility>(f => f.Configuration = config);

                var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
                foreach (var l in scheduler.ListenerManager.GetTriggerListeners())
                    Console.WriteLine(l);
                var trigli = scheduler.ListenerManager.GetTriggerListener(typeof(SomeTriggerListener).AssemblyQualifiedName);
                Assert.IsNotNull(trigli);
            }
        }

        [Test]
        public void FacilityRegistersReleasingJobListener() {
            using (var c = new WindsorContainer()) {
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz");
                c.AddFacility<QuartzFacility>(f => f.Configuration = config);
                var scheduler = c.Resolve<IScheduler>();
                //Assert.IsNotNull(scheduler.GlobalJobListeners);
                //Assert.AreEqual(2, scheduler.GlobalJobListeners.Count);
                Assert.IsAssignableFrom(typeof(ReleasingJobListener), scheduler.ListenerManager.GetJobListeners()[0]);
            }
        }

        [Test]
        public void DisposableJobIsDisposed() {
            using (var c = new WindsorContainer()) {
                c.Register(Component.For<DisposableJob>().LifeStyle.Transient);
                var config = new MutableConfiguration("facility");
                config.CreateChild("quartz");
                c.AddFacility<QuartzFacility>(f => f.Configuration = config);
                var scheduler = c.Resolve<IScheduler>();
                var jobDetail = JobBuilder.Create<DisposableJob>().WithIdentity("somejob").Build();
                var trigger = TriggerBuilder.Create().WithIdentity("sometrigger").WithSimpleSchedule(s => s.WithIntervalInSeconds(1)).Build();
                scheduler.ScheduleJob(jobDetail, trigger);
                Assert.IsFalse(DisposableJob.Disposed);
                scheduler.Start();
                Thread.Sleep(1000);
                Assert.IsTrue(DisposableJob.Disposed);
            }
        }

        class DisposableJob : IJob, IDisposable
        {
            public static bool Disposed = false;

            public void Dispose()
            {
                Console.WriteLine("Dispose");
                Disposed = true;
            }

            public void Execute(IJobExecutionContext context)
            {
                Console.WriteLine("Execute");
            }
        }
    }
}