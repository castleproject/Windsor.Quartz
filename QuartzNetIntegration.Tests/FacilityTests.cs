using System;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using NUnit.Framework;
using Quartz;

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
        [ExpectedException(typeof (DependencyResolverException))]
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

            c.AddComponent("jobli", typeof (IJobListener), typeof (SomeJobListener));
            c.AddFacility("quartz", new QuartzFacility());
            var scheduler = c.Resolve<IScheduler>() as QuartzNetScheduler;
            foreach (IJobListener l in scheduler.GlobalJobListeners)
                Console.WriteLine(l.Name);
            Assert.AreEqual(2, scheduler.GlobalJobListeners.Count);
        }

        [Test]
        public void GlobalTriggerListeners() {
            var c = new WindsorContainer();
            var config = new MutableConfiguration("facility");
            config.CreateChild("quartz").CreateChild("item", "qwe").Attribute("key", "qq");
            config.CreateChild("globalTriggerListeners").CreateChild("item", "${jobli}");
            c.Kernel.ConfigurationStore.AddFacilityConfiguration("quartz", config);

            c.AddComponent("jobli", typeof (ITriggerListener), typeof (SomeTriggerListener));
            c.AddFacility("quartz", new QuartzFacility());

            var scheduler = c.Resolve<IScheduler>() as QuartzNetScheduler;
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

            c.AddComponent("jobli", typeof (IJobListener), typeof (SomeJobListener));
            c.AddFacility("quartz", new QuartzFacility());

            var scheduler = c.Resolve<IScheduler>() as QuartzNetScheduler;
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

            c.AddComponent("trigli", typeof(ITriggerListener), typeof(SomeTriggerListener));
            c.AddFacility("quartz", new QuartzFacility());

            var scheduler = c.Resolve<IScheduler>() as QuartzNetScheduler;
            foreach (var l in scheduler.TriggerListenerNames)
                Console.WriteLine(l);
            var trigli = scheduler.GetTriggerListener(typeof(SomeTriggerListener).AssemblyQualifiedName);
            Assert.IsNotNull(trigli);

        }


        public class SomeTriggerListener : ITriggerListener {
            public void TriggerFired(Trigger trigger, JobExecutionContext context) {
                throw new NotImplementedException();
            }

            public bool VetoJobExecution(Trigger trigger, JobExecutionContext context) {
                throw new NotImplementedException();
            }

            public void TriggerMisfired(Trigger trigger) {
                throw new NotImplementedException();
            }

            public void TriggerComplete(Trigger trigger, JobExecutionContext context, SchedulerInstruction triggerInstructionCode) {
                throw new NotImplementedException();
            }

            public string Name {
                get { return GetType().AssemblyQualifiedName; }
            }
        }

        public class SomeJobListener : IJobListener {
            public void JobToBeExecuted(JobExecutionContext context) {
                throw new NotImplementedException();
            }

            public void JobExecutionVetoed(JobExecutionContext context) {
                throw new NotImplementedException();
            }

            public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
                throw new NotImplementedException();
            }

            public string Name {
                get { return GetType().AssemblyQualifiedName; }
            }
        }
    }
}