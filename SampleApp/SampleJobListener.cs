using System;
using Quartz;

namespace SampleApp {
	public class SampleJobListener : IJobListener {
		public void JobToBeExecuted(JobExecutionContext context) {
			Console.WriteLine("JobToBeExecuted");
		}

		public void JobExecutionVetoed(JobExecutionContext context) {
			Console.WriteLine("JobExecutionVetoed");
		}

		public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
			Console.WriteLine("JobWasExecuted");
		}

		public string Name {
			get { return "SampleJobListener"; }
		}
	}
}