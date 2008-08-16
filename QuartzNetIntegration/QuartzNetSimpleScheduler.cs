using System.Collections.Generic;
using System.Linq;
using Quartz;

namespace QuartzNetIntegration {
	public class QuartzNetSimpleScheduler : IJobScheduler {
		private readonly IScheduler scheduler;

		public QuartzNetSimpleScheduler(IScheduler scheduler) {
			this.scheduler = scheduler;
		}

		public bool DeleteJob(string jobName) {
			return scheduler.DeleteJob(jobName, null);
		}

		public bool Interrupt(string jobName) {
			return scheduler.Interrupt(jobName, null);
		}

		public ICollection<string> GetJobNames() {
			return scheduler.GetJobNames(null);
		}

		public void RunJob(string jobName) {
			scheduler.TriggerJob(jobName, null);
		}

		public ICollection<string> GetExecutingJobs() {
			return scheduler.GetCurrentlyExecutingJobs()
				.Cast<JobExecutionContext>()
				.Select(c => c.JobDetail.Name)
				.ToList();
		}

		public void PauseAll() {
			scheduler.PauseAll();
		}

		public void ResumeAll() {
			scheduler.ResumeAll();
		}

		public void PauseJob(string jobName) {
			scheduler.PauseJob(jobName, null);
		}
	}
}