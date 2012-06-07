using System;
using Quartz;

namespace SampleApp {
	public class SampleTriggerListener : ITriggerListener {
		public void TriggerFired(ITrigger trigger, IJobExecutionContext context) {
			Console.WriteLine(Name + ".TriggerFired");
		}

		public bool VetoJobExecution(ITrigger trigger, IJobExecutionContext context) {
			Console.WriteLine(Name + ".VetoJobExecution");
			return false;
		}

		public void TriggerMisfired(ITrigger trigger) {
			Console.WriteLine(Name + ".TriggerMisfired");
		}

		public void TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode) {
			Console.WriteLine(Name + ".TriggerComplete");
		}

		public string Name { get; set; }

		public SampleTriggerListener() {
			Name = GetType().Name;
		}
	}
}