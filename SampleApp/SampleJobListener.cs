using System;
using Quartz;

namespace SampleApp {
	public class SampleJobListener : IJobListener {
		public void JobToBeExecuted(IJobExecutionContext context) {
			Console.WriteLine(Name + ".JobToBeExecuted");
		}

		public void JobExecutionVetoed(IJobExecutionContext context) {
			Console.WriteLine(Name + ".JobExecutionVetoed");
		}

		public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException) {
			Console.WriteLine(Name + ".JobWasExecuted");
		}

		public string Name { get; set; }

		public SampleJobListener() {
			Name = GetType().Name;
		}
	}
}