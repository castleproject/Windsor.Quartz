using System;
using Quartz;

namespace SampleApp {
	public class SampleSchedulerListener : ISchedulerListener {
		public void JobScheduled(Trigger trigger) {
			Console.WriteLine(GetType().Name + ".JobScheduled");
		}

		public void JobUnscheduled(string triggerName, string triggerGroup) {
			Console.WriteLine(GetType().Name + ".JobUnscheduled");
		}

		public void TriggerFinalized(Trigger trigger) {
			Console.WriteLine(GetType().Name + ".TriggerFinalized");
		}

		public void TriggersPaused(string triggerName, string triggerGroup) {
			Console.WriteLine(GetType().Name + ".TriggersPaused");
		}

		public void TriggersResumed(string triggerName, string triggerGroup) {
			Console.WriteLine(GetType().Name + ".TriggersResumed");
		}

		public void JobsPaused(string jobName, string jobGroup) {
			Console.WriteLine(GetType().Name + ".JobsPaused");
		}

		public void JobsResumed(string jobName, string jobGroup) {
			Console.WriteLine(GetType().Name + ".JobsResumed");
		}

		public void SchedulerError(string msg, SchedulerException cause) {
			Console.WriteLine(GetType().Name + ".SchedulerError");
		}

		public void SchedulerShutdown() {
			Console.WriteLine(GetType().Name + ".SchedulerShutdown");
		}
	}
}