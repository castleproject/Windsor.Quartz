using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Castle.Facilities.Quartz.SampleApp.Listeners
{
    public interface ISampleTriggerListener : ITriggerListener
    {

    }
    public class SampleTriggerListener : ISampleTriggerListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SampleTriggerListener" /> class.
        /// </summary>
        public SampleTriggerListener()
        {
            Name = GetType().Name;
        }

        public async Task TriggerFired(ITrigger trigger, IJobExecutionContext context,
            CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage("JobToBeExecuted", token);
        }

        public async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context,
            CancellationToken token = default(CancellationToken))
        {
            return await WriteMesssageWithBoolResult("VetoJobExecution", false, token);
        }

        public async Task TriggerMisfired(ITrigger trigger, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage("TriggerMisfired", token);
        }

        public async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context,
            SchedulerInstruction triggerInstructionCode, CancellationToken token = default(CancellationToken))
        {
            await WriteMesssage("TriggerComplete", token);
        }

        /// <summary>
        ///     Get the name of the <see cref="T:Quartz.ITriggerListener" />.
        /// </summary>
        public string Name { get; set; }

        private Task WriteMesssage(string message, CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() => Console.WriteLine("{0}.{1}", GetType().Name, message), token);
        }

        private Task<bool> WriteMesssageWithBoolResult(string message, bool result,
            CancellationToken token = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                Console.WriteLine("{0}.{1}", GetType().Name, message);
                return result;
            }, token);
        }
    }
}