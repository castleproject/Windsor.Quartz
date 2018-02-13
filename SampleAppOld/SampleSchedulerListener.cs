using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace SampleApp
{
    public class SampleSchedulerListener : ISchedulerListener
    {
        public async Task JobScheduled(ITrigger trigger, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobScheduled", token);
        }

        public async Task JobUnscheduled(TriggerKey triggerKey, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobUnscheduled", token);
        }

        public async Task TriggerFinalized(ITrigger trigger, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".TriggerFinalized", token);
        }

        public async Task TriggerPaused(TriggerKey triggerKey, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".TriggerPaused", token);
        }

        public async Task TriggersPaused(string triggerGroup, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".TriggersPaused", token);
        }

        public async Task TriggerResumed(TriggerKey triggerKey, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".TriggerResumed", token);
        }

        public async Task TriggersResumed(string triggerGroup, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".TriggersResumed", token);
        }

        public async Task JobAdded(IJobDetail jobDetail, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobAdded", token);
        }

        public async Task JobDeleted(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobDeleted", token);
        }

        public async Task JobPaused(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobPaused", token);
        }

        public async Task JobInterrupted(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobInterupted", token);
        }

        public async Task JobsPaused(string jobGroup, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobsPaused", token);
        }

        public async Task JobResumed(JobKey jobKey, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobResumed", token);
        }

        public async Task JobsResumed(string jobGroup, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".JobsResumed", token);
        }

        public async Task SchedulerError(string msg, SchedulerException cause,
            CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".SchedulerError", token);
        }

        public async Task SchedulerInStandbyMode(CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".SchedulerInStandbyMode", token);
        }

        public async Task SchedulerStarted(CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".SchedulerStarted", token);
        }

        public async Task SchedulerStarting(CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".SchedulerStarting", token);
        }

        public async Task SchedulerShutdown(CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".SchedulerShutdown", token);
        }

        public async Task SchedulerShuttingdown(CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".SchedulerShuttingdown", token);
        }

        public async Task SchedulingDataCleared(CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage(".SchedulingDataCleared", token);
        }

        private Task WriteMesssage(string message, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => Console.WriteLine("{0}.{1}", GetType().Name, message), token);
        }
    }
}