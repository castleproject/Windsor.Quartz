using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Castle.Windsor;
using NUnit.Framework;
using Quartz;

namespace Castle.Facilities.Quartz.Tests.UnitTests
{
    [TestFixture]
    public abstract class BaseUnitTest
    {
        protected const string SchedulerInstanceName = "UnitTestScheduler";
        protected readonly Dictionary<string, string> QuartzProperties = new Dictionary<string, string>
        {
            {"quartz.scheduler.instanceName", SchedulerInstanceName},
            {"quartz.threadPool.type", "Quartz.Simpl.DefaultThreadPool, Quartz"},
            {"quartz.threadPool.threadCount", "5"}
        };

        protected IWindsorContainer Container;

        [SetUp]
        public void OnSetup()
        {
            Container = CreateContainer();
        }

        protected abstract IWindsorContainer CreateContainer();

        protected void Sleep(int seconds)
        {
            // sleep ... and ... wait ...
            var task = Task.Run(() => Thread.Sleep(seconds * 1000));
            task.Wait();
        }
        protected void ScheduleJob()
        {
            IJobDetail job = JobBuilder.Create<IJob>().WithIdentity("TestJob", "TestGroup").Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("TestJob_Trigger1", "TestGroup")
                .WithSimpleSchedule(s => 
                    s.WithRepeatCount(1).WithIntervalInSeconds(1))
                .Build();

            IScheduler scheduler = Container.Resolve<IScheduler>();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}