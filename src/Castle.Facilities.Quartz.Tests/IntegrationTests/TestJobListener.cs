using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Castle.Facilities.Quartz.Tests.IntegrationTests
{
    public interface ITestJobListener : IJobListener
    {
        bool HasFiredJobWasExecuted { get; set; }
    }

    public class TestJobListener : ITestJobListener
    {
        public TestJobListener()
        {
            Name = GetType().Name;
        }

        public bool HasFiredJobWasExecuted { get; set; }

        public async Task JobToBeExecuted(IJobExecutionContext context,
            CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage("JobToBeExecuted", token);
        }

        public async Task JobExecutionVetoed(IJobExecutionContext context,
            CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage("JobExecutionVetoed", token);
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException,
            CancellationToken token = default(CancellationToken))
        {
            HasFiredJobWasExecuted = true;
            await WriteMesssage("JobWasExecuted", token);
        }

        public string Name { get; set; }

        private Task WriteMesssage(string message, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => Console.WriteLine("{0}.{1}", GetType().Name, message), token);
        }
    }
}