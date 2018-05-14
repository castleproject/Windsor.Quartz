using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.Facilities.Startable;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Moq;
using NUnit.Framework;
using Quartz;
using Quartz.Impl.Matchers;

namespace Castle.Facilities.Quartz.Tests.UnitTests
{
    [TestFixture]
    public class QuartzFacilityUnitTests : BaseUnitTest
    {
        protected Mock<IJob> TestJob;
        protected Mock<IJobListener> TestJobListener;
        protected Mock<ISchedulerListener> TestSchedulerListener;
        protected Mock<ITriggerListener> TestTriggerListener;

        protected override IWindsorContainer CreateContainer()
        {
            TestJobListener = new Mock<IJobListener>();
            TestSchedulerListener = new Mock<ISchedulerListener>();
            TestTriggerListener = new Mock<ITriggerListener>();
            TestJob = new Mock<IJob>();

            // Job
            TestJob.Setup(m => m.Execute(It.IsAny<IJobExecutionContext>()))
                .Returns(Task.Run(() => Console.WriteLine("Execute")))
                .Verifiable();

            // JobListener
            TestJobListener
                .SetupGet(p => p.Name).Returns("TestJobListener");
            TestJobListener
                .Setup(m => m.JobExecutionVetoed(It.IsAny<IJobExecutionContext>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("JobListener - JobExecutionVetoed")))
                .Verifiable();
            TestJobListener
                .Setup(m => m.JobToBeExecuted(It.IsAny<IJobExecutionContext>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("JobListener - JobToBeExecuted")))
                .Verifiable();
            TestJobListener
                .Setup(m => m.JobWasExecuted(It.IsAny<IJobExecutionContext>(), It.IsAny<JobExecutionException>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("JobListener - JobWasExecuted")))
                .Verifiable();

            // TriggerListener
            TestTriggerListener
                .SetupGet(p => p.Name).Returns("TestTriggerListener");
            TestTriggerListener
                .Setup(m => m.TriggerComplete(It.IsAny<ITrigger>(), It.IsAny<IJobExecutionContext>(), It.IsAny<SchedulerInstruction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("TriggerListener - TriggerComplete")))
                .Verifiable();
            TestTriggerListener
                .Setup(m => m.TriggerFired(It.IsAny<ITrigger>(), It.IsAny<IJobExecutionContext>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("TriggerListener - TriggerFired")))
                .Verifiable();
            TestTriggerListener
                .Setup(m => m.TriggerMisfired(It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("TriggerListener - TriggerMisfired")))
                .Verifiable();

            // SchedulerListener
            TestSchedulerListener
                .Setup(m => m.JobAdded(It.IsAny<IJobDetail>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobAdded")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.JobDeleted(It.IsAny<JobKey>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobDeleted")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.JobInterrupted(It.IsAny<JobKey>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobInterrupted")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.JobPaused(It.IsAny<JobKey>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobPaused")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.JobResumed(It.IsAny<JobKey>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobResumed")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.JobScheduled(It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobScheduled")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.JobUnscheduled(It.IsAny<TriggerKey>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobUnscheduled")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.JobsPaused(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobsPaused")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.JobsResumed(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - JobsResumed")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.SchedulerError(It.IsAny<string>(), It.IsAny<SchedulerException>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - SchedulerError")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.SchedulerInStandbyMode(It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - SchedulerInStandbyMode")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.SchedulerShutdown(It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - SchedulerShutdown")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.SchedulerError(It.IsAny<string>(), It.IsAny<SchedulerException>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - SchedulerError")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.SchedulerShuttingdown(It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - SchedulerShuttingdown")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.SchedulerStarted(It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - SchedulerStarted")))
                .Verifiable();
            TestSchedulerListener
                .Setup(m => m.SchedulerStarting(It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => Console.WriteLine("SchedulerListener - SchedulerStarting")))
                .Verifiable();


            // Container
            return new WindsorContainer()
                .Register(Component.For<IJobListener>().Instance(TestJobListener.Object).LifestyleTransient())
                .Register(Component.For<ITriggerListener>().Instance(TestTriggerListener.Object).LifestyleTransient())
                .Register(Component.For<ISchedulerListener>().Instance(TestSchedulerListener.Object).LifestyleTransient())
                .Register(Component.For<IJob>().Instance(TestJob.Object).LifestyleTransient())
                .AddFacility<StartableFacility>(q => q.DeferredStart());
        }

        [Test]
        public void GlobalJobListenerFired()
        {
            // Add Quartz (with 1 joblistener)
            Container.AddFacility<QuartzFacility>(q =>
                q.SetJobListeners(new JobListener(Container.Resolve<IJobListener>()))
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            TestJobListener.Verify(m => m.JobWasExecuted(It.IsAny<IJobExecutionContext>(),
                It.IsAny<JobExecutionException>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void JobListenerFired()
        {
            // Add Quartz (with 1 joblistener)
            Container.AddFacility<QuartzFacility>(q =>
                q.SetJobListeners(new JobListener(Container.Resolve<IJobListener>(), new IMatcher<JobKey>[] { KeyMatcher<JobKey>.KeyEquals(new JobKey("TestJob", "TestGroup")) }))
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            TestJobListener.Verify(m => m.JobWasExecuted(It.IsAny<IJobExecutionContext>(),
                It.IsAny<JobExecutionException>(), It.IsAny<CancellationToken>()));
        }


        [Test]
        public void JobListenerNotFired()
        {
            // Add Quartz (with 1 joblistener)
            Container.AddFacility<QuartzFacility>(q =>
                q.SetJobListeners(new JobListener(Container.Resolve<IJobListener>(), new IMatcher<JobKey>[] { KeyMatcher<JobKey>.KeyEquals(new JobKey("FakeJob", "FakeGroup")) }))
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            TestJobListener.Verify(m => m.JobWasExecuted(It.IsAny<IJobExecutionContext>(), It.IsAny<JobExecutionException>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void GlobalTriggerListenerFired()
        {
            // Add Quartz (with 1 triggerlistener)
            Container.AddFacility<QuartzFacility>(q =>
                q.SetTriggerListeners(new TriggerListener(Container.Resolve<ITriggerListener>()))
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            TestTriggerListener.Verify(m => m.TriggerComplete(It.IsAny<ITrigger>(), It.IsAny<IJobExecutionContext>(),
                It.IsAny<SchedulerInstruction>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void TriggerListenerFired()
        {
            // Add Quartz (with 1 triggerlistener)
            Container.AddFacility<QuartzFacility>(q =>
                q.SetTriggerListeners(new TriggerListener(Container.Resolve<ITriggerListener>(), new IMatcher<TriggerKey>[] { KeyMatcher<TriggerKey>.KeyEquals(new TriggerKey("TestJob_Trigger1", "TestGroup")) }))
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            TestTriggerListener.Verify(m => m.TriggerComplete(It.IsAny<ITrigger>(), It.IsAny<IJobExecutionContext>(),
                It.IsAny<SchedulerInstruction>(), It.IsAny<CancellationToken>()));
        }

        [Test]
        public void TriggerListenerNotFired()
        {
            // Add Quartz (with 1 triggerlistener)
            Container.AddFacility<QuartzFacility>(q =>
                q.SetTriggerListeners(new TriggerListener(Container.Resolve<ITriggerListener>(), new IMatcher<TriggerKey>[] { KeyMatcher<TriggerKey>.KeyEquals(new TriggerKey("FakeTrigger", "FakeGroup")) }))
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            TestTriggerListener.Verify(m => m.TriggerComplete(It.IsAny<ITrigger>(), It.IsAny<IJobExecutionContext>(),
                It.IsAny<SchedulerInstruction>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void SchedulerListenerFired()
        {
            // Add Quartz (with 1 schedulerlistener)
            Container.AddFacility<QuartzFacility>(q =>
                q.SetSchedulerListeners(Container.Resolve<ISchedulerListener>())
                    .SetProperties(QuartzProperties));

            // Schedule Job
            ScheduleJob();
            Sleep(2);

            // Assert
            TestSchedulerListener.Verify(m => m.JobDeleted(It.IsAny<JobKey>(), It.IsAny<CancellationToken>()));
            TestSchedulerListener.Verify(m => m.JobAdded(It.IsAny<IJobDetail>(), It.IsAny<CancellationToken>()));
            TestSchedulerListener.Verify(m => m.JobScheduled(It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()));
            TestSchedulerListener.Verify(m => m.TriggerFinalized(It.IsAny<ITrigger>(), It.IsAny<CancellationToken>()));
        }
    }
}