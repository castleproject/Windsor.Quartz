using System;
using Quartz;

namespace SampleApp {
	public class SampleJobListener : IJobListener {
		public void JobToBeExecuted(JobExecutionContext context) {
			throw new NotImplementedException();
		}

		public void JobExecutionVetoed(JobExecutionContext context) {
			throw new NotImplementedException();
		}

		public void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
			throw new NotImplementedException();
		}

		public string Name {
			get { return "SampleJobListener"; }
		}
	}
}