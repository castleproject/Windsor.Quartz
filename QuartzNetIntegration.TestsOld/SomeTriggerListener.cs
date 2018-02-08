using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Castle.Facilities.QuartzIntegration.Tests {
    public class SomeTriggerListener : ITriggerListener
    {
        public async Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => throw new NotImplementedException(), token);
        }

        public async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken token = default(CancellationToken))
        {
            return await Task.Run(new Func<bool>(() => throw new NotImplementedException()), token);
        }

        public async Task TriggerMisfired(ITrigger trigger, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => throw new NotImplementedException(), token);
        }

        public async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken token = default(CancellationToken))
        {
            await Task.Run(() => throw new NotImplementedException(), token);
        }

        public string Name => GetType().AssemblyQualifiedName;
    }
}