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
            var c = new WindsorContainer();
            c.AddFacility("quartz", new QuartzFacility());
        }

        [Test]
        [ExpectedException(typeof (FacilityException))]
        public void NoProps_throws() {
            var c = new WindsorContainer();
            c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", new MutableConfiguration("facility"));
            c.AddFacility("quartz", new QuartzFacility());
        }

        [Test]
        public void Basic() {
            var c = new WindsorContainer();
            var config = new MutableConfiguration("facility");
            config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
            c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
            c.AddFacility("quartz", new QuartzFacility());
            c.Resolve<IJobScheduler>();
            c.Resolve<IScheduler>();
        }

        [Test]
        [ExpectedException(typeof(HandlerException))]
        public void GlobalJobListeners_with_no_registration_throws() {
            var c = new WindsorContainer();
            var config = new MutableConfiguration("facility");
            config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
            config.CreateChild("globalJobListeners").CreateChild("item", "${jobli}");
            c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
            c.AddFacility("quartz", new QuartzFacility());
            c.Resolve<IScheduler>();
        }

        [Test]
        public void GlobalJobListeners() {
            var c = new WindsorContainer();
            var config = new MutableConfiguration("facility");
            config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
            config.CreateChild("globalJobListeners").CreateChild("item", "${jobli}");
            c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
            c.Register(Component.For<IJobListener>().ImplementedBy<SomeJobListener>().Named("jobli"));
            c.AddFacility("quartz", new QuartzFacility());
            var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
            foreach (IJobListener l in scheduler.GlobalJobListeners)
                Console.WriteLine(l.Name);
            Assert.AreEqual(3, scheduler.GlobalJobListeners.Count);
        }

        [Test]
        public void GlobalTriggerListeners() {
            var c = new WindsorContainer();
            var config = new MutableConfiguration("facility");
            config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
            config.CreateChild("globalTriggerListeners").CreateChild("item", "${jobli}");
            c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
            c.Register(Component.For<ITriggerListener>().ImplementedBy<SomeTriggerListener>().Named("jobli"));
            c.AddFacility("quartz", new QuartzFacility());

            var scheduler = (QuartzNetScheduler)c.Resolve<IScheduler>();
            foreach (ITriggerListener l in scheduler.GlobalTriggerListeners)
                Console.WriteLine(l.Name);
            Assert.AreEqual(1, scheduler.GlobalTriggerListeners.Count);
        }

        [Test]
        public void JobListeners() {
            var c = new WindsorContainer();
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
            foreach (var l in scheduler.JobListenerNames)
                Console.WriteLine(l);
            var jobli = scheduler.GetJobListener(typeof(SomeJobListener).AssemblyQualifiedName);
            Assert.IsNotNull(jobli);
        }

        [Test]
        public void TriggerListeners() {
            var c = new WindsorContainer();
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
            foreach (var l in scheduler.TriggerListenerNames)
                Console.WriteLine(l);
            var trigli = scheduler.GetTriggerListener(typeof(SomeTriggerListener).AssemblyQualifiedName);
            Assert.IsNotNull(trigli);

        }

        [Test]
        public void FacilityRegistersReleasingJobListener() {
            var c = new WindsorContainer();
            var config = new MutableConfiguration("facility");
            config.CreateChild("quartz");
            c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
            c.AddFacility("quartz", new QuartzFacility());
            var scheduler = c.Resolve<IScheduler>();
            Assert.IsNotNull(scheduler.GlobalJobListeners);
            Assert.AreEqual(2, scheduler.GlobalJobListeners.Count);
            Assert.IsInstanceOfType(typeof(ReleasingJobListener), scheduler.GlobalJobListeners[0]);
        }

        [Test]
        public void DisposableJobIsDisposed() {
            var c = new WindsorContainer();
            c.Register(Component.For<DisposableJob>().LifeStyle.Transient);
            var config = new MutableConfiguration("facility");
            config.CreateChild("quartz");
            c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);
            c.AddFacility("quartz", new QuartzFacility());
            var scheduler = c.Resolve<IScheduler>();
            var jobDetail = new JobDetail("somejob", typeof(DisposableJob));
            var trigger = TriggerUtils.MakeSecondlyTrigger("sometrigger");
            scheduler.ScheduleJob(jobDetail, trigger);
            Assert.IsFalse(DisposableJob.Disposed);
            scheduler.Start();
            Thread.Sleep(1000);
            Assert.IsTrue(DisposableJob.Disposed);
        }

        class DisposableJob : IJob, IDisposable {
            public static bool Disposed = false;

            public void Dispose() {
                Console.WriteLine("Dispose");
                Disposed = true;
            }

            public void Execute(JobExecutionContext context) {
                Console.WriteLine("Execute");
            }
        }
    }
}