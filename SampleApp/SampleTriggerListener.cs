using System;
using Quartz;

namespace SampleApp {
	public class SampleTriggerListener : ITriggerListener {
		public void TriggerFired(Trigger trigger, JobExecutionContext context) {
			Console.WriteLine(Name + ".TriggerFired");
		}

		public bool VetoJobExecution(Trigger trigger, JobExecutionContext context) {
			Console.WriteLine(Name + ".VetoJobExecution");
			return false;
		}

		public void TriggerMisfired(Trigger trigger) {
			Console.WriteLine(Name + ".TriggerMisfired");
		}

		public void TriggerComplete(Trigger trigger, JobExecutionContext context, SchedulerInstruction triggerInstructionCode) {
			Console.WriteLine(Name + ".TriggerComplete");
		}

		public string Name {
			get { return GetType().Name; }
		}
	}
}