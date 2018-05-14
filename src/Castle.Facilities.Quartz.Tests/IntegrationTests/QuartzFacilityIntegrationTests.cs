using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Quartz;

namespace Castle.Facilities.Quartz.Tests.IntegrationTests
{
    [TestFixture]
    public class QuartzFacilityIntegrationTests
    {
        [SetUp]
        public void OnSetup()
        {
            _container = CreateContainer();
        }

        public const string SchedulerInstanceName = "IntegrationTestScheduler";
        private IWindsorContainer _container;

        public Dictionary<string, string> QuartzProperties => new Dictionary<string, string>
        {
            {"quartz.scheduler.instanceName", SchedulerInstanceName},
            {"quartz.threadPool.type", "Quartz.Simpl.DefaultThreadPool, Quartz"},
            {"quartz.threadPool.threadCount", "1"}
        };


        private IWindsorContainer CreateContainer()
        {
            var containerBuilder = new WindsorContainer()
                .Register(Component.For<ITestJobListener>().ImplementedBy<TestJobListener>().LifestyleSingleton())
                .Register(Component.For<ITestJob>().ImplementedBy<TestJob>().LifestyleTransient())
                .AddFacility<StartableFacility>(q => q.DeferredStart());

            return containerBuilder;
        }

        private void Sleep(int seconds)
        {
            // sleep ... and ... wait ...
            var task = Task.Run(() => Thread.Sleep(seconds * 1000));
            task.Wait();
        }

        private void ScheduleJob()
        {
            var job = JobBuilder.Create<ITestJob>().WithIdentity("IntegrationTestJob", "IntegrationTestGroup").Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("IntegrationTestJob_Trigger1", "IntegrationTestGroup")
                .WithSimpleSchedule(s =>
                    s.WithRepeatCount(1).WithIntervalInSeconds(1))
                .Build();

            var scheduler = _container.Resolve<IScheduler>();
            scheduler.ScheduleJob(job, trigger);
        }

        [Test]
        public void TestJobDisposing()
        {
            // Add Quartz
            _container.AddFacility<QuartzFacility>(q =>
                q.SetJobListeners(new JobListener(_container.Resolve<ITestJobListener>()))
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            Assert.That(TestJob.IsDisposed, Is.True);
        }

        [Test]
        public void TestJobListener()
        {
            // Add Quartz
            _container.AddFacility<QuartzFacility>(q =>
                q.SetJobListeners(new JobListener(_container.Resolve<ITestJobListener>()))
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            var listener = _container.Resolve<ITestJobListener>();
            Assert.That(listener.HasFiredJobWasExecuted, Is.True);
        }
    }
}