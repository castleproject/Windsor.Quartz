using System;
using Quartz;

namespace SampleApp {
	public class SampleTriggerListener : ITriggerListener {
		public void TriggerFired(Trigger trigger, JobExecutionContext context) {
			Console.WriteLine("TriggerFired");
		}

		public bool VetoJobExecution(Trigger trigger, JobExecutionContext context) {
			Console.WriteLine("VetoJobExecution");
			return false;
		}

		public void TriggerMisfired(Trigger trigger) {
			Console.WriteLine("TriggerMisfired");
		}

		public void TriggerComplete(Trigger trigger, JobExecutionContext context, SchedulerInstruction triggerInstructionCode) {
			Console.WriteLine("TriggerComplete");
		}

		public string Name {
			get { return GetType().Name; }
		}
	}
}