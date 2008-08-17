using System;
using Quartz;

namespace SampleApp {
	public class SampleJobListener : IJobListener {
		public void JobToBeExecuted(JobExecutionContext context) {
			Console.WriteLine(Name + ".JobToBeExecuted");
		}

		public void JobExecutionVetoed(JobExecutionContext context) {
			Console.WriteLine(Name + ".JobExecutionVetoed");
		}

		public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
			Console.WriteLine(Name + ".JobWasExecuted");
		}

		public string Name { get; set; }

		public SampleJobListener() {
			Name = GetType().Name;
		}
	}
}