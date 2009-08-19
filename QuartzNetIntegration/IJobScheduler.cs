using System.Collections.Generic;
using Quartz;

namespace Castle.Facilities.QuartzIntegration {
    public interface IJobScheduler {
        ICollection<string> GetJobNames();

        /// <summary>
        /// Runs a job immediately
        /// </summary>
        /// <param name="jobName"></param>
        void RunJob(string jobName);
        ICollection<string> GetExecutingJobs();

        /// <summary>
        /// Pauses all triggers
        /// </summary>
        void PauseAll();

        /// <summary>
        /// Resumes all triggers
        /// </summary>
        void ResumeAll();

        /// <summary>
        /// Pauses a job's triggers
        /// </summary>
        /// <param name="jobName"></param>
        void PauseJob(string jobName);

        /// <summary>
        /// Resumes triggers of a paused job
        /// </summary>
        /// <param name="jobName"></param>
        void ResumeJob(string jobName);
        bool DeleteJob(string jobName);

        /// <summary>
        /// Interrupts a running job
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        bool Interrupt(string jobName);

        /// <summary>
        /// Gets the job status, assuming it has only one trigger
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        TriggerState GetJobStatus(string jobName);
    }
}