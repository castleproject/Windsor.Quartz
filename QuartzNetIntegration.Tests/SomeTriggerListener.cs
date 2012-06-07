using System;
using Quartz;

namespace Castle.Facilities.QuartzIntegration.Tests {
    public class SomeTriggerListener : ITriggerListener {
        public void TriggerFired(ITrigger trigger, IJobExecutionContext context) {
            throw new NotImplementedException();
        }

        public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context) {
            throw new NotImplementedException();
        }

        public void TriggerMisfired(ITrigger trigger) {
            throw new NotImplementedException();
        }

        public void TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode) {
            throw new NotImplementedException();
        }

        public string Name {
            get { return GetType().AssemblyQualifiedName; }
        }
    }
}