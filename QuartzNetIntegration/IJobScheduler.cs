using System.Collections.Generic;

namespace QuartzNetIntegration {
	public interface IJobScheduler {
		ICollection<string> GetJobNames();
		void RunJob(string jobName);
		ICollection<string> GetExecutingJobs();
		void PauseAll();
		void ResumeAll();
		void PauseJob(string jobName);
		bool DeleteJob(string jobName);
		bool Interrupt(string jobName);
	}
}