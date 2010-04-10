using System;
using Quartz;

namespace Castle.Facilities.QuartzIntegration.Tests {
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
}